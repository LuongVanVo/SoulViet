using AutoMapper;
using MediatR;
using NetTopologySuite.Geometries;
using SoulViet.Modules.SoulMap.SoulMap.Application.DTOs;
using SoulViet.Modules.SoulMap.SoulMap.Application.Interfaces.Repositories;

namespace SoulViet.Modules.SoulMap.SoulMap.Application.Features.Map.Queries.GetNearbyPlaces;

public class GetNearbyPlacesHandler : IRequestHandler<GetNearbyPlacesQuery, List<MapMarkerDto>>
{
    private readonly IMapRepository _mapRepository;
    private readonly IMapper _mapper;

    public GetNearbyPlacesHandler(IMapRepository mapRepository, IMapper mapper)
    {
        _mapRepository = mapRepository;
        _mapper = mapper;
    }

    public async Task<List<MapMarkerDto>> Handle(GetNearbyPlacesQuery request, CancellationToken cancellationToken)
    {
        var userLocation = new Point(request.Longitude, request.Latitude) { SRID = 4326 };

        var results = new List<MapMarkerDto>();
        var mode = request.Mode.ToLower();

        if (mode == "tourist" || mode == "all")
        {
            var attractions = await _mapRepository.GetNearbyAttractionsAsync(userLocation, request.RadiusInMeters, cancellationToken);
            results.AddRange(_mapper.Map<List<MapMarkerDto>>(attractions));
        }

        if (mode == "accommodation" || mode == "all")
        {
            var accommodations = await _mapRepository.GetNearbyAccommodationsAsync(userLocation, request.RadiusInMeters, cancellationToken);
            results.AddRange(_mapper.Map<List<MapMarkerDto>>(accommodations));
        }

        return results;
    }
}