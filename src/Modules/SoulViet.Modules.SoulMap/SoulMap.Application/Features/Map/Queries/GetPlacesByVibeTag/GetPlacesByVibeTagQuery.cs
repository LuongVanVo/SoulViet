using MediatR;
using SoulViet.Modules.SoulMap.SoulMap.Application.DTOs;
using SoulViet.Shared.Domain.Enums;

namespace SoulViet.Modules.SoulMap.SoulMap.Application.Features.Map.Queries.GetPlacesByVibeTag;

public class GetPlacesByVibeTagQuery : IRequest<List<MapMarkerDto>>
{
    public VibeTag VibeTag { get; set; }
    public Guid? ProvinceId { get; set; }
    public string Mode { get; set; } = "all"; // "tourist", "accommodation", hoặc "all"
}