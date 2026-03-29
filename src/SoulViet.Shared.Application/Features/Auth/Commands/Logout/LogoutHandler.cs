using MediatR;
using SoulViet.Shared.Application.Exceptions;
using SoulViet.Shared.Application.Features.Auth.Results;
using SoulViet.Shared.Application.Interfaces.Repositories;

namespace SoulViet.Shared.Application.Features.Auth.Commands.Logout;

public class LogoutHandler : IRequestHandler<LogoutCommand, AuthResponse>
{
    private readonly IUserSessionRepository _userSessionRepository;
    public LogoutHandler(IUserSessionRepository userSessionRepository)
    {
        _userSessionRepository = userSessionRepository;
    }

    public async Task<AuthResponse> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var token = await _userSessionRepository.GetSessionByRefreshTokenAsync(request.RefreshToken);
        if (token == null)
            throw new NotFoundException("Refresh token not found.");

        token.IsRevoked = true;
        token.IsUsed = true;
        await _userSessionRepository.UpdateSessionAsync(token);

        return new AuthResponse
        {
            Success = true,
            Message = "Logout successful."
        };
    }
}