using AutoMapper;
using MediatR;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Utilities;
using SoulViet.Modules.SoulMap.SoulMap.Application.DTOs;
using SoulViet.Modules.SoulMap.SoulMap.Application.Interfaces.Repositories;

namespace SoulViet.Modules.SoulMap.SoulMap.Application.Features.Map.Queries.GetPlacesInBounds;

public class GetPlacesInBoundsHandler : IRequestHandler<GetPlacesInBoundsQuery, List<MapMarkerDto>>
{
    private readonly IMapRepository _mapRepository;
    private readonly IMapper _mapper;
    public GetPlacesInBoundsHandler(IMapRepository mapRepository, IMapper mapper)
    {
        _mapRepository = mapRepository;
        _mapper = mapper;
    }

    public async Task<List<MapMarkerDto>> Handle(GetPlacesInBoundsQuery request, CancellationToken cancellationToken)
    {
        var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        var envelope = new Envelope(request.MinLng, request.MaxLng, request.MinLat, request.MaxLat);
        var boundingBox = geometryFactory.ToGeometry(envelope) as Polygon;

        var results = new List<MapMarkerDto>();
        var mode = request.Mode.ToLower();

        if (mode == "tourist" || mode == "all")
        {
            var attractions = await _mapRepository.GetAttractionsInBoundsAsync(boundingBox, cancellationToken);
            results.AddRange(_mapper.Map<List<MapMarkerDto>>(attractions));
        }

        if (mode == "accommodation" || mode == "all")
        {
            var accommodations = await _mapRepository.GetAccommodationsInBoundsAsync(boundingBox, cancellationToken);
            results.AddRange(_mapper.Map<List<MapMarkerDto>>(accommodations));
        }

        return results;
    }
}