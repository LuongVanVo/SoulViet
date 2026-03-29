using MediatR;
using SoulViet.Shared.Application.Exceptions;
using SoulViet.Shared.Application.Features.Auth.Results;
using SoulViet.Shared.Application.Interfaces;
using SoulViet.Shared.Application.Interfaces.Repositories;
using SoulViet.Shared.Domain.Entities;

namespace SoulViet.Shared.Application.Features.Auth.Commands.Login;

public class LoginHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IUserSessionRepository _userSessionRepository;

    public LoginHandler(IUserRepository userRepository, ITokenService tokenService,
        IUserSessionRepository userSessionRepository)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _userSessionRepository = userSessionRepository;
    }

    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // Check if user exists
        var user = await _userRepository.GetUserByEmailAsync(request.Email);
        if (user == null)
            throw new NotFoundException($"User with email {request.Email} not found.");

        // Check account active and email verified
        if (!user.IsActive || !user.IsEmailConfirmed)
            throw new BadRequestException("Account is not active or email is not verified.");

        // Check password
        var isValidPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);
        if (!isValidPassword)
            throw new BadRequestException("Invalid password or email! Please try again.");

        // get user roles
        var roles = await _userRepository.GetUserRolesAsync(user.Id);

        // Generate token
        var (accessToken, jwtId) = _tokenService.GenerateAccessToken(user, roles);
        var refreshToken = _tokenService.GenerateRefreshToken();

        // save refresh token to database
        var userSession = new UserSession
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            RefreshToken = refreshToken,
            JwtId = jwtId,
            IsRevoked = false,
            IsUsed = false,
            ExpiresAt = DateTime.UtcNow.AddDays(Environment.GetEnvironmentVariable("REFRESH_TOKEN_EXPIRY_DAYS") != null
                ? int.Parse(Environment.GetEnvironmentVariable("REFRESH_TOKEN_EXPIRY_DAYS")!)
                : 7),
            IpAddress = request.IpAddress ?? "Unknown",
            DeviceInfo = request.DeviceInfo ?? "Unknown",
        };

        await _userSessionRepository.AddSessionAsync(userSession);

        return new AuthResponse
        {
            Success = true,
            Message = "Login successful",
            AccessToken = accessToken,
            RefreshToken = refreshToken,
        };
    }
}