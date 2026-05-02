using MediatR;

namespace SoulViet.Shared.Application.Features.UserAddressBook.Commands.SetDefaultAddress;

public class SetDefaultAddressCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
}