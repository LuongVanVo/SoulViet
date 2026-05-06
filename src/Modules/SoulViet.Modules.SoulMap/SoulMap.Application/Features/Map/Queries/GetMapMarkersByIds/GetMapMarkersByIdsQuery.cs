using MediatR;
using SoulViet.Modules.SoulMap.SoulMap.Application.DTOs;

namespace SoulViet.Modules.SoulMap.SoulMap.Application.Features.Map.Queries.GetMapMarkersByIds;

public class GetMapMarkersByIdsQuery : IRequest<List<MapMarkerDto>>
{
    public List<Guid> PlaceIds { get; set; } = new();
}