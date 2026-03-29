using MediatR;
using SoulViet.Shared.Application.Exceptions;
using SoulViet.Shared.Application.Features.Auth.Results;
using SoulViet.Shared.Application.Interfaces.Repositories;

namespace SoulViet.Shared.Application.Features.Auth.Commands.ConfirmEmail;

public class ConfirmEmailHandler : IRequestHandler<ConfirmEmailCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository;
    public ConfirmEmailHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<AuthResponse> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByIdAsync(request.UserId);
        if (user == null)
            throw new NotFoundException("User not found");

        // Check token used
        if (user.IsEmailConfirmed)
            throw new BadRequestException("Account has already been confirmed.");

        // Check token valid
        if (user.VerficationToken != request.Token)
            throw new BadRequestException("Invalid verification token.");

        // Check token expired
        if (user.VerficationTokenExpiry < DateTime.UtcNow)
            throw new BadRequestException("Verification token has expired.");

        user.IsEmailConfirmed = true;
        await _userRepository.RemoveVerificationTokenAsync(user);

        return new AuthResponse
        {
            Success = true,
            Message = "Email confirmed successfully."
        };
    }
}