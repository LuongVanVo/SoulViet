using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SoulViet.Modules.Marketplace.Marketplace.Application.DTOs;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Orders.Common;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Enums;
using StackExchange.Redis;

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

    public async Task<List<OrderItem>> GetUnsettledItemsAsync(DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _dbContext.OrderItems
            .Include(x => x.Order)
            .Where(x => x.Order.Status == OrderStatus.Delivered && x.IsSettled == false && x.CreatedAt <= endDate)
            .ToListAsync(cancellationToken);
    }

    public void UpdateRange(IEnumerable<OrderItem> orderItems)
    {
        _dbContext.OrderItems.UpdateRange(orderItems);
    }

    private static DateTime? TryExtractServiceDate(string? metadata)
    {
        if (string.IsNullOrWhiteSpace(metadata)) return null;
        try
        {
            using var doc = JsonDocument.Parse(metadata);
            var root = doc.RootElement;

            string[] keys = ["serviceDate", "travelDate", "bookingDate", "date"];
            foreach (var key in keys)
            {
                if (root.TryGetProperty(key, out var p))
                {
                    if (p.ValueKind == JsonValueKind.String && DateTime.TryParse(p.GetString(), out var dt))
                        return dt;
                }
            }
        } catch { }
        return null;
    }

    private static DateTime? ResolveExpiredAt(DateTime? serviceDate)
    {
        if (!serviceDate.HasValue) return null;
        return serviceDate.Value.Date.AddDays(1).AddTicks(-1); // End of the service date
    }

    public async Task<(List<MyTicketDto> Items, int TotalCount)> GetUserTicketsWithPaginationAsync(Guid userId, TicketStatusFilter status, int pageNumber, int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.OrderItems
            .Include(oi => oi.Order)
            .ThenInclude(o => o.MasterOrder)
            .Where(oi =>
                oi.Order.UserId == userId &&
                (oi.ProductTypeSnapshot == ProductType.WorkshopTicket) && !string.IsNullOrEmpty(oi.TicketCode))
            .AsQueryable();

        var raw = await query.OrderByDescending(x => x.CreatedAt).ToListAsync(cancellationToken);

        var mapped = raw.Select(oi =>
        {
            var serviceDate = TryExtractServiceDate(oi.ItemMetadata);
            var expiredAt = ResolveExpiredAt(serviceDate);
            var isExpired = !oi.IsTicketUsed && expiredAt.HasValue && DateTime.UtcNow > expiredAt.Value;

            return new MyTicketDto
            {
                OrderItemId = oi.Id,
                OrderId = oi.OrderId,
                MasterOrderId = oi.Order.MasterOrderId,
                ProductId = oi.ProductId,
                ProductName = oi.ProductNameSnapshot,
                ProductImage = oi.ProductImageSnapshot,
                VariantName = oi.VariantNameSnapshot,
                TicketCode = oi.TicketCode!,
                TicketQrUrl = oi.TicketQRUrl,
                IsUsed = oi.IsTicketUsed,
                TicketUsedDate = oi.TicketUsedDate,
                ServiceDate = serviceDate,
                ExpiredAt = expiredAt,
                IsExpired = isExpired,
                PartnerName = oi.Order.PartnerId.ToString(),
                CreatedDate = oi.CreatedAt
            };
        });

        mapped = status switch
        {
            TicketStatusFilter.Active => mapped.Where(t => !t.IsUsed && !t.IsExpired),
            TicketStatusFilter.Used => mapped.Where(t => t.IsUsed),
            TicketStatusFilter.Expired => mapped.Where(x => !x.IsUsed && x.IsExpired),
            _ => mapped
        };

        var total = mapped.Count();
        var items = mapped
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return (items, total);
    }

    public async Task<MyTicketDetailDto?> GetUserTicketDetailAsync(Guid userId, Guid orderItemId, CancellationToken cancellationToken = default)
    {
        var oi = await _dbContext.OrderItems
            .Include(x => x.Order)
            .ThenInclude(o => o.MasterOrder)
            .FirstOrDefaultAsync(x => x.Id == orderItemId && x.Order.UserId == userId, cancellationToken);

        if (oi == null) return null;
        if (string.IsNullOrWhiteSpace(oi.TicketCode)) return null;

        var serviceDate = TryExtractServiceDate(oi.ItemMetadata);
        var expiredAt = ResolveExpiredAt(serviceDate);
        var isExpired = !oi.IsTicketUsed && expiredAt.HasValue && DateTime.UtcNow > expiredAt.Value;

        return new MyTicketDetailDto
        {
            OrderItemId = oi.Id,
            OrderId = oi.OrderId,
            MasterOrderId = oi.Order.MasterOrderId,
            ProductId = oi.ProductId,
            ProductName = oi.ProductNameSnapshot,
            ProductImage = oi.ProductImageSnapshot,
            VariantName = oi.VariantNameSnapshot,
            TicketCode = oi.TicketCode!,
            TicketQrUrl = oi.TicketQRUrl,
            IsUsed = oi.IsTicketUsed,
            TicketUsedDate = oi.TicketUsedDate,
            ServiceDate = serviceDate,
            ExpiredAt = expiredAt,
            IsExpired = isExpired,
            ItemMetadata = oi.ItemMetadata,
            PartnerName = oi.Order.PartnerId.ToString(),
            CreatedDate = oi.CreatedAt
        };
    }
}