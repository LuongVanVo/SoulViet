using Microsoft.EntityFrameworkCore;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;

namespace SoulViet.Modules.Marketplace.Marketplace.Infrastructure.Persistence.Repositories;

public class SoulCoinTransactionRepository : ISoulCoinTransactionRepository
{
    private readonly MarketplaceDbContext _dbContext;
    public SoulCoinTransactionRepository(MarketplaceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(SoulCoinTransaction transaction, CancellationToken cancellationToken = default)
    {
        await _dbContext.SoulCoinTransactions.AddAsync(transaction, cancellationToken);
    }

    public async Task<(List<SoulCoinTransaction> Items, int TotalCount)> GetByUserIdWithPaginationAsync(Guid userId, int pageNumber, int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.SoulCoinTransactions
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .AsQueryable();

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}