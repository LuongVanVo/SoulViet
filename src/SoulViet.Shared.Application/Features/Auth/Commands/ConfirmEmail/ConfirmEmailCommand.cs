using MediatR;
using SoulViet.Shared.Application.Features.Auth.Results;

namespace SoulViet.Shared.Application.Features.Auth.Commands.ConfirmEmail;

public class ConfirmEmailCommand : IRequest<AuthResponse>
{
    public Guid UserId { get; set; }
    public string Token { get; set; } = string.Empty;
}