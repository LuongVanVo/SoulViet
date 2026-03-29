using FluentValidation;
using SoulViet.Shared.Application.Features.Auth.Results;

namespace SoulViet.Shared.Application.Features.Auth.Commands.Login;

public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().EmailAddress().WithMessage("Email is required and must be a valid email address.");
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required and must be at least 8 characters long.");
    }
}