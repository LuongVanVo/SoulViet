using AutoMapper;
using MediatR;
using SoulViet.Modules.SoulMap.SoulMap.Application.DTOs;
using SoulViet.Modules.SoulMap.SoulMap.Application.Exceptions;
using SoulViet.Modules.SoulMap.SoulMap.Application.Interfaces.Repositories;

namespace SoulViet.Modules.SoulMap.SoulMap.Application.Features.Map.Queries.GetAccommodationDetail;

public class GetAccommodationDetailHandler : IRequestHandler<GetAccommodationDetailQuery, AccommodationDetailDto>
{
    private readonly IMapRepository _mapRepository;
    private readonly IMapper _mapper;
    public GetAccommodationDetailHandler(IMapRepository mapRepository, IMapper mapper)
    {
        _mapRepository = mapRepository;
        _mapper = mapper;
    }

    public async Task<AccommodationDetailDto> Handle(GetAccommodationDetailQuery request, CancellationToken cancellationToken)
    {
        var place = await _mapRepository.GetAccommodationByIdAsync(request.Id, cancellationToken);
        if (place == null)
            throw new NotFoundException("Not found accommodation with the specified ID.");

        return _mapper.Map<AccommodationDetailDto>(place);
    }
}