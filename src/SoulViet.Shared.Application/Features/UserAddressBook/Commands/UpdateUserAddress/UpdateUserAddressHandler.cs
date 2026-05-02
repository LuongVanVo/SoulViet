using MediatR;
using SoulViet.Shared.Application.Exceptions;
using SoulViet.Shared.Application.Features.UserAddressBook.Results;
using SoulViet.Shared.Application.Interfaces.Repositories;

namespace SoulViet.Shared.Application.Features.UserAddressBook.Commands.UpdateUserAddress;

public class UpdateUserAddressHandler : IRequestHandler<UpdateUserAddressCommand, UpdateUserAddressResponse>
{
    private readonly IUserAddressRepository _userAddressRepository;
    public UpdateUserAddressHandler(IUserAddressRepository userAddressRepository)
    {
        _userAddressRepository = userAddressRepository;
    }

    public async Task<UpdateUserAddressResponse> Handle(UpdateUserAddressCommand request, CancellationToken cancellationToken)
    {
        var address = await _userAddressRepository.GetByIdAsync(request.Id, cancellationToken);
        if (address == null || address.UserId != request.UserId)
            throw new NotFoundException("Address not found or does not belong to the user.");

        if (request.ReceiverName != null) address.ReceiverName = request.ReceiverName;
        if (request.ReceiverPhone != null) address.ReceiverPhone = request.ReceiverPhone;
        if (request.Province != null) address.Province = request.Province;

        if (request.District != null) address.District = request.District;

        if (request.Ward != null) address.Ward = request.Ward;
        if (request.DetailedAddress != null) address.DetailedAddress = request.DetailedAddress;

        if (request.IsDefault.HasValue)
        {
            if (request.IsDefault.Value && !address.IsDefault)
            {
                var otherAddresses = await _userAddressRepository.GetByUserIdAsync(request.UserId, cancellationToken);
                foreach (var addr in otherAddresses)
                {
                    addr.IsDefault = false;
                    _userAddressRepository.Update(addr);
                }
            }
            address.IsDefault = request.IsDefault.Value;
        }

        _userAddressRepository.Update(address);
        await _userAddressRepository.SaveChangesAsync(cancellationToken);

        return new UpdateUserAddressResponse
        {
            Success = true,
            Message = "User Address updated successfully.",
            Id = address.Id
        };
    }
}