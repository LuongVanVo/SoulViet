using MediatR;
using SoulViet.Shared.Application.Exceptions;
using SoulViet.Shared.Application.Features.Auth.Results;
using SoulViet.Shared.Application.Interfaces;
using SoulViet.Shared.Application.Interfaces.Repositories;
using SoulViet.Shared.Domain.Entities;

namespace SoulViet.Shared.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, AuthResponse>
{
    private readonly ITokenService _tokenService;
    private readonly IUserSessionRepository _userSessionRepository;
    private readonly IUserRepository _userRepository;
    public RefreshTokenHandler(ITokenService tokenService, IUserSessionRepository userSessionRepository, IUserRepository userRepository)
    {
        _tokenService = tokenService;
        _userSessionRepository = userSessionRepository;
        _userRepository = userRepository;
    }

    public async Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // Find refresh token in database
        var storedToken = await _userSessionRepository.GetSessionByRefreshTokenAsync(request.RefreshToken);
        if (storedToken == null)
            throw new NotFoundException("Refresh token not found.");

        if (storedToken.IsUsed)
            throw new BadRequestException("Refresh token has already been used.");

        if (storedToken.IsRevoked)
            throw new BadRequestException("Refresh token has been revoked.");

        if (storedToken.ExpiresAt < DateTime.UtcNow)
            throw new BadRequestException("Refresh token has expired.");

        // Mark the refresh token as used
        storedToken.IsUsed = true;
        storedToken.IsRevoked = true;
        await _userSessionRepository.UpdateSessionAsync(storedToken);

        // Release new token
        var userId = storedToken.UserId;
        var user = await _userRepository.GetUserByIdAsync(userId);
        var roles = await _userRepository.GetUserRolesAsync(userId);

        var (newAccessToken, newJwtId) = _tokenService.GenerateAccessToken(user!, roles);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        // Save new refresh token to database
        var newUserSession = new UserSession
        {
            Id = Guid.NewGuid(),
            UserId = user!.Id,
            RefreshToken = newRefreshToken,
            JwtId = newJwtId,
            IsRevoked = false,
            IsUsed = false,
            ExpiresAt = DateTime.UtcNow.AddDays(Environment.GetEnvironmentVariable("REFRESH_TOKEN_EXPIRY_DAYS") != null
                ? int.Parse(Environment.GetEnvironmentVariable("REFRESH_TOKEN_EXPIRY_DAYS")!)
                : 7),
            IpAddress = request.IpAddress ?? "Unknown",
            DeviceInfo = request.DeviceInfo ?? "Unknown",
        };

        await _userSessionRepository.AddSessionAsync(newUserSession);

        return new AuthResponse
        {
            Success = true,
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            Message = "Token refreshed successfully."
        };
    }
}