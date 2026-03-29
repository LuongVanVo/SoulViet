using MediatR;
using SoulViet.Shared.Application.Features.Auth.Results;

namespace SoulViet.Shared.Application.Features.Auth.Commands.Login;

public class LoginCommand : IRequest<AuthResponse>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public string? DeviceInfo { get; set; }
}