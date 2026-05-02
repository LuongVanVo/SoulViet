using System.Data;
using FluentValidation;

namespace SoulViet.Shared.Application.Features.UserAddressBook.Commands.DeleteUserAddress;

public class DeleteUserAddressValidator : AbstractValidator<DeleteUserAddressCommand>
{
    public DeleteUserAddressValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Address ID is required.")
            .Must(id => id != Guid.Empty).WithMessage("Address ID must be a valid GUID.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.")
            .Must(id => id != Guid.Empty).WithMessage("User ID must be a valid GUID.");
    }
}