using MassTransit;
using MediatR;
using Microsoft.Extensions.Configuration;
using SoulViet.Shared.Application.Common.Events;
using SoulViet.Shared.Application.Exceptions;
using SoulViet.Shared.Application.Features.Auth.Results;
using SoulViet.Shared.Application.Interfaces;
using SoulViet.Shared.Application.Interfaces.Repositories;

namespace SoulViet.Shared.Application.Features.Auth.Commands.ForgotPassword;

public class ForgotPasswordHandler : IRequestHandler<ForgotPasswordCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ICacheService _cacheService;
    private readonly IConfiguration _configuration;
    public ForgotPasswordHandler(IUserRepository userRepository, IPublishEndpoint publishEndpoint, ICacheService cacheService, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _publishEndpoint = publishEndpoint;
        _cacheService = cacheService;
        _configuration = configuration;
    }

    public async Task<AuthResponse> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByEmailAsync(request.Email);
        if (user == null || !await _userRepository.IsEmailConfirmedAsync(user.Id))
            throw new NotFoundException("User not found or email not confirmed.");

        var resetToken = Guid.NewGuid().ToString("N");

        // Save reset token to redis with expiry time
        var cacheKey = $"pwd_reset:{user.Email}";
        await _cacheService.SetAsync(
            key: cacheKey,
            value: resetToken,
            absoluteExpireTime: TimeSpan.FromMinutes(int.Parse(_configuration["Redis:AbsoluteExpirationMinutes"] ?? "15")),
            cancellationToken: cancellationToken
            );

        // Create link for reset password
        var resetLink = Environment.GetEnvironmentVariable("CLIENT_URL") + $"/api/auth/reset-password?email={Uri.EscapeDataString(user.Email)}&token={Uri.EscapeDataString(resetToken)}";

        // Publish event to send email
        await _publishEndpoint.Publish(new ForgotPasswordEvent
        {
            Email = user.Email,
            ResetLink = resetLink,
            Language = request.Language
        }, cancellationToken);

        return new AuthResponse
        {
            Success = true,
            Message = "Check your email to reset your password."
        };
    }
}