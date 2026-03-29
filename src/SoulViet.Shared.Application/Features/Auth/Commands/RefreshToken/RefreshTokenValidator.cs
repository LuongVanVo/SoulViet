using FluentValidation;

namespace SoulViet.Shared.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenValidator : AbstractValidator<RefreshTokenCommand>
{


    public RefreshTokenValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required.")
            .Must(BeAValidBase64String).WithMessage("Invalid refresh token format.");
    }

    private bool BeAValidBase64String(string token)
    {
        if (string.IsNullOrWhiteSpace(token) || token.Length % 4 != 0)
            return false;

        Span<byte> buffer = new Span<byte>(new byte[token.Length]);
        return Convert.TryFromBase64String(token, buffer, out _);
    }
}