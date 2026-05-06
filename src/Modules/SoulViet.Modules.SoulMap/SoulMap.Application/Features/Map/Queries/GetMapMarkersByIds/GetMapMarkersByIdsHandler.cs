using AutoMapper;
using MediatR;
using SoulViet.Modules.SoulMap.SoulMap.Application.DTOs;
using SoulViet.Modules.SoulMap.SoulMap.Application.Interfaces.Repositories;

namespace SoulViet.Modules.SoulMap.SoulMap.Application.Features.Map.Queries.GetMapMarkersByIds;

public class GetMapMarkersByIdsHandler : IRequestHandler<GetMapMarkersByIdsQuery, List<MapMarkerDto>>
{
    private readonly IMapRepository _mapRepository;
    private readonly IMapper _mapper;
    public GetMapMarkersByIdsHandler(IMapRepository mapRepository, IMapper mapper)
    {
        _mapRepository = mapRepository;
        _mapper = mapper;
    }

    public async Task<List<MapMarkerDto>> Handle(GetMapMarkersByIdsQuery request, CancellationToken cancellationToken)
    {
        var results = new List<MapMarkerDto>();

        var attractions = await _mapRepository.GetAttractionsByIdsAsync(request.PlaceIds, cancellationToken);
        results.AddRange(_mapper.Map<List<MapMarkerDto>>(attractions));

        var accommodations = await _mapRepository.GetAccommodationsByIdsAsync(request.PlaceIds, cancellationToken);
        results.AddRange(_mapper.Map<List<MapMarkerDto>>(accommodations));

        return results.OrderBy(x => request.PlaceIds.IndexOf(x.Id)).ToList();
    }
}