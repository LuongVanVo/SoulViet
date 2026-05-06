using AutoMapper;
using MediatR;
using SoulViet.Modules.SoulMap.SoulMap.Application.DTOs;
using SoulViet.Modules.SoulMap.SoulMap.Application.Interfaces.Repositories;

namespace SoulViet.Modules.SoulMap.SoulMap.Application.Features.Map.Queries.GetTrendingPlaces;

public class GetTrendingPlacesHandler : IRequestHandler<GetTrendingPlacesQuery, List<MapMarkerDto>>
{
    private readonly IMapRepository _mapRepository;
    private readonly IMapper _mapper;
    public GetTrendingPlacesHandler(IMapRepository mapRepository, IMapper mapper)
    {
        _mapRepository = mapRepository;
        _mapper = mapper;
    }

    public async Task<List<MapMarkerDto>> Handle(GetTrendingPlacesQuery request, CancellationToken cancellationToken)
    {
        var results = new List<MapMarkerDto>();
        var mode = request.Mode.ToLower();

        if (mode == "tourist" || mode == "all")
        {
            var attractions =
                await _mapRepository.GetTrendingAttractionsAsync(request.ProvinceId, request.Take, cancellationToken);
            results.AddRange(_mapper.Map<List<MapMarkerDto>>(attractions));
        }

        if (mode == "accommodation" || mode == "all")
        {
            var accommodations =
                await _mapRepository.GetTrendingAccommodationsAsync(request.ProvinceId, request.Take, cancellationToken);
            results.AddRange(_mapper.Map<List<MapMarkerDto>>(accommodations));
        }

        return results.OrderByDescending(x => x.RatingScore).ToList();
    }
}