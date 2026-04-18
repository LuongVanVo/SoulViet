using Microsoft.Extensions.Logging;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Enums;

namespace SoulViet.Modules.Marketplace.Marketplace.Infrastructure.Services;

public class PaymentTimeoutService : IPaymentTimeoutService
{
    private readonly IMasterOrderRepository _masterOrderRepository;
    private readonly IMarketplaceProductRepository _marketplaceProductRepository;
    private readonly IVoucherRepository _voucherRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PaymentTimeoutService> _logger;
    public PaymentTimeoutService(IMasterOrderRepository masterOrderRepository, IMarketplaceProductRepository marketplaceProductRepository, IVoucherRepository voucherRepository, IUnitOfWork unitOfWork, ILogger<PaymentTimeoutService> logger)
    {
        _masterOrderRepository = masterOrderRepository;
        _marketplaceProductRepository = marketplaceProductRepository;
        _voucherRepository = voucherRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task ProcessTimeoutAsync(Guid masterOrderId, CancellationToken cancellationToken = default)
    {
        var masterOrder = await _masterOrderRepository.GetByIdWithDetailsAsync(masterOrderId, cancellationToken);
        if (masterOrder == null || masterOrder.PaymentStatus != PaymentStatus.Pending)
        {
            return;
        }

        _logger.LogInformation("Order {OrderId} has timed out payment. Processing cancellation and inventory restoration.", masterOrderId);

        await _unitOfWork.BeginTransactionAsync(CancellationToken.None);
        try
        {
            masterOrder.PaymentStatus = PaymentStatus.Failed;

            foreach (var vendorOrder in masterOrder.VendorOrders)
            {
                vendorOrder.Status = OrderStatus.Cancelled;

                // Rollback stock
                foreach (var item in vendorOrder.OrderItems)
                {
                    var product =
                        await _marketplaceProductRepository.GetByIdAsync(item.ProductId, CancellationToken.None);
                    if (product != null)
                    {
                        product.Stock += item.Quantity;
                        await _marketplaceProductRepository.UpdateAsync(product, CancellationToken.None);
                    }
                }

                // Rollback Shop Voucher
                if (!string.IsNullOrEmpty(vendorOrder.ShopVoucherCode))
                {
                    var shopVoucher = await _voucherRepository.GetByCodeAsync(vendorOrder.ShopVoucherCode, vendorOrder.PartnerId, CancellationToken.None);
                    if (shopVoucher != null && shopVoucher.UsedCount > 0)
                    {
                        shopVoucher.UsedCount -= 1;
                        _voucherRepository.Update(shopVoucher);
                    }
                }
            }

            // Rollback Platform Voucher
            if (!string.IsNullOrEmpty(masterOrder.PlatformVoucherCode))
            {
                var platformVoucher = await _voucherRepository.GetByCodeAsync(masterOrder.PlatformVoucherCode, null, CancellationToken.None);
                if (platformVoucher != null && platformVoucher.UsedCount > 0)
                {
                    platformVoucher.UsedCount -= 1;
                    _voucherRepository.Update(platformVoucher);
                }
            }

            await _unitOfWork.SaveChangesAsync(CancellationToken.None);
            await _unitOfWork.CommitTransactionAsync(CancellationToken.None);

                _logger.LogInformation("Successfully processed payment timeout for order {OrderId}. Order cancelled and inventory restored.", masterOrderId);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(CancellationToken.None);
            _logger.LogError(ex, "Error processing payment timeout for order {OrderId}", masterOrderId);
        }
    }
}