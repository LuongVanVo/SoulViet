using Microsoft.EntityFrameworkCore;
using SoulViet.Modules.Marketplace.Marketplace.Application.Features.Settlements.Results;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Enums;

namespace SoulViet.Modules.Marketplace.Marketplace.Infrastructure.Persistence.Repositories;

public class PayoutBatchRepository : IPayoutBatchRepository
{
    private readonly MarketplaceDbContext _dbContext;
    public PayoutBatchRepository(MarketplaceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddRangeAsync(IEnumerable<PayoutBatch> payoutBatches, CancellationToken cancellationToken = default)
    {
        await _dbContext.PayoutBatches.AddRangeAsync(payoutBatches, cancellationToken);
    }

    public async Task<(List<PayoutBatchAdminResponse> Items, int TotalCount)> GetPagedBatchesForAdminAsync(string? searchName, SettlementStatus? status, int pageNumber, int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.PayoutBatches.AsQueryable();

        if (status.HasValue)
            query = query.Where(x => x.Status == status.Value);

        if (!string.IsNullOrWhiteSpace(searchName))
        {
            query = query.Where(x => x.PartnerNameSnapshot.ToLower().Contains(searchName.ToLower()));
        }

        int totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new PayoutBatchAdminResponse
            {
                Id = x.Id,
                PartnerId = x.PartnerId,
                PartnerName = x.PartnerNameSnapshot,
                PeriodStart = x.PeriodStart,
                PeriodEnd = x.PeriodEnd,
                TotalSales = x.TotalSales,
                TotalCommission = x.TotalCommission,
                NetPayout = x.NetPayout,
                Status = x.Status.ToString(),
                CreatedAt = x.CreatedAt
            }).ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<(List<PayoutBatchPartnerResponse> Items, int TotalCount)> GetPagedBatchesForPartnerAsync(Guid partnerId, SettlementStatus? status, int pageNumber, int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.PayoutBatches.Where(x => x.PartnerId == partnerId).AsQueryable();

        if (status.HasValue)
            query = query.Where(x => x.Status == status.Value);

        int totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new PayoutBatchPartnerResponse
            {
                Id = x.Id,
                PeriodStart = x.PeriodStart,
                PeriodEnd = x.PeriodEnd,
                TotalSales = x.TotalSales,
                NetPayout = x.NetPayout,
                Status = x.Status.ToString(),
                CreatedAt = x.CreatedAt
            }).ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<PayoutBatchDetailResponse?> GetBatchDetailsAsync(Guid batchId, CancellationToken cancellationToken = default)
    {
        var batch = await _dbContext.PayoutBatches
            .FirstOrDefaultAsync(x => x.Id == batchId, cancellationToken);

        if (batch == null) return null;

        var items = await (from oi in _dbContext.OrderItems
            join o in _dbContext.Orders on oi.OrderId equals o.Id
            where oi.PayoutBatchId == batchId
            select new SettledItemDetail
            {
                OrderItemId = oi.Id,
                CustomerId = o.UserId,
                ProductName = oi.ProductNameSnapshot,
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice,
                PartnerEarnings = oi.PartnerEarnings,
                OrderedAt = oi.CreatedAt
            }).ToListAsync(cancellationToken);

        return new PayoutBatchDetailResponse
        {
            BatchId = batch.Id,
            Status = batch.Status.ToString(),
            TotalNetPayout = batch.NetPayout,
            Items = items
        };
    }

    public async Task<PayoutBatch?> GetByIdAsync(Guid batchId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.PayoutBatches
            .FirstOrDefaultAsync(x => x.Id == batchId, cancellationToken);
    }
}