using MassTransit;
using MediatR;
using SoulViet.Shared.Application.Exceptions;
using SoulViet.Shared.Application.Features.UserAddressBook.Results;
using SoulViet.Shared.Application.Interfaces;
using SoulViet.Shared.Application.Interfaces.Repositories;
using SoulViet.Shared.Domain.Entities;

namespace SoulViet.Shared.Application.Features.UserAddressBook.Commands.CreateUserAddress;

public class CreateUserAddressHandler : IRequestHandler<CreateUserAddressCommand, CreateUserAddressResponse>
{
    private readonly IUserAddressRepository _userAddressRepository;
    private const int MaxAddressLimit = 5;
    public CreateUserAddressHandler(IUserAddressRepository userAddressRepository)
    {
        _userAddressRepository = userAddressRepository;
    }

    public async Task<CreateUserAddressResponse> Handle(CreateUserAddressCommand request, CancellationToken cancellationToken)
    {
        // Check if user has reached maxaddress limit
        var currentAddressCount = await _userAddressRepository.CountByUserIdAsync(request.UserId, cancellationToken);
        if (currentAddressCount >= MaxAddressLimit)
            throw new BadRequestException($"You only can have up to {MaxAddressLimit} addresses in your address book. Please delete an existing address before adding a new one.");

        // If is first address, set as default
        bool isDefault = request.IsDefault || currentAddressCount == 0;

        if (isDefault && currentAddressCount > 0)
        {
            var existingAddresses = await _userAddressRepository.GetByUserIdAsync(request.UserId, cancellationToken);
            foreach (var addr in existingAddresses)
            {
                addr.IsDefault = false;
                _userAddressRepository.Update(addr);
            }
        }

        var newAddress = new UserAddress
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            ReceiverName = request.ReceiverName,
            ReceiverPhone = request.ReceiverPhone,
            Province = request.Province,
            District = request.District,
            Ward = request.Ward,
            DetailedAddress = request.DetailedAddress,
            IsDefault = isDefault
        };

        await _userAddressRepository.AddAsync(newAddress, cancellationToken);

        await _userAddressRepository.SaveChangesAsync(cancellationToken);

        return new CreateUserAddressResponse
        {
            Success = true,
            Message = "Address created successfully.",
            Id = newAddress.Id
        };
    }
}