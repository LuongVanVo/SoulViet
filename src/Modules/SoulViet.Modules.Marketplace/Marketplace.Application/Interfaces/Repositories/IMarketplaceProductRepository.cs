using SoulViet.Modules.Marketplace.Marketplace.Domain.Entities;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Interfaces.Repositories;

public interface IMarketplaceProductRepository
{
    // Add new product in Database
    Task AddAsync(MarketProduct product, CancellationToken cancellationToken = default);

    // Get product by id
    Task<MarketProduct?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    // Update product
    Task UpdateAsync(MarketProduct product, CancellationToken cancellationToken = default);

    // Verify product
    Task VerifyProductByIdAsync(MarketProduct product, CancellationToken cancellationToken = default);

    // Delete product (soft delete)
    Task DeleteAsync(MarketProduct product, CancellationToken cancellationToken = default);

    // Get all products of a partner
    Task<(IEnumerable<MarketProduct> Items, int TotalCount)> GetPagedByPartnerIdAsync(Guid partnerId, string? searchTerm, bool? isActive, Guid? categoryId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);

    // Get all product for admin
    Task<(IEnumerable<MarketProduct> Items, int TotalCount)> GetPagedForAdminAsync(string? searchTerm, bool? isVerified, bool? isActive, Guid? categoryId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);

    // Get all product for tourists
    Task<(IEnumerable<MarketProduct> Items, int TotalCount)> GetPublishedProductsAsync(
        string? searchTerm,
        Guid? categoryId,
        decimal? minPrice,
        decimal? maxPrice,
        Guid? provinceId,
        string? sortBy,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task<IEnumerable<MarketProduct>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);

}