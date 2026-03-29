using AutoMapper;
using MassTransit;
using MediatR;
using SoulViet.Shared.Application.Common.Events;
using SoulViet.Shared.Application.Exceptions;
using SoulViet.Shared.Application.Features.Auth.Results;
using SoulViet.Shared.Application.Interfaces.Repositories;
using SoulViet.Shared.Domain.Entities;

namespace SoulViet.Shared.Application.Features.Auth.Commands.Register;

public class RegisterHandler : IRequestHandler<RegisterCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishEndpoint;
    public RegisterHandler(IUserRepository userRepository, IMapper mapper, IRoleRepository roleRepository, IPublishEndpoint publishEndpoint)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _roleRepository = roleRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // check if user already exists
        var existingUser = await _userRepository.GetUserByEmailAsync(request.Email);
        if (existingUser != null)
            throw new BadRequestException($"User with email {request.Email} already exists.");

        // create new user
        var newUser = _mapper.Map<User>(request);
        newUser.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var touristRole = await _roleRepository.GetRoleByNameAsync("Tourist");
        if (touristRole != null)
        {
            newUser.UserRoles = new List<UserRole>
            {
                new UserRole { UserId = newUser.Id ,RoleId = touristRole.Id }
            };
        }


        var isVietNamese = !string.IsNullOrEmpty(request.Language) &&
                           request.Language.StartsWith("vi", StringComparison.OrdinalIgnoreCase);
        // Send email
        var verificationToken = Guid.NewGuid().ToString();
        newUser.VerficationToken = verificationToken;
        newUser.VerficationTokenExpiry = DateTime.UtcNow.AddHours(24);

        await _userRepository.CreateUserAsync(newUser);

        var vertificationLink = Environment.GetEnvironmentVariable("CLIENT_URL") + $"/api/auth/confirm-email?token={verificationToken}&userId={newUser.Id}";
        var emailSubject = isVietNamese ? "Xác thưc tài khoản SoulViet" : "Confirm your SoulViet account";
        var emailBody = isVietNamese
            ? $@"
                <html>
                    <body>
                        <h2>Chào {request.FullName},</h2>
                        <p>Cảm ơn bạn đã đăng ký tài khoản tại SoulViet.</p>
                        <p>Vui lòng click vào nút bên dưới để kích hoạt tài khoản:</p>
                        <a href='{vertificationLink}' style='background-color: #4CAF50; color: white; padding: 10px 20px; text-decoration: none;'>Kích hoạt ngay</a>
                    </body>
                </html>"
            : $@"
                <html>
                    <body>
                        <h2>Hello {request.FullName},</h2>
                        <p>Thank you for registering an account at SoulViet.</p>
                        <p>Please click the button below to activate your account:</p>
                        <a href='{vertificationLink}' style='background-color: #4CAF50; color: white; padding: 10px 20px; text-decoration: none;'>Activate Now</a>
                    </body>
                </html>";

        // Publish event to send email
        await _publishEndpoint.Publish(new UserRegisteredEvent
        {
            Email = request.Email,
            Subject = emailSubject,
            Body = emailBody
        }, cancellationToken);

        return new AuthResponse
        {
            Success = true,
            Message = "User registered successfully."
        };
    }
}