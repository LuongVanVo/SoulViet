using FluentValidation;

namespace SoulViet.Shared.Application.Features.UserAddressBook.Commands.CreateUserAddress;

public class CreateUserAddressValidator : AbstractValidator<CreateUserAddressCommand>
{
    public CreateUserAddressValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.")
            .Must(x => x != Guid.Empty).WithMessage("UserId must be a valid GUID.");

        RuleFor(x => x.ReceiverName)
            .NotEmpty().WithMessage("ReceiverName is required.")
            .MaximumLength(150).WithMessage("ReceiverName must not exceed 150 characters.");

        RuleFor(x => x.ReceiverPhone)
            .NotEmpty().WithMessage("ReceiverPhone is required.")
            .Matches(@"^\+?[0-9\s\-]+$").WithMessage("ReceiverPhone must be a valid phone number format.")
            .WithMessage("ReceiverPhone must be a valid phone number format.")
            .MaximumLength(20).WithMessage("ReceiverPhone must not exceed 20 characters.");

        RuleFor(x => x.Province)
            .NotEmpty().WithMessage("Province/City is required.")
            .MaximumLength(100).WithMessage("Province/City must not exceed 100 characters.");

        RuleFor(x => x.District)
            .MaximumLength(100).WithMessage("District must not exceed 100 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.District));

        RuleFor(x => x.Ward)
            .NotEmpty().WithMessage("Ward is required.")
            .MaximumLength(100).WithMessage("Ward must not exceed 100 characters.");

        RuleFor(x => x.DetailedAddress)
            .NotEmpty().WithMessage("DetailedAddress is required.")
            .MaximumLength(500).WithMessage("DetailedAddress must not exceed 500 characters.");
    }
}