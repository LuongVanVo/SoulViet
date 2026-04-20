using MediatR;
using Microsoft.Extensions.Logging;
using SoulViet.Modules.Marketplace.Marketplace.Application.Exceptions;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Enums;
using SoulViet.Shared.Application.Interfaces.Repositories;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Commands.CancelOrder;

public class CancelOrderHandler : IRequestHandler<CancelOrderCommand, bool>
{
    private readonly IMasterOrderRepository _masterOrderRepository;
    private readonly IMarketplaceProductRepository _marketplaceProductRepository;
    private readonly IVoucherRepository _voucherRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISoulCoinTransactionRepository _soulCoinTransactionRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<CancelOrderHandler> _logger;
    public CancelOrderHandler(IMasterOrderRepository masterOrderRepository, IMarketplaceProductRepository marketplaceProductRepository, IVoucherRepository voucherRepository, IUnitOfWork unitOfWork, ISoulCoinTransactionRepository soulCoinTransactionRepository, IUserRepository userRepository, ILogger<CancelOrderHandler> logger)
    {
        _masterOrderRepository = masterOrderRepository;
        _marketplaceProductRepository = marketplaceProductRepository;
        _voucherRepository = voucherRepository;
        _unitOfWork = unitOfWork;
        _soulCoinTransactionRepository = soulCoinTransactionRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<bool> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var masterOrder =
            await _masterOrderRepository.GetByIdWithDetailsAsync(request.MasterOrderId, cancellationToken);
        if (masterOrder == null || masterOrder.UserId != request.UserId)
            throw new NotFoundException("Order not found or does not belong to user.");

        if (masterOrder.PaymentStatus != PaymentStatus.Pending)
            throw new BadRequestException("Only orders with pending payment can be cancelled.");

        // Transaction cancel order and restore inventory
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            masterOrder.PaymentStatus = PaymentStatus.Canceled;

            foreach (var vendorOrder in masterOrder.VendorOrders)
            {
                vendorOrder.Status = OrderStatus.Cancelled;

                // Restore stock
                foreach (var item in vendorOrder.OrderItems)
                {
                    var product = await _marketplaceProductRepository.GetByIdAsync(item.ProductId, cancellationToken);
                    if (product != null)
                    {
                        product.Stock += item.Quantity;
                        await _marketplaceProductRepository.UpdateAsync(product, cancellationToken);
                    }
                }

                // Restore Shop Voucher
                if (!string.IsNullOrEmpty(vendorOrder.ShopVoucherCode))
                {
                    var shopVoucher = await _voucherRepository.GetByCodeAsync(vendorOrder.ShopVoucherCode,
                        vendorOrder.PartnerId, cancellationToken);
                    if (shopVoucher != null && shopVoucher.UsedCount > 0)
                    {
                        shopVoucher.UsedCount -= 1;
                        _voucherRepository.Update(shopVoucher);
                    }
                }
            }

            // Restore Platform Voucher
            if (!string.IsNullOrEmpty(masterOrder.PlatformVoucherCode))
            {
                var platformVoucher =
                    await _voucherRepository.GetByCodeAsync(masterOrder.PlatformVoucherCode, null, cancellationToken);
                if (platformVoucher != null && platformVoucher.UsedCount > 0)
                {
                    platformVoucher.UsedCount -= 1;
                    _voucherRepository.Update(platformVoucher);
                }
            }

            // Restore SoulCoin
            if (masterOrder.SoulCoinUsed > 0)
            {
                var user = await _userRepository.GetUserByIdAsync(masterOrder.UserId);
                if (user != null)
                {
                    // Refund SoulCoin to user
                    user.SoulCoinBalance += (int)masterOrder.SoulCoinUsed;
                    await _userRepository.UpdateUserAsync(user);

                    var coinTransaction = new SoulCoinTransaction
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        Amount = masterOrder.SoulCoinUsed,
                        Type = SoulCoinTransactionType.Refund,
                        ReferenceId = masterOrder.Id.ToString(),
                        Description = $"Refund SoulCoin for cancelled order {masterOrder.Id}",
                        CreatedAt = DateTime.UtcNow
                    };
                    await _soulCoinTransactionRepository.AddAsync(coinTransaction, cancellationToken);
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation("Tourist {UserId} has successfully cancelled the order {OrderId}", request.UserId, request.MasterOrderId);
            return true;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(ex, "Error occurred while cancelling order {OrderId}", request.MasterOrderId);
            throw new BadRequestException("An error occurred while cancelling the order. Please try again.");
        }
    }
}