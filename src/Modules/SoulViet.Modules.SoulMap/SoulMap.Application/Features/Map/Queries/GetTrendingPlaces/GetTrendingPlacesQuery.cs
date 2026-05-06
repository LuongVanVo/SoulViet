using MediatR;
using SoulViet.Modules.SoulMap.SoulMap.Application.DTOs;

namespace SoulViet.Modules.SoulMap.SoulMap.Application.Features.Map.Queries.GetTrendingPlaces;

public class GetTrendingPlacesQuery : IRequest<List<MapMarkerDto>>
{
    public Guid ProvinceId { get; set; }
    public int Take { get; set; } = 20;
    public string Mode { get; set; } = "all";
}