using MediatR;

namespace SoulViet.Shared.Application.Features.UserAddressBook.Commands.DeleteUserAddress;

public class DeleteUserAddressCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
}