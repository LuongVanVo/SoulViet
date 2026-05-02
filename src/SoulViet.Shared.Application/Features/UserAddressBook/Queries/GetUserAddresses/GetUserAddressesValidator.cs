using FluentValidation;

namespace SoulViet.Shared.Application.Features.UserAddressBook.Queries.GetUserAddresses;

public class GetUserAddressesValidator : AbstractValidator<GetUserAddressesQuery>
{
    public GetUserAddressesValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.")
            .Must(id => id != Guid.Empty).WithMessage("User ID must be a valid GUID.");
    }
}