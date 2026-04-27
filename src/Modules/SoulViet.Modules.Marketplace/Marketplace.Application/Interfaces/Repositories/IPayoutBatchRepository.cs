using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Settlements.Results;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Enums;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;

public interface IPayoutBatchRepository
{
    Task AddRangeAsync(IEnumerable<PayoutBatch> payoutBatches, CancellationToken cancellationToken = default);
    Task<(List<PayoutBatchAdminResponse> Items, int TotalCount)> GetPagedBatchesForAdminAsync(string? searchName, SettlementStatus? status, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<(List<PayoutBatchPartnerResponse> Items, int TotalCount)> GetPagedBatchesForPartnerAsync(Guid partnerId, SettlementStatus? status, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<PayoutBatchDetailResponse?> GetBatchDetailsAsync(Guid batchId, CancellationToken cancellationToken = default);
    Task<PayoutBatch?> GetByIdAsync(Guid batchId, CancellationToken cancellationToken = default);
}