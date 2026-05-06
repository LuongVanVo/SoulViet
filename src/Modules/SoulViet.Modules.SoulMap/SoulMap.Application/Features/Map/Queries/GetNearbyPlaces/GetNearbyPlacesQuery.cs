using MediatR;
using SoulViet.Modules.SoulMap.SoulMap.Application.DTOs;

namespace SoulViet.Modules.SoulMap.SoulMap.Application.Features.Map.Queries.GetNearbyPlaces;

public class GetNearbyPlacesQuery : IRequest<List<MapMarkerDto>>
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double RadiusInMeters { get; set; } = 5000; // Default to 5km radius

    // Mode: "tourist", "accommodation", or "all"
    public string Mode { get; set; } = "all";
}