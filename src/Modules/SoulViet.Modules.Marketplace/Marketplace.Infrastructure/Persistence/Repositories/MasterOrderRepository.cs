using Microsoft.EntityFrameworkCore;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Enums;

namespace SoulViet.Modules.Marketplace.Marketplace.Infrastructure.Persistence.Repositories;

public class MasterOrderRepository : IMasterOrderRepository
{
    private readonly MarketplaceDbContext _dbContext;
    public MasterOrderRepository(MarketplaceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(MasterOrder masterOrder, CancellationToken cancellationToken = default)
    {
        await _dbContext.MasterOrders.AddAsync(masterOrder, cancellationToken);
    }

    public async Task<MasterOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.MasterOrders.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<MasterOrder?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.MasterOrders
            .Include(m => m.VendorOrders)
                .ThenInclude(o => o.OrderItems)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<(List<MasterOrder> Items, int TotalCount)> GetByUserIdWithPaginationAsync(
        Guid userId,
        int pageNumber,
        int pageSize,
        PaymentStatus? paymentStatus,
        PaymentMethod? paymentMethod,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.MasterOrders
            .Include(m => m.VendorOrders)
            .ThenInclude(o => o.OrderItems)
            .Where(m => m.UserId == userId)
            .AsQueryable();

        if (paymentStatus.HasValue)
        {
            query = query.Where(m => m.PaymentStatus == paymentStatus.Value);
        }

        if (paymentMethod.HasValue)
        {
            query = query.Where(m => m.PaymentMethod == paymentMethod.Value);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(m => m.CreatedAt >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(m => m.CreatedAt <= toDate.Value);
        }

        query = query.OrderByDescending(m => m.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}