using AutoMapper;
using MediatR;
using SoulViet.Modules.SoulMap.SoulMap.Application.DTOs;
using SoulViet.Modules.SoulMap.SoulMap.Application.Exceptions;
using SoulViet.Modules.SoulMap.SoulMap.Application.Interfaces.Repositories;

namespace SoulViet.Modules.SoulMap.SoulMap.Application.Features.Map.Queries.GetTouristAttractionDetail;

public class GetTouristAttractionDetailHandler : IRequestHandler<GetTouristAttractionDetailQuery, TouristAttractionDetailDto>
{
    private readonly IMapRepository _mapRepository;
    private readonly IMapper _mapper;
    public GetTouristAttractionDetailHandler(IMapRepository mapRepository, IMapper mapper)
    {
        _mapRepository = mapRepository;
        _mapper = mapper;
    }

    public async Task<TouristAttractionDetailDto> Handle(GetTouristAttractionDetailQuery request, CancellationToken cancellationToken)
    {
        var place = await _mapRepository.GetAttractionByIdAsync(request.Id, cancellationToken);
        if (place == null)
            throw new NotFoundException("Not found tourist attraction with the specified ID.");

        return _mapper.Map<TouristAttractionDetailDto>(place);
    }
}