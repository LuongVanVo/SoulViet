using FluentValidation;

namespace SoulViet.Shared.Application.Features.Auth.Commands.Register;

public class RegisterValidator : AbstractValidator<RegisterCommand>
{
    public RegisterValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required.");
        RuleFor(x => x.Email)
            .NotEmpty().EmailAddress().WithMessage("Email is required and must be a valid email address.");
        RuleFor(x => x.Password)
            .NotEmpty().MinimumLength(8).WithMessage("Password is required and must be at least 8 characters long.");
        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password).WithMessage("Confirm password must match the password.");
        RuleFor(x => x.Language)
            .NotEmpty().WithMessage("Language is required.")
            .Must(lang => lang == "vi" || lang == "en").WithMessage("Language must be either 'vi' or 'en'.");
    }
}