using Hangfire;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Http;
using SoulViet.Modules.Marketplace.Marketplace.Application.Exceptions;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Results;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Enums;
using SoulViet.Modules.Marketplace.Marketplace.Infrastructure.Services;
using SoulViet.Shared.Application.Common.Events;
using SoulViet.Shared.Application.Interfaces.Repositories;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderHandler(
    ICartRepository cartRepository,
    IMarketplaceProductRepository marketplaceProductRepository,
    IVoucherRepository voucherRepository,
    IMasterOrderRepository masterOrderRepository,
    IUnitOfWork unitOfWork,
    IVnPayService vnPayService,
    IHttpContextAccessor httpContextAccessor,
    IPublishEndpoint publishEndpoint,
    IUserRepository userRepository,
    IBackgroundJobClient backgroundJobClient,
    ISoulCoinTransactionRepository soulCoinTransactionRepository)
    : IRequestHandler<CreateOrderCommand, CreateOrderResponse>
{
    private readonly ICartRepository _cartRepository = cartRepository;
    private readonly IMarketplaceProductRepository _marketplaceProductRepository = marketplaceProductRepository;
    private readonly IVoucherRepository _voucherRepository = voucherRepository;
    private readonly IMasterOrderRepository _masterOrderRepository = masterOrderRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IVnPayService _vnPayService = vnPayService;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IBackgroundJobClient _backgroundJobClient = backgroundJobClient;
    private readonly ISoulCoinTransactionRepository _soulCoinTransactionRepository = soulCoinTransactionRepository;

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

            // Process SoulCoin usage
            int soulCoinToUse = 0;

            if (request.UseSoulCoin && request.SoulCoinAmountToUse > 0)
            {
                var user = await _userRepository.GetUserByIdAsync(request.UserId);
                if (user == null) throw new BadRequestException("User not found.");

                if (user.SoulCoinBalance < request.SoulCoinAmountToUse)
                    throw new BadRequestException("Insufficient SoulCoin balance.");

                soulCoinToUse = (int)Math.Min((decimal)request.SoulCoinAmountToUse, masterOrder.GrandTotal);

                // subtract SoulCoin from user balance
                user.SoulCoinBalance -= soulCoinToUse;
                await _userRepository.UpdateUserAsync(user);

                // record SoulCoin transaction
                var coinTransaction = new SoulCoinTransaction
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Amount = -soulCoinToUse,
                    Type = SoulCoinTransactionType.Payment,
                    ReferenceId = masterOrder.Id.ToString(),
                    Description = $"Pay for order {masterOrder.Id}",
                    CreatedAt = DateTime.UtcNow
                };
                await _soulCoinTransactionRepository.AddAsync(coinTransaction, cancellationToken);
            }

            // Update fields new payment into MasterOrder
            masterOrder.SoulCoinUsed = soulCoinToUse;
            masterOrder.FinalPayableAmount = masterOrder.GrandTotal - soulCoinToUse;

            if (masterOrder.FinalPayableAmount == 0)
            {
                masterOrder.PaymentStatus = PaymentStatus.Success;
                foreach (var vendorOrder in masterOrder.VendorOrders)
                {
                    vendorOrder.Status = OrderStatus.Processing;
                }
            }

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

            // 7. Generate payment URL if needed
            string? paymentUrl = null;
            if (request.PaymentMethod == PaymentMethod.VnPay && masterOrder.FinalPayableAmount > 0)
            {
                var context = _httpContextAccessor.HttpContext;
                if (context != null)
                {
                    paymentUrl = _vnPayService.CreatePaymentUrl(masterOrder, context);
                }

                // set a timeout of 24h with background job to automatically cancel order if payment not completed
                _backgroundJobClient.Schedule<IPaymentTimeoutService>(
                    service => service.ProcessTimeoutAsync(masterOrder.Id, CancellationToken.None),
                    TimeSpan.FromDays(1)
                );

                // _backgroundJobClient.Schedule<IPaymentTimeoutService>(
                //     service => service.ProcessTimeoutAsync(masterOrder.Id, CancellationToken.None),
                //     TimeSpan.FromMinutes(1)
                // );
            }

            // Send mail notification to user and partner can be triggered
            var acceptLanguage = _httpContextAccessor.HttpContext?.Request.Headers["Accept-Language"].ToString() ?? "vi";
            var isVietnamese = acceptLanguage.StartsWith("vi", StringComparison.OrdinalIgnoreCase);
            var userLanguage = isVietnamese ? "vi" : "en";

            try
            {
                await _publishEndpoint.Publish(new UserOrderCreatedEvent()
                {
                    MasterOrderId = masterOrder.Id,
                    UserId = request.UserId,
                    ReceiverName = request.ReceiverName,
                    ReceiverEmail = request.ReceiverEmail,
                    GrandTotal = masterOrder.GrandTotal,
                    Language = userLanguage
                }, cancellationToken);

                foreach (var vendorOrder in masterOrder.VendorOrders)
                {
                    var partner = await _userRepository.GetUserByIdAsync(vendorOrder.PartnerId);
                    if (partner != null && !string.IsNullOrEmpty(partner.Email))
                    {
                        var partnerEmail = partner.Email;
                        await _publishEndpoint.Publish(new PartnerOrderCreatedEvent
                        {
                            OrderId = vendorOrder.Id,
                            PartnerId = vendorOrder.PartnerId,
                            PartnerEmail = partnerEmail,
                            CustomerName = request.ReceiverName,
                            TotalAmount = Math.Max(0, vendorOrder.TotalAmount - vendorOrder.ShopDiscountAmount),
                            Language = "vi"
                        }, cancellationToken);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send notification emails: {ex.Message}");
            }

            return new CreateOrderResponse
            {
                Success = true,
                Message = "Order created successfully.",
                MasterOrderId = masterOrder.Id,
                GrandTotal = masterOrder.GrandTotal,
                PaymentUrl = paymentUrl,
                SoulCoinUsed = request.UseSoulCoin,
                FinalPayableAmount = masterOrder.FinalPayableAmount
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