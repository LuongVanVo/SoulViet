using NetTopologySuite.Geometries;
using SoulViet.Modules.SoulMap.SoulMap.Domain.Entities;
using SoulViet.Shared.Domain.Enums;

namespace SoulViet.Modules.SoulMap.SoulMap.Application.Interfaces.Repositories;

public interface IMapRepository
{
    // Lấy tất cả điểm theo Province
    Task<List<TouristAttraction>> GetAttractionsByProvinceAsync(Guid provinceId, CancellationToken cancellationToken = default);
    Task<List<Accommodation>> GetAccommodationsByProvinceAsync(Guid provinceId, CancellationToken cancellationToken = default);

    // Tìm kiếm điểm gần đây dựa trên vị trí người dùng và bán kính
    Task<List<TouristAttraction>> GetNearbyAttractionsAsync(Point userLocation, double radiusInMeters, CancellationToken cancellationToken = default);
    Task<List<Accommodation>> GetNearbyAccommodationsAsync(Point userLocation, double radiusInMeters, CancellationToken cancellationToken = default);

    // Lấy trending
    Task<List<TouristAttraction>> GetTrendingAttractionsAsync(Guid provinceId, int take = 10, CancellationToken cancellationToken = default);
    Task<List<Accommodation>> GetTrendingAccommodationsAsync(Guid provinceId, int take = 10, CancellationToken cancellationToken = default);

    // Lấy theo inbound
    Task<List<TouristAttraction>> GetAttractionsInBoundsAsync(Polygon boundingBox, CancellationToken cancellationToken = default);
    Task<List<Accommodation>> GetAccommodationsInBoundsAsync(Polygon boundingBox, CancellationToken cancellationToken = default);

    // Lấy các điểm từ danh sách ID (do AI trả về)
    Task<List<TouristAttraction>> GetAttractionsByIdsAsync(List<Guid> attractionIds, CancellationToken cancellationToken = default);
    Task<List<Accommodation>> GetAccommodationsByIdsAsync(List<Guid> accommodationIds, CancellationToken cancellationToken = default);

    // Lấy chi tiết một điểm
    Task<TouristAttraction?> GetAttractionByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Accommodation?> GetAccommodationByIdAsync(Guid id, CancellationToken cancellationToken = default);

    // Lấy các điểm theo vibe tag
    Task<List<TouristAttraction>> GetAttractionsByVibeTagAsync(VibeTag vibeTag, Guid? provinceId = null, CancellationToken cancellationToken = default);
    Task<List<Accommodation>> GetAccommodationsByVibeTagAsync(VibeTag vibeTag, Guid? provinceId = null, CancellationToken cancellationToken = default);
}