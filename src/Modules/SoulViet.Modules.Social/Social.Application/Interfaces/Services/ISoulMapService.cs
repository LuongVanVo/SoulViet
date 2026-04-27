using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SoulViet.Modules.Social.Social.Application.Interfaces.Services
{
    public interface ISoulMapService
    {
        Task<List<Guid>> GetNearbyLocationIdsAsync(double lat, double lon, double radiusKm, CancellationToken cancellationToken = default);
    }
}
