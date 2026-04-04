using Microsoft.EntityFrameworkCore;
using SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;
using SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;

namespace SoulViet.Modules.Marketplace.Marketplace.Infrastructure.Persistence.Repositories;

public class MarketplaceCategoryRepository : IMarketplaceCategoryRepository
{
    private readonly MarketplaceDbContext _dbContext;
    public MarketplaceCategoryRepository(MarketplaceDbContext dbContext)
    {
        _dbContext = (MarketplaceDbContext)dbContext;
    }

    public async Task<bool> IsSlugExistsAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await _dbContext.MarketplaceCategories
            .AnyAsync(x => x.Slug == slug, cancellationToken);
    }

    public async Task AddAsync(MarketplaceCategory category, CancellationToken cancellationToken = default)
    {
        await _dbContext.MarketplaceCategories.AddAsync(category, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<MarketplaceCategory>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.MarketplaceCategories
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.CategoryType).ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<MarketplaceCategory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.MarketplaceCategories
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task UpdateAsync(MarketplaceCategory category, CancellationToken cancellationToken = default)
    {
        _dbContext.MarketplaceCategories.Update(category);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SoftDeleteAsync(MarketplaceCategory category, CancellationToken cancellationToken = default)
    {
        category.IsActive = false;
        _dbContext.MarketplaceCategories.Update(category);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}