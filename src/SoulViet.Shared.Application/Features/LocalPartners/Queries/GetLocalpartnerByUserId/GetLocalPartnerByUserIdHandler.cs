using AutoMapper;
using MediatR;
using SoulViet.Shared.Application.DTOs;
using SoulViet.Shared.Application.Exceptions;
using SoulViet.Shared.Application.Interfaces.Repositories;

namespace SoulViet.Shared.Application.Features.LocalPartners.Queries.GetLocalpartnerByUserId;

public class GetLocalPartnerByUserIdHandler : IRequestHandler<GetLocalPartnerByUserIdQuery, LocalPartnerInfoDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    public GetLocalPartnerByUserIdHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<LocalPartnerInfoDto> Handle(GetLocalPartnerByUserIdQuery request, CancellationToken cancellationToken)
    {
        var localPartner = await _userRepository.GetLocalPartnerInfoByUserIdAsync(request.UserId);
        if (localPartner == null)
            throw new NotFoundException("Local partner not found for the given user ID.");

        return localPartner;
    }
}