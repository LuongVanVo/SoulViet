using MediatR;
using SoulViet.Shared.Application.Features.Auth.Results;

namespace SoulViet.Shared.Application.Features.Auth.Commands.Logout;

public class LogoutCommand : IRequest<AuthResponse>
{
    public string RefreshToken { get; set; } = string.Empty;
}