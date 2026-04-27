using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Settlements.Results;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;
using SoulViet.Shared.Application.Interfaces;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Settlements.Queries.GetPayoutBatchDetails;

public class GetPayoutBatchDetailsHandler : IRequestHandler<GetPayoutBatchDetailsQuery, PayoutBatchDetailResponse>
{
    private readonly IPayoutBatchRepository _payoutBatchRepository;
    private readonly IPartnerIntegrationService _partnerIntegrationService;
    public GetPayoutBatchDetailsHandler(IPayoutBatchRepository payoutBatchRepository, IPartnerIntegrationService partnerIntegrationService)
    {
        _payoutBatchRepository = payoutBatchRepository;
        _partnerIntegrationService = partnerIntegrationService;
    }

    public async Task<PayoutBatchDetailResponse> Handle(GetPayoutBatchDetailsQuery request, CancellationToken cancellationToken)
    {
        var batchDetails = await _payoutBatchRepository.GetBatchDetailsAsync(request.PayoutBatchId, cancellationToken);
        if (batchDetails == null || !batchDetails.Items.Any())
            return batchDetails!;

        var customerIds = batchDetails.Items.Select(x => x.CustomerId).Distinct().ToList();
        var userNamesDict = await _partnerIntegrationService.GetPartnerNamesAsync(customerIds, cancellationToken);

        foreach (var item in batchDetails.Items)
        {
            item.CustomerName = userNamesDict.TryGetValue(item.CustomerId, out var name) ? name : "Customer Anonymous";
        }

        return batchDetails;
    }
}