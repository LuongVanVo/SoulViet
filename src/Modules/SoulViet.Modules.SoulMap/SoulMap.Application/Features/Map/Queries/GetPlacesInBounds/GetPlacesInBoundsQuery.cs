using MediatR;
using SoulViet.Modules.SoulMap.SoulMap.Application.DTOs;

namespace SoulViet.Modules.SoulMap.SoulMap.Application.Features.Map.Queries.GetPlacesInBounds;

public class GetPlacesInBoundsQuery : IRequest<List<MapMarkerDto>>
{
    public double MinLat { get; set; } // South
    public double MaxLat { get; set; } // North
    public double MinLng { get; set; } // West
    public double MaxLng { get; set; } // East
    public string Mode { get; set; } = "all";
}