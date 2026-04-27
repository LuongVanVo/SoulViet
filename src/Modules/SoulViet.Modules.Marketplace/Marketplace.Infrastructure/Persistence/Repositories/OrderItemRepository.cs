using Microsoft.EntityFrameworkCore;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;

namespace SoulViet.Modules.Marketplace.Marketplace.Infrastructure.Persistence.Repositories;

public class OrderItemRepository : IOrderItemRepository
{
    private readonly MarketplaceDbContext _dbContext;
    public OrderItemRepository(MarketplaceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<OrderItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.OrderItems.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<OrderItem?> GetTicketByIdWithOrderAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.OrderItems
            .Include(x => x.Order)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<List<OrderItem>> GetItemsByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.OrderItems
            .Where(x => x.OrderId == orderId)
            .ToListAsync(cancellationToken);
    }

    public void Update(OrderItem orderItem)
    {
        _dbContext.OrderItems.Update(orderItem);
    }
}