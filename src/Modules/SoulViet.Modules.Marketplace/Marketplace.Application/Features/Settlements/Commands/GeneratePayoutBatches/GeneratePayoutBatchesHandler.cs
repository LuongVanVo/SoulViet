using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Enums;
using SoulViet.Shared.Application.Interfaces;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Settlements.Commands.GeneratePayoutBatches;

public class GeneratePayoutBatchesHandler : IRequestHandler<GeneratePayoutBatchesCommand, bool>
{
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly IPayoutBatchRepository _payoutBatchRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPartnerIntegrationService _partnerIntegrationService;
    public GeneratePayoutBatchesHandler(IOrderItemRepository orderItemRepository, IPayoutBatchRepository payoutBatchRepository ,IUnitOfWork unitOfWork, IPartnerIntegrationService partnerIntegrationService)
    {
        _orderItemRepository = orderItemRepository;
        _payoutBatchRepository = payoutBatchRepository;
        _unitOfWork = unitOfWork;
        _partnerIntegrationService = partnerIntegrationService;
    }

    public async Task<bool> Handle(GeneratePayoutBatchesCommand request, CancellationToken cancellationToken)
    {
        var unsettledItems = await _orderItemRepository.GetUnsettledItemsAsync(request.EndDate, cancellationToken);
        if (!unsettledItems.Any()) return true;

        var groupedByPartner = unsettledItems.GroupBy(i => i.Order.PartnerId);

        var partnerIds = groupedByPartner.Select(g => g.Key).ToList();

        var partnerNamesDict = await _partnerIntegrationService.GetPartnerNamesAsync(partnerIds, cancellationToken);

        var newBatches = new List<PayoutBatch>();
        var itemsToUpdate = new List<OrderItem>();

        foreach (var group in groupedByPartner)
        {
            var partnerId = group.Key;
            var items = group.ToList();

            // Calculate
            decimal totalSales = items.Sum(x => x.UnitPrice * x.Quantity);
            decimal totalCommission = items.Sum(x => (x.UnitPrice * x.Quantity) * (x.CommissionRate / 100));
            decimal netPayout = items.Sum(x => x.PartnerEarnings);

            var partnerName = partnerNamesDict.TryGetValue(partnerId, out var name) ? name : "N/A";
            // Create payout batch
            var payoutBatch = new PayoutBatch
            {
                Id = Guid.NewGuid(),
                PartnerId = partnerId,
                PartnerNameSnapshot = partnerName,
                PeriodStart = items.Min(x => x.CreatedAt),
                PeriodEnd = request.EndDate,
                TotalSales = totalSales,
                TotalCommission = totalCommission,
                NetPayout = netPayout,
                Status = SettlementStatus.Pending
            };

            newBatches.Add(payoutBatch);

            // Update status item
            foreach (var item in items)
            {
                item.IsSettled = true;
                item.PayoutBatchId = payoutBatch.Id;
                itemsToUpdate.Add(item);
            }
        }

        // Save to database
        await _payoutBatchRepository.AddRangeAsync(newBatches, cancellationToken);
        _orderItemRepository.UpdateRange(itemsToUpdate);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}