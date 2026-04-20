using Microsoft.EntityFrameworkCore;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Enums;

namespace SoulViet.Modules.Marketplace.Marketplace.Infrastructure.Persistence.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly MarketplaceDbContext _dbContext;
    public OrderRepository(MarketplaceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Update(Order order)
    {
        _dbContext.Orders.Update(order);
    }

    public async Task<Order?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<(List<Order> Items, int TotalCount)> GetOrdersWithPaginationAsync(Guid? partnerId, OrderStatus? status, int pageNumber, int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Orders
            .Include(o => o.OrderItems)
            .AsQueryable();

        if (partnerId.HasValue)
            query = query.Where(o => o.PartnerId == partnerId.Value);

        if (status.HasValue)
            query = query.Where(o => o.Status == status.Value);

        query = query.OrderByDescending(o => o.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<(List<Order> Items, int TotalCount)> GetShopOrdersWithPaginationAsync(Guid partnerId, int pageNumber, int pageSize, OrderStatus? status,
        PaymentStatus? paymentStatus, PaymentMethod? paymentMethod, DateTime? fromDate, DateTime? toDate,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Orders
            .Include(o => o.OrderItems)
            .Include(o => o.MasterOrder)
            .Where(o => o.PartnerId == partnerId)
            .AsQueryable();

        // Filter by order status
        if (status.HasValue)
            query = query.Where(o => o.Status == status.Value);

        // Filter by payment status
        if (paymentStatus.HasValue)
            query = query.Where(o => o.MasterOrder != null && o.MasterOrder.PaymentStatus == paymentStatus.Value);

        if (paymentMethod.HasValue)
            query = query.Where(o => o.MasterOrder != null && o.MasterOrder.PaymentMethod == paymentMethod.Value);

        if (fromDate.HasValue)
        {
            query = query.Where(o => o.CreatedAt >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(o => o.CreatedAt <= toDate.Value);
        }

        query = query.OrderByDescending(o => o.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Orders.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task UpdateAsync(Order order, CancellationToken cancellationToken = default)
    {
        _dbContext.Orders.Update(order);
        return Task.CompletedTask;
    }
}