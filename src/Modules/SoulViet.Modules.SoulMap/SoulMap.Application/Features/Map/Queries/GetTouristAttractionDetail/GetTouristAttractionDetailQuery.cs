using MediatR;
using SoulViet.Modules.SoulMap.SoulMap.Application.DTOs;

namespace SoulViet.Modules.SoulMap.SoulMap.Application.Features.Map.Queries.GetTouristAttractionDetail;

public class GetTouristAttractionDetailQuery : IRequest<TouristAttractionDetailDto>
{
    public Guid Id { get; set; }
}