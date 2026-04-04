using SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;

public interface IMarketplaceCategoryRepository
{
    // Kiểm tra xem Slug đã tồn tại hay chưa
    Task<bool> IsSlugExistsAsync(string slug, CancellationToken cancellationToken = default);

    // Thêm mới danh mục
    Task AddAsync(MarketplaceCategory category, CancellationToken cancellationToken = default);

    // Get all categories
    Task<IEnumerable<MarketplaceCategory>> GetAllActiveAsync(CancellationToken cancellationToken = default);

    // Get category by id
    Task<MarketplaceCategory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    // Update category
    Task UpdateAsync(MarketplaceCategory category, CancellationToken cancellationToken = default);

    // Soft delete category
    Task SoftDeleteAsync(MarketplaceCategory category, CancellationToken cancellationToken = default);
}