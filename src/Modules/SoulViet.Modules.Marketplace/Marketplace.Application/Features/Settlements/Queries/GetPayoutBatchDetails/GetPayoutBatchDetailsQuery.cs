using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Settlements.Results;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Settlements.Queries.GetPayoutBatchDetails;

public class GetPayoutBatchDetailsQuery : IRequest<PayoutBatchDetailResponse>
{
    public Guid PayoutBatchId { get; set; }
}