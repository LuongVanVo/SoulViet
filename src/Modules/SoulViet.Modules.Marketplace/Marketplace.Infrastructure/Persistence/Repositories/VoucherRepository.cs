using Microsoft.EntityFrameworkCore;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Enums;

namespace SoulViet.Modules.Marketplace.Marketplace.Infrastructure.Persistence.Repositories;

public class VoucherRepository : IVoucherRepository
{
    private readonly MarketplaceDbContext _dbContext;
    public VoucherRepository(MarketplaceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Voucher?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Vouchers.FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
    }

    public async Task<Voucher?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Vouchers.FirstOrDefaultAsync(v => v.Code.ToUpper() == code.ToUpper(), cancellationToken);
    }

    public async Task<Voucher?> GetByCodeAsync(string code, Guid? partnerId, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Vouchers.Where(v => v.Code.ToUpper() == code.ToUpper());

        if (partnerId.HasValue)
        {
            query = query.Where(v => v.Scope == VoucherScope.Shop && v.PartnerId == partnerId.Value);
        }
        else
        {
            query = query.Where(v => v.Scope == VoucherScope.Platform);
        }

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task AddAsync(Voucher voucher, CancellationToken cancellationToken = default)
    {
        await _dbContext.Vouchers.AddAsync(voucher, cancellationToken);
    }

    public void Update(Voucher voucher)
    {
        _dbContext.Vouchers.Update(voucher);
    }

    public async Task<Voucher?> GetValidVoucherAsync(string code, Guid? partnerId, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        var query = _dbContext.Vouchers.Where(v =>
            v.Code.ToUpper() == code.ToUpper() &&
            v.IsActive &&
            v.UsedCount < v.UsageLimit &&
            v.StartDate <= now &&
            v.EndDate >= now
        );

        if (partnerId.HasValue)
        {
            query = query.Where(v => v.Scope == VoucherScope.Shop && v.PartnerId == partnerId.Value);
        }
        else
        {
            // Nếu không truyền partnerId, chỉ lấy mã chung cho toàn sàn
            query = query.Where(v => v.Scope == VoucherScope.Platform);
        }

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<Voucher>> GetAvailableVouchersAsync(Guid? partnerId, decimal currentOrderAmount,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        Console.WriteLine($"Timezone of now: {TimeZoneInfo.Local.DisplayName}, now: {now}");

        var query = _dbContext.Vouchers.Where(v =>
            v.IsActive &&
            v.UsedCount < v.UsageLimit &&
            v.StartDate <= now &&
            v.EndDate >= now &&
            v.MinOrderAmount <= currentOrderAmount
        );

        if (partnerId.HasValue)
        {
            query = query.Where(v => v.Scope == VoucherScope.Shop && v.PartnerId == partnerId.Value);
        }
        else
        {
            query = query.Where(v => v.Scope == VoucherScope.Platform);
        }

        return await query.OrderByDescending(v => v.DiscountValue).ToListAsync(cancellationToken);
    }

    public async Task<(List<Voucher> Items, int TotalCount)> GetVouchersWithPaginationAsync(Guid? partnerId, int pageNumber, int pageSize, string? searchTerm, bool? isActive,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Vouchers.AsQueryable();

        if (partnerId.HasValue)
        {
            query = query.Where(v => v.PartnerId == partnerId.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.Trim().ToUpper();
            query = query.Where(v => v.Code.ToUpper().Contains(searchTerm));
        }

        if (isActive.HasValue)
        {
            query = query.Where(v => v.IsActive == isActive.Value);
        }

        var totalCount = query.Count();

        var items = await query
            .OrderByDescending(v => v.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}