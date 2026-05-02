using MediatR;
using SoulViet.Shared.Application.Exceptions;
using SoulViet.Shared.Application.Interfaces.Repositories;

namespace SoulViet.Shared.Application.Features.UserAddressBook.Commands.DeleteUserAddress;

public class DeleteUserAddressHandler : IRequestHandler<DeleteUserAddressCommand, bool>
{
    private readonly IUserAddressRepository _userAddressRepository;
    public DeleteUserAddressHandler(IUserAddressRepository userAddressRepository)
    {
        _userAddressRepository = userAddressRepository;
    }

    public async Task<bool> Handle(DeleteUserAddressCommand request, CancellationToken cancellationToken)
    {
        var address = await _userAddressRepository.GetByIdAsync(request.Id, cancellationToken);
        if (address == null || address.UserId != request.UserId)
            throw new NotFoundException("User address not found or does not belong to the user.");

        bool wasDefault = address.IsDefault;

        _userAddressRepository.Remove(address);

        if (wasDefault)
        {
            var remainingAddresses = await _userAddressRepository.GetByUserIdAsync(request.UserId, cancellationToken);
            var nextDefault = remainingAddresses
                .Where(a => a.Id != request.Id)
                .OrderByDescending(a => a.CreatedAt)
                .FirstOrDefault();

            if (nextDefault != null)
            {
                nextDefault.IsDefault = true;
                _userAddressRepository.Update(nextDefault);
            }
        }

        await _userAddressRepository.SaveChangesAsync(cancellationToken);
        return true;
    }
}