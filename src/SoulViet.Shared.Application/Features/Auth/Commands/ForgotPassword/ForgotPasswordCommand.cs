using MediatR;
using SoulViet.Shared.Application.Features.Auth.Results;

namespace SoulViet.Shared.Application.Features.Auth.Commands.ForgotPassword;

public class ForgotPasswordCommand : IRequest<AuthResponse>
{
    public string Email { get; set; } = string.Empty;
    public string Language { get; set; } = "vi";
}