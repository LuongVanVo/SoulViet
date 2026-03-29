using MediatR;
using SoulViet.Shared.Application.Features.Auth.Results;

namespace SoulViet.Shared.Application.Features.Auth.Commands.Register;

public class RegisterCommand : IRequest<AuthResponse>
{
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public string Language { get; set; } = "vi";

}