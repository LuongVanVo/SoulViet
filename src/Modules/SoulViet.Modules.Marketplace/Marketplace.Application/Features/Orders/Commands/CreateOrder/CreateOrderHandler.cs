using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.Exceptions;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Results;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Enums;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderHandler(
    ICartRepository cartRepository,
    IMarketplaceProductRepository marketplaceProductRepository,
    IVoucherRepository voucherRepository,
    IMasterOrderRepository masterOrderRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateOrderCommand, CreateOrderResponse>
{
    private readonly ICartRepository _cartRepository = cartRepository;
    private readonly IMarketplaceProductRepository _marketplaceProductRepository = marketplaceProductRepository;
    private readonly IVoucherRepository _voucherRepository = voucherRepository;
    private readonly IMasterOrderRepository _masterOrderRepository = masterOrderRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<CreateOrderResponse> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        if (!request.SelectedCartItemIds.Any())
            throw new BadRequestException("No items selected for checkout.");

        // start transaction
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // 1. Get and validate cart items
            var cartItems =
                await _cartRepository.GetItemsByIdsAsync(request.SelectedCartItemIds, request.UserId,
                    cancellationToken);
            if (cartItems.Count != request.SelectedCartItemIds.Count)
                throw new BadRequestException("Cart items mismatch. Please refresh your cart and try again.");

            // Init master order
            var masterOrder = new MasterOrder
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                PaymentMethod = request.PaymentMethod,
                PaymentStatus = PaymentStatus.Pending,
                TotalItemsPrice = 0,
                TotalShippingFee = 0,
                PlatformDiscountAmount = 0,
                VendorOrders = new List<Order>()
            };

            var groupedItems = cartItems.GroupBy(c => c.MarketplaceProduct.PartnerId);

            // 2. Process each shop's order
            foreach (var group in groupedItems)
            {
                var partnerId = group.Key;
                var vendorOrder = new Order
                {
                    Id = Guid.NewGuid(),
                    MasterOrderId = masterOrder.Id,
                    PartnerId = partnerId,
                    UserId = request.UserId,
                    ReceiverName = request.ReceiverName,
                    ReceiverPhone = request.ReceiverPhone,
                    ShippingAddress = request.ShippingAddress,
                    OrderNotes = request.OrderNotes,
                    Status = OrderStatus.Pending,
                    TotalAmount = 0,
                    OrderItems = new List<OrderItem>()
                };

                decimal subTotal = 0;
                bool hasPhysicalGoods = false;

                foreach (var cartItem in group)
                {
                    var product = cartItem.MarketplaceProduct;

                    // subtract stock quantity
                    if (product.Stock < cartItem.Quantity)
                        throw new BadRequestException($"Product '{product.Name}' is out of stock. Available quantity: {product.Stock}.");

                    product.Stock -= cartItem.Quantity;
                    await _marketplaceProductRepository.UpdateAsync(product, cancellationToken);

                    // Create order item
                    var orderItem = new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        OrderId = vendorOrder.Id,
                        ProductId = product.Id,
                        ProductNameSnapshot = product.Name,
                        ProductImageSnapshot = product.Media.MainImage ?? "",
                        Quantity = cartItem.Quantity,
                        UnitPrice = product.PromotionalPrice ?? product.Price,
                        ItemMetadata = cartItem.ItemMetadata,
                        CommissionRate = 0, // Tạm set phí hoa hồng bằng 0, có thể tính toán logic % phí nền tảng ở đây sau
                        PlatformFee = 0,
                        PartnerEarnings = (product.PromotionalPrice ?? product.Price) * cartItem.Quantity
                    };

                    vendorOrder.OrderItems.Add(orderItem);
                    subTotal += orderItem.UnitPrice * orderItem.Quantity;

                    if (product.ProductType == ProductType.PhysicalGoods)
                        hasPhysicalGoods = true;
                }

                // Calculate shipping fee (for simplicity, we use a flat fee if there are physical goods)
                vendorOrder.ShippingFee = hasPhysicalGoods ? 30000.0m : 0m;

                // Process voucher of shop
                decimal shopDiscountAmount = 0;
                if (request.ShopVoucherCodes.TryGetValue(partnerId, out var shopVoucherCode) &&
                    !string.IsNullOrEmpty(shopVoucherCode))
                {
                    var voucher =
                        await _voucherRepository.GetValidVoucherAsync(shopVoucherCode, partnerId, cancellationToken);
                    if (voucher == null || subTotal < voucher.MinOrderAmount)
                        throw new BadRequestException($"Shop voucher '{shopVoucherCode}' is invalid.");

                    shopDiscountAmount =
                        CalculateDiscount(voucher, subTotal, vendorOrder.ShippingFee, hasPhysicalGoods);

                    // Update voucher usage
                    voucher.UsedCount += 1;
                    _voucherRepository.Update(voucher);

                    vendorOrder.ShopVoucherCode = voucher.Code;
                    vendorOrder.ShopDiscountAmount = shopDiscountAmount;
                }

                // Finalize vendor order
                vendorOrder.TotalAmount = Math.Max(0, subTotal + vendorOrder.ShippingFee - shopDiscountAmount);
                masterOrder.VendorOrders.Add(vendorOrder);

                masterOrder.TotalItemsPrice += subTotal;
                masterOrder.TotalShippingFee += vendorOrder.ShippingFee;
            }

            // 3. Process voucher of platform (on sum subtracted by shop voucher)
            decimal totalBeforePlatformVoucher = masterOrder.VendorOrders.Sum(v => v.TotalAmount);

            if (!string.IsNullOrEmpty(request.PlatformVoucherCode))
            {
                var platformVoucher = await _voucherRepository.GetValidVoucherAsync(request.PlatformVoucherCode, null, cancellationToken);
                if (platformVoucher == null || totalBeforePlatformVoucher < platformVoucher.MinOrderAmount)
                    throw new BadRequestException("Platform voucher is invalid");

                bool anyPhysicalGoods = masterOrder.VendorOrders.Any(vo => vo.ShippingFee > 0);
                masterOrder.PlatformDiscountAmount = CalculateDiscount(platformVoucher, totalBeforePlatformVoucher,
                    masterOrder.TotalShippingFee, anyPhysicalGoods);
                masterOrder.PlatformVoucherCode = platformVoucher.Code;

                // Update voucher usage
                platformVoucher.UsedCount += 1;
                _voucherRepository.Update(platformVoucher);
            }

            // Finalize master order
            masterOrder.GrandTotal = Math.Max(0, totalBeforePlatformVoucher - masterOrder.PlatformDiscountAmount);

            // 4. Save to database
            await _masterOrderRepository.AddAsync(masterOrder, cancellationToken);

            // 5. Clear purchased items from cart
            var currentCart = await _cartRepository.GetByUserIdAsync(request.UserId, cancellationToken);
            if (currentCart != null)
            {
                currentCart.Items = currentCart.Items.Where(i => !request.SelectedCartItemIds.Contains(i.Id)).ToList();
                await _cartRepository.SaveCartAsync(request.UserId, currentCart, cancellationToken);
            }

            // 6. Commit transaction
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return new CreateOrderResponse
            {
                Success = true,
                Message = "Order created successfully.",
                MasterOrderId = masterOrder.Id,
                GrandTotal = masterOrder.GrandTotal
            };
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    private decimal CalculateDiscount(Voucher voucher, decimal subTotal, decimal shippingFee, bool hasPhysicalGoods)
    {
        decimal discountAmount = 0;
        switch (voucher.DiscountType)
        {
            case DiscountType.FixedAmount:
                discountAmount = voucher.DiscountValue;
                break;
            case DiscountType.Percentage:
                discountAmount = subTotal * (voucher.DiscountValue / 100);
                if (voucher.MaxDiscountAmount.HasValue && discountAmount > voucher.MaxDiscountAmount.Value)
                    discountAmount = voucher.MaxDiscountAmount.Value;
                break;
            case DiscountType.FreeShipping:
                if (!hasPhysicalGoods)
                    throw new BadRequestException("Free shipping voucher cannot be applied to orders without physical goods.");
                discountAmount = shippingFee;
                if (voucher.MaxDiscountAmount.HasValue && discountAmount > voucher.MaxDiscountAmount.Value)
                    discountAmount = voucher.MaxDiscountAmount.Value;
                break;
        }

        return discountAmount;
    }
}