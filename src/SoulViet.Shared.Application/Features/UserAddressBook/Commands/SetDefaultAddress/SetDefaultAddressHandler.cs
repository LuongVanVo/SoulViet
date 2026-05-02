using MediatR;
using MediatR.Pipeline;
using SoulViet.Shared.Application.Exceptions;
using SoulViet.Shared.Application.Interfaces.Repositories;

namespace SoulViet.Shared.Application.Features.UserAddressBook.Commands.SetDefaultAddress;

public class SetDefaultAddressHandler : IRequestHandler<SetDefaultAddressCommand, bool>
{
    private readonly IUserAddressRepository _userAddressRepository;
    public SetDefaultAddressHandler(IUserAddressRepository userAddressRepository)
    {
        _userAddressRepository = userAddressRepository;
    }

    public async Task<bool> Handle(SetDefaultAddressCommand request, CancellationToken cancellationToken)
    {
        var allAdresses = await _userAddressRepository.GetByUserIdAsync(request.UserId, cancellationToken);

        var targetAddress = allAdresses.FirstOrDefault(x => x.Id == request.Id);
        if (targetAddress == null)
            throw new NotFoundException("Address not found.");

        if (targetAddress.IsDefault) return true;

        foreach (var addr in allAdresses)
        {
            bool isTarget = (addr.Id == request.Id);
            if (addr.IsDefault != isTarget)
            {
                addr.IsDefault = isTarget;
                _userAddressRepository.Update(addr);
            }
        }

        await _userAddressRepository.SaveChangesAsync(cancellationToken);
        return true;
    }
}