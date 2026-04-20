using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.Exceptions;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Results;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Enums;
using SoulViet.Shared.Application.Interfaces.Repositories;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Queries.PreviewOrder;

public class PreviewOrderHandler : IRequestHandler<PreviewOrderQuery, PreviewOrderResponse>
{
    private readonly ICartRepository _cartRepository;
    private readonly IMarketplaceProductRepository _marketplaceProductRepository;
    private readonly IVoucherRepository _voucherRepository;
    private readonly IUserRepository _userRepository;
    public PreviewOrderHandler(ICartRepository cartRepository, IMarketplaceProductRepository marketplaceProductRepository, IVoucherRepository voucherRepository, IUserRepository userRepository)
    {
        _cartRepository = cartRepository;
        _marketplaceProductRepository = marketplaceProductRepository;
        _voucherRepository = voucherRepository;
        _userRepository = userRepository;
    }

    public async Task<PreviewOrderResponse> Handle(PreviewOrderQuery request, CancellationToken cancellationToken)
    {
        if (!request.SelectedCartItemIds.Any())
            throw new BadRequestException("Please select at least one item to checkout.");

        // Get cart items
        var cartItems =
            await _cartRepository.GetItemsByIdsAsync(request.SelectedCartItemIds, request.UserId, cancellationToken);
        if (cartItems.Count != request.SelectedCartItemIds.Count)
            throw new BadRequestException("Some items in the cart are invalid or no longer available.");

        var response = new PreviewOrderResponse();
        decimal masterSubTotal = 0;
        decimal masterShippingFee = 0;

        // Group cart items by shop
        var groudedItems = cartItems.GroupBy(c => c.MarketplaceProduct.PartnerId);

        foreach (var group in groudedItems)
        {
            var partnerId = group.Key;
            var vendorOrder = new PreviewVendorOrder { PartnerId = partnerId };

            bool hasPhysicalGoods = false;

            // Calculate subtotal for this shop
            foreach (var cartItem in group)
            {
                var itemTotalPrice = (cartItem.MarketplaceProduct.PromotionalPrice ?? cartItem.MarketplaceProduct.Price) * cartItem.Quantity;
                vendorOrder.SubTotal += itemTotalPrice;

                vendorOrder.Items.Add(new PreviewOrderItem
                {
                    CartItemId = cartItem.Id,
                    ProductId = cartItem.MarketplaceProductId,
                    ProductName = cartItem.MarketplaceProduct.Name,
                    UnitPrice = cartItem.MarketplaceProduct.PromotionalPrice ?? cartItem.MarketplaceProduct.Price,
                    Quantity = cartItem.Quantity
                });

                if (cartItem.MarketplaceProduct.ProductType == ProductType.PhysicalGoods)
                    hasPhysicalGoods = true;
            }

            // Calculate shipping fee (for simplicity, we use a flat fee if there are physical goods)
            // Tạm tính: nếu có hàng vật lý thì phí ship đồng giá 30k, nếu là tour hoặc vé thì không tính phí ship (update phí ship sau khi có thêm thông tin về địa điểm giao hàng)
            vendorOrder.ShippingFee = hasPhysicalGoods ? 30000.0m : 0m;

            // Apply shop voucher if provided
            if (request.ShopVoucherCodes.TryGetValue(partnerId, out var shopVoucherCode) &&
                !string.IsNullOrEmpty(shopVoucherCode))
            {
                var voucher =
                    await _voucherRepository.GetValidVoucherAsync(shopVoucherCode, partnerId, cancellationToken);

                if (voucher == null)
                    throw new BadRequestException($"Shop voucher '{shopVoucherCode}' is invalid or expired");
                if (vendorOrder.SubTotal < voucher.MinOrderAmount)
                    throw new BadRequestException($"Shop voucher requires min order of {voucher.MinOrderAmount}.");

                vendorOrder.ShopVoucherCode = voucher.Code;
                vendorOrder.ShopDiscountAmount = CalculateDiscount(voucher.DiscountType, voucher.DiscountValue,
                    voucher.MaxDiscountAmount, vendorOrder.SubTotal, vendorOrder.ShippingFee, hasPhysicalGoods);
            }

            // Confirm price for this shop
            vendorOrder.TotalAmount = vendorOrder.SubTotal + vendorOrder.ShippingFee - vendorOrder.ShopDiscountAmount;

            // Data must not be negative
            if (vendorOrder.TotalAmount < 0) vendorOrder.TotalAmount = 0;

            response.VendorOrders.Add(vendorOrder);

            // Update master totals
            masterSubTotal += vendorOrder.SubTotal;
            masterShippingFee += vendorOrder.ShippingFee;
        }

        // Handle master order
        response.TotalItemsPrice = masterSubTotal;
        response.TotalShippingFee = masterShippingFee;
        decimal totalAmountBeforePlatformVoucher = response.VendorOrders.Sum(v => v.TotalAmount); // Tổng tiền sau khi đã áp dụng voucher shop

        // Apply platform voucher if provided
        if (!string.IsNullOrEmpty(request.PlatformVoucherCode))
        {
            var platformVoucher = await _voucherRepository.GetValidVoucherAsync(request.PlatformVoucherCode, null, cancellationToken);

            if (platformVoucher == null)
                throw new BadRequestException("Platform voucher is invalid or expired.");
            if (totalAmountBeforePlatformVoucher < platformVoucher.MinOrderAmount)
                throw new BadRequestException(
                    $"Platform voucher requires min order of {platformVoucher.MinOrderAmount}");

            // Logic Freeship của sàn cũng phải check xem tổng đơn hàng vật lý không
            bool anyPhysicalGoods = response.VendorOrders.Any(vo => vo.ShippingFee > 0);

            response.PlatformVoucherCode = platformVoucher.Code;
            response.PlatformDiscountAmount = CalculateDiscount(platformVoucher.DiscountType,
                platformVoucher.DiscountValue, platformVoucher.MaxDiscountAmount, totalAmountBeforePlatformVoucher,
                masterShippingFee, anyPhysicalGoods);
        }

        // Final grand total
        response.GrandTotal = totalAmountBeforePlatformVoucher - response.PlatformDiscountAmount;
        if (response.GrandTotal < 0) response.GrandTotal = 0;

        decimal soulCoinToUse = 0;

        if (request.UseSoulCoin && request.SoulCoinAmountToUse > 0)
        {
            var user = await _userRepository.GetUserByIdAsync(request.UserId);
            if (user == null) throw new BadRequestException("User not found.");

            if (user.SoulCoinBalance < request.SoulCoinAmountToUse)
                throw new BadRequestException("Insufficient Soul Coin balance.");

            soulCoinToUse = (int)Math.Min((decimal)request.SoulCoinAmountToUse, response.GrandTotal);
        }

        response.SoulCoinUsed = soulCoinToUse;
        response.FinalPayableAmount = response.GrandTotal - soulCoinToUse;

        return response;
    }

    private decimal CalculateDiscount(DiscountType type, decimal value, decimal? maxAmount, decimal subTotal,
        decimal shippingFee, bool hasPhysicalGoods)
    {
        decimal discountAmount = 0;

        switch (type)
        {
            case DiscountType.FixedAmount:
                discountAmount = value;
                break;

            case DiscountType.Percentage:
                discountAmount = subTotal * (value / 100);
                if (maxAmount.HasValue && discountAmount > maxAmount.Value)
                    discountAmount = maxAmount.Value;
                break;

            case DiscountType.FreeShipping:
                if (!hasPhysicalGoods)
                    throw new BadRequestException("Free shipping voucher cannot be applied to orders with only non-physical products.");

                discountAmount = shippingFee;
                if (maxAmount.HasValue && discountAmount > maxAmount.Value)
                    discountAmount = maxAmount.Value;
                break;
        }

        return discountAmount;
    }
}