using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.Common.Models;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Settlements.Results;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Settlements.Queries.GetPayoutBatchesForAdmin;

public class GetPayoutBatchesForAdminHandler : IRequestHandler<GetPayoutBatchesForAdminQuery, PaginatedList<PayoutBatchAdminResponse>>
{
    private readonly IPayoutBatchRepository _payoutBatchRepository;
    public GetPayoutBatchesForAdminHandler(IPayoutBatchRepository payoutBatchRepository)
    {
        _payoutBatchRepository = payoutBatchRepository;
    }

    public async Task<PaginatedList<PayoutBatchAdminResponse>> Handle(GetPayoutBatchesForAdminQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _payoutBatchRepository.GetPagedBatchesForAdminAsync(
            request.SearchName ,request.Status, request.PageNumber, request.PageSize, cancellationToken
        );

        return new PaginatedList<PayoutBatchAdminResponse>(
            items,
            totalCount,
            request.PageNumber,
            request.PageSize
        );
    }
}