using Microsoft.EntityFrameworkCore;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;

namespace SoulViet.Modules.Marketplace.Marketplace.Infrastructure.Persistence.Repositories;

public class MarketplaceProductRepository : IMarketplaceProductRepository
{
    private readonly MarketplaceDbContext _dbContext;
    public MarketplaceProductRepository(MarketplaceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(MarketProduct product, CancellationToken cancellationToken = default)
    {
        await _dbContext.MarketProducts.AddAsync(product, cancellationToken);
    }

    public async Task<MarketProduct?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.MarketProducts
            .Include(x => x.MarketplaceCategory)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<MarketProduct?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.MarketProducts
            .Include(x => x.MarketplaceCategory)
            .Include(x => x.Attributes)
            .Include(x => x.Variants)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task UpdateAsync(MarketProduct product, CancellationToken cancellationToken = default)
    {
        _dbContext.MarketProducts.Update(product);
        return Task.CompletedTask;
    }

    public async Task VerifyProductByIdAsync(MarketProduct product, CancellationToken cancellationToken = default)
    {
        product.IsVerifiedByAdmin = true;
        _dbContext.MarketProducts.Update(product);
    }

    public Task DeleteAsync(MarketProduct product, CancellationToken cancellationToken = default)
    {
        product.IsDeleted = true;
        product.IsActive = false;
        _dbContext.MarketProducts.Update(product);
        return Task.CompletedTask;
    }

    public async Task<(IEnumerable<MarketProduct> Items, int TotalCount)> GetPagedByPartnerIdAsync(Guid partnerId, string? searchTerm, bool? isActive, Guid? categoryId, int pageNumber, int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.MarketProducts
            .Include(x => x.MarketplaceCategory)
            .Where(x => x.PartnerId == partnerId)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(x => x.Name.ToLower().Contains(term) || (x.Description).ToLower().Contains(term));
        }

        if (isActive.HasValue)
        {
            query = query.Where(x => x.IsActive == isActive.Value);
        }

        if (categoryId.HasValue)
            query = query.Where(x => x.CategoryId == categoryId.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<(IEnumerable<MarketProduct> Items, int TotalCount)> GetPagedForAdminAsync(string? searchTerm, bool? isVerified, bool? isActive, Guid? categoryId, int pageNumber,
        int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.MarketProducts
            .Include(x => x.MarketplaceCategory)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(x => x.Name.ToLower().Contains(term) || (x.Description).ToLower().Contains(term));
        }

        if (isVerified.HasValue)
            query = query.Where(x => x.IsVerifiedByAdmin == isVerified.Value);

        if (isActive.HasValue)
            query = query.Where(x => x.IsActive == isActive.Value);

        if (categoryId.HasValue)
            query = query.Where(x => x.CategoryId == categoryId.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<(IEnumerable<MarketProduct> Items, int TotalCount)> GetPublishedProductsAsync(string? searchTerm, Guid? categoryId, decimal? minPrice, decimal? maxPrice,
        Guid? provinceId, string? sortBy, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.MarketProducts
            .Include(x => x.MarketplaceCategory)
            .Where(x => x.IsActive == true && x.IsVerifiedByAdmin == true && x.IsDeleted == false)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(x => x.Name.ToLower().Contains(term) || (x.Description).ToLower().Contains(term));
        }

        if (categoryId.HasValue)
            query = query.Where(x => x.CategoryId == categoryId.Value);

        if (provinceId.HasValue)
            query = query.Where(x => x.ProvinceId == provinceId.Value);

        if (minPrice.HasValue)
            query = query.Where(x => (x.PromotionalPrice ?? x.Price) >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(x => (x.PromotionalPrice ?? x.Price) <= maxPrice.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        // sorting
        query = sortBy?.ToLower() switch
        {
            "price_asc" => query.OrderBy(x => x.PromotionalPrice ?? x.Price),
            "price_desc" => query.OrderByDescending(x => x.PromotionalPrice ?? x.Price),
            "oldest" => query.OrderBy(x => x.CreatedAt),
            _ => query.OrderByDescending(x => x.CreatedAt)
        };

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<IEnumerable<MarketProduct>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        if (ids == null || !ids.Any())
        {
            return new List<MarketProduct>();
        }

        return await _dbContext.MarketProducts // Ensure this matches your DbSet name in MarketplaceDbContext
            .Include(p => p.Media)             // Include Media to avoid null reference on MainImage
            .Include(p => p.Attributes)
            .Include(p => p.Variants)
            .Where(p => ids.Contains(p.Id))
            .ToListAsync(cancellationToken);
    }
}