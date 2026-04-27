using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using SoulViet.Modules.Social.Social.Application.Interfaces.Services;
using SoulViet.Modules.SoulMap.SoulMap.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoulViet.Modules.Social.Social.Infrastructure.Services
{
    public class SoulMapService : ISoulMapService
    {
        private readonly SoulMapDbContext _soulMapDbContext;

        private static readonly GeometryFactory GeometryFactory =
            new GeometryFactory(new PrecisionModel(), 4326);

        public SoulMapService(SoulMapDbContext soulMapDbContext)
        {
            _soulMapDbContext = soulMapDbContext;
        }

        public async Task<List<Guid>> GetNearbyLocationIdsAsync(double lat, double lon, double radiusKm, CancellationToken cancellationToken = default)
        {
            var userLocation = GeometryFactory.CreatePoint(new Coordinate(lon, lat));
            double radiusMeters = radiusKm * 1000;

            var attractionIds = _soulMapDbContext.TouristAttractions
                .Where(x => EF.Functions.IsWithinDistance(x.Location, userLocation, radiusMeters, true))
                .Select(x => x.Id);

            var accommodationIds = _soulMapDbContext.Accommodations
                .Where(x => EF.Functions.IsWithinDistance(x.Location, userLocation, radiusMeters, true))
                .Select(x => x.Id);

            return await attractionIds
                .Union(accommodationIds)
                .ToListAsync(cancellationToken);
        }
    }
}
