using MediatR;
using SoulViet.Shared.Application.Features.UserAddressBook.Results;

namespace SoulViet.Shared.Application.Features.UserAddressBook.Commands.UpdateUserAddress;

public class UpdateUserAddressCommand : IRequest<UpdateUserAddressResponse>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public string? ReceiverName { get; set; }
    public string? ReceiverPhone { get; set; }
    public string? Province { get; set; }
    public string? District { get; set; }
    public string? Ward { get; set; }
    public string? DetailedAddress { get; set; }
    public bool? IsDefault { get; set; }
}