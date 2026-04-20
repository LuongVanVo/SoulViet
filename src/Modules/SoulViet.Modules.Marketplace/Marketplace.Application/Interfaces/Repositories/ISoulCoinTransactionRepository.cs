using SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;

public interface ISoulCoinTransactionRepository
{
    Task AddAsync(SoulCoinTransaction transaction, CancellationToken cancellationToken = default);
    Task<(List<SoulCoinTransaction> Items, int TotalCount)> GetByUserIdWithPaginationAsync(
        Guid userId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default
    );
}