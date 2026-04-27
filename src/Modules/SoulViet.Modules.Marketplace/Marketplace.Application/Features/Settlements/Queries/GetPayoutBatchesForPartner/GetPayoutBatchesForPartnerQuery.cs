using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.Common.Models;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Settlements.Results;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Enums;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Features.Settlements.Queries.GetPayoutBatchesForPartner;

public class GetPayoutBatchesForPartnerQuery : IRequest<PaginatedList<PayoutBatchPartnerResponse>>
{
    public Guid PartnerId { get; set; }
    public SettlementStatus? Status { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}