using MediatR;
using SoulViet.Shared.Application.Features.UserAddressBook.Results;

namespace SoulViet.Shared.Application.Features.UserAddressBook.Commands.CreateUserAddress;

public class CreateUserAddressCommand : IRequest<CreateUserAddressResponse>
{
    public Guid UserId { get; set; }
    public string ReceiverName { get; set; } = string.Empty;
    public string ReceiverPhone { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;
    public string? District { get; set; } = string.Empty;
    public string Ward { get; set; } = string.Empty;
    public string DetailedAddress { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
}