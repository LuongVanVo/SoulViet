using MediatR;
using SoulViet.Shared.Application.DTOs;

namespace SoulViet.Shared.Application.Features.UserAddressBook.Queries.GetUserAddresses;

public class GetUserAddressesQuery : IRequest<List<UserAddressDto>>
{
    public Guid UserId { get; set; }
}