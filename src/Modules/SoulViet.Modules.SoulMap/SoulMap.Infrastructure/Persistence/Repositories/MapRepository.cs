using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using SoulViet.Modules.SoulMap.SoulMap.Application.Interfaces.Repositories;
using SoulViet.Modules.SoulMap.SoulMap.Domain.Entities;
using SoulViet.Shared.Domain.Enums;

namespace SoulViet.Modules.SoulMap.SoulMap.Infrastructure.Persistence.Repositories;

public class MapRepository : IMapRepository
{
    private readonly SoulMapDbContext _dbContext;
    public MapRepository(SoulMapDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private double MetersToDegrees(double meters)
    {
        return meters / 111320.0; // 1 degree = 111.32 km = 111320 meters
    }

    public async Task<List<TouristAttraction>> GetAttractionsByProvinceAsync(Guid provinceId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.TouristAttractions
            .Include(x => x.Category)
            .Where(x => x.ProvinceId == provinceId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Accommodation>> GetAccommodationsByProvinceAsync(Guid provinceId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Accommodations
            .Where(x => x.ProvinceId == provinceId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<TouristAttraction>> GetNearbyAttractionsAsync(Point userLocation, double radiusInMeters,
        CancellationToken cancellationToken = default)
    {
        double radiusInDegrees = MetersToDegrees(radiusInMeters);

        return await _dbContext.TouristAttractions
            .Include(x => x.Category)
            .Where(x => x.Location.IsWithinDistance(userLocation, radiusInDegrees))
            .OrderBy(x => x.Location.Distance(userLocation))
            .Take(50)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Accommodation>> GetNearbyAccommodationsAsync(Point userLocation, double radiusInMeters,
        CancellationToken cancellationToken = default)
    {
        double radiusInDegrees = MetersToDegrees(radiusInMeters);

        return await _dbContext.Accommodations
            .Where(x => x.Location.IsWithinDistance(userLocation, radiusInDegrees))
            .OrderBy(x => x.Location.Distance(userLocation))
            .Take(50)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<TouristAttraction>> GetTrendingAttractionsAsync(Guid provinceId, int take = 10, CancellationToken cancellationToken = default)
    {
        return await _dbContext.TouristAttractions
            .Include(x => x.Category)
            .Where(x => x.ProvinceId == provinceId && x.RatingScore > 0)
            .OrderByDescending(x => x.RatingScore)
            .ThenByDescending(x => x.ReviewCount)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Accommodation>> GetTrendingAccommodationsAsync(Guid provinceId, int take = 10, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Accommodations
            .Where(x => x.ProvinceId == provinceId && x.RatingScore > 0)
            .OrderByDescending(x => x.RatingScore)
            .ThenByDescending(x => x.ReviewCount)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<TouristAttraction>> GetAttractionsInBoundsAsync(Polygon boundingBox, CancellationToken cancellationToken = default)
    {
        return await _dbContext.TouristAttractions
            .Where(x => x.Location.CoveredBy(boundingBox))
            .Take(100) // Tránh lag nếu zoom out quá rộng
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Accommodation>> GetAccommodationsInBoundsAsync(Polygon boundingBox, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Accommodations
            .Where(x => x.Location.CoveredBy(boundingBox))
            .Take(100)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<TouristAttraction>> GetAttractionsByIdsAsync(List<Guid> attractionIds, CancellationToken cancellationToken = default)
    {
        return await _dbContext.TouristAttractions
            .Where(x => attractionIds.Contains(x.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Accommodation>> GetAccommodationsByIdsAsync(List<Guid> accommodationIds, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Accommodations
            .Where(x => accommodationIds.Contains(x.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<TouristAttraction?> GetAttractionByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.TouristAttractions
            .Include(x => x.Category)
            .Include(x => x.Province)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Accommodation?> GetAccommodationByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Accommodations
            .Include(x => x.Media)
            .Include(x => x.Province)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<List<TouristAttraction>> GetAttractionsByVibeTagAsync(VibeTag vibeTag, Guid? provinceId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.TouristAttractions.Include(x => x.Category).AsQueryable();

        if (provinceId.HasValue)
            query = query.Where(x => x.ProvinceId == provinceId.Value);

        return await query
            .Where(x => x.VibeTag == vibeTag)
            .OrderByDescending(x => x.RatingScore)
            .Take(50)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Accommodation>> GetAccommodationsByVibeTagAsync(VibeTag vibeTag, Guid? provinceId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Accommodations.AsQueryable();

        if (provinceId.HasValue)
            query = query.Where(x => x.ProvinceId == provinceId.Value);

        return await query
            .Where(x => x.VibeTag == vibeTag)
            .OrderByDescending(x => x.RatingScore)
            .Take(50)
            .ToListAsync(cancellationToken);
    }
}