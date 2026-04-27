using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.Common.Models;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Settlements.Results;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Settlements.Queries.GetPayoutBatchesForPartner;

public class GetPayoutBatchesForPartnerHandler : IRequestHandler<GetPayoutBatchesForPartnerQuery, PaginatedList<PayoutBatchPartnerResponse>>
{
    private readonly IPayoutBatchRepository _payoutBatchRepository;
    public GetPayoutBatchesForPartnerHandler(IPayoutBatchRepository payoutBatchRepository)
    {
        _payoutBatchRepository = payoutBatchRepository;
    }

    public async Task<PaginatedList<PayoutBatchPartnerResponse>> Handle(GetPayoutBatchesForPartnerQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _payoutBatchRepository.GetPagedBatchesForPartnerAsync(
            request.PartnerId, request.Status, request.PageNumber, request.PageSize, cancellationToken
        );

        return new PaginatedList<PayoutBatchPartnerResponse>(
            items,
            totalCount,
            request.PageNumber,
            request.PageSize
        );
    }
}