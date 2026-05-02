using AutoMapper;
using MediatR;
using SoulViet.Shared.Application.DTOs;
using SoulViet.Shared.Application.Interfaces.Repositories;

namespace SoulViet.Shared.Application.Features.UserAddressBook.Queries.GetUserAddresses;

public class GetUserAddressesHandler : IRequestHandler<GetUserAddressesQuery, List<UserAddressDto>>
{
    private readonly IUserAddressRepository _userAddressRepository;
    private readonly IMapper _mapper;
    public GetUserAddressesHandler(IUserAddressRepository userAddressRepository, IMapper mapper)
    {
        _userAddressRepository = userAddressRepository;
        _mapper = mapper;
    }

    public async Task<List<UserAddressDto>> Handle(GetUserAddressesQuery request, CancellationToken cancellationToken)
    {
        var addresses = await _userAddressRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        return _mapper.Map<List<UserAddressDto>>(addresses);
    }
}