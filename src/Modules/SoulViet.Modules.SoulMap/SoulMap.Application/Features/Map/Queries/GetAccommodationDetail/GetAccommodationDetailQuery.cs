using MediatR;
using SoulViet.Modules.SoulMap.SoulMap.Application.DTOs;

namespace SoulViet.Modules.SoulMap.SoulMap.Application.Features.Map.Queries.GetAccommodationDetail;

public class GetAccommodationDetailQuery : IRequest<AccommodationDetailDto>
{
    public Guid Id { get; set; }
}