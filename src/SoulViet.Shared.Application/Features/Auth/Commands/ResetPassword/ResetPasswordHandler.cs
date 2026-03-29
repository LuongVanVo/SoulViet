using MediatR;
using SoulViet.Shared.Application.Exceptions;
using SoulViet.Shared.Application.Features.Auth.Results;
using SoulViet.Shared.Application.Interfaces;
using SoulViet.Shared.Application.Interfaces.Repositories;

namespace SoulViet.Shared.Application.Features.Auth.Commands.ResetPassword;

public class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly ICacheService _cacheService;
    public ResetPasswordHandler(IUserRepository userRepository, ICacheService cacheService)
    {
        _userRepository = userRepository;
        _cacheService = cacheService;
    }

    public async Task<AuthResponse> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByEmailAsync(request.Email);
        if (user == null)
            throw new NotFoundException("User not found.");

        var cacheKey = $"pwd_reset:{user.Email}";
        var savedToken = await _cacheService.GetAsync<string>(cacheKey, cancellationToken);

        if (string.IsNullOrEmpty(savedToken))
            throw new BadRequestException("Token has expired or does not exist. Please request a new password reset.");

        if (savedToken != request.Token)
            throw new BadRequestException("Invalid reset token.");

        // Hash new password
        user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        await _userRepository.UpdateUserAsync(user);

        // Remove token from cache
        await _cacheService.RemoveAsync(cacheKey, cancellationToken);

        return new AuthResponse
        {
            Success = true,
            Message = "Password reset successful."
        };
    }
}