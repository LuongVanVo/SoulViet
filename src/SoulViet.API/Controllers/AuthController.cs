using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoulViet.API.Helper;
using SoulViet.Shared.Application.Features.Auth.Commands.ConfirmEmail;
using SoulViet.Shared.Application.Features.Auth.Commands.ForgotPassword;
using SoulViet.Shared.Application.Features.Auth.Commands.Login;
using SoulViet.Shared.Application.Features.Auth.Commands.Logout;
using SoulViet.Shared.Application.Features.Auth.Commands.RefreshToken;
using SoulViet.Shared.Application.Features.Auth.Commands.Register;
using SoulViet.Shared.Application.Features.Auth.Commands.ResetPassword;
using SoulViet.Shared.Application.Features.Auth.Queries.GetUserProfile;
using SoulViet.Shared.Application.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace SoulViet.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Tags("Auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICookieService _cookieService;
    public AuthController(IMediator mediator, ICookieService cookieService)
    {
        _mediator = mediator;
        _cookieService = cookieService;
    }

    [HttpPost("register")]
    [SwaggerOperation(Summary = "Register a new user", Description = "Registers a new user with the provided email, full name, password, and language.")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("confirm-email")]
    [SwaggerOperation(Summary = "Confirm email address", Description = "Confirms the user's email address using the provided token and user ID.")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmEmailCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("login")]
    [SwaggerOperation(Summary = "Login user", Description = "Logs in a user with the provided email and password.")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        command.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        command.DeviceInfo = Request.Headers["User-Agent"].ToString();
        var result = await _mediator.Send(command);

        // Set cookie
        _cookieService.SetAuthCookie(result.AccessToken ?? string.Empty, result.RefreshToken ?? string.Empty);

        return Ok(result);
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Get current user ID",
        Description = "Retrieves the ID of the currently authenticated user.")]
    public async Task<IActionResult> GetCurrentUserId()
    {
        var query = new GetUserProfileQuery();
        query.UserId = User.GetCurrentUserId();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [Authorize]
    [HttpPost("logout")]
    [SwaggerOperation(Summary = "Logout user",
        Description = "Logs out the currently authenticated user by clearing the authentication cookies.")]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = Request.Cookies["refresh_token"];
        var command = new LogoutCommand() { RefreshToken = refreshToken ?? string.Empty };
        var result = await _mediator.Send(command);

        // Clear cookie
        _cookieService.RemoveAuthCookie();

        return Ok(result);
    }

    // Forgot password
    [HttpPost("forgot-password")]
    [SwaggerOperation(Summary = "Forgot password",
        Description = "Sends a password reset email to the user with the provided email address.")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
    {
        var response = await _mediator.Send(command);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpPost("reset-password")]
    [SwaggerOperation(Summary = "Reset password",
        Description = "Resets the user's password using the provided token, user ID, and new password.")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
    {
        var response = await _mediator.Send(command);
        if (!response.Success)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }

    [Authorize(Roles = "Tourist")]
    [HttpGet("data-secret")]
    public IActionResult DataSecret()
    {
        return Ok("Nà ná na na anh độ mixi");
    }

    [HttpPost("refresh-token")]
    [SwaggerOperation(Summary = "Refresh access token",
        Description = "Refreshes the access token using the provided refresh token.")]
    public async Task<IActionResult> RefreshToken()
    {
        var refreshToken = Request.Cookies["refresh_token"];
        var command = new RefreshTokenCommand
        {
            RefreshToken = refreshToken ?? string.Empty,
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
            DeviceInfo = Request.Headers["User-Agent"].ToString()
        };

        var result = await _mediator.Send(command);

        // set new cookie
        _cookieService.SetAuthCookie(result.AccessToken ?? string.Empty, result.RefreshToken ?? string.Empty);

        return Ok(result);
    }
}