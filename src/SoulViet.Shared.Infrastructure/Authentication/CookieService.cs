using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SoulViet.Shared.Application.Interfaces;

namespace SoulViet.Shared.Infrastructure.Authentication;

public class CookieService : ICookieService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;
    public CookieService(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
    {
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
    }

    public void SetAuthCookie(string accessToken, string refreshToken)
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null) return;

        var cookieExpiryDaysStr = _configuration["COOKIE_EXPIRY_DAYS"] ?? Environment.GetEnvironmentVariable("COOKIE_EXPIRY_DAYS") ?? "7";
        _ = int.TryParse(cookieExpiryDaysStr, out var cookieExpiryDays);

        var cookiesOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(cookieExpiryDays)
        };

        context.Response.Cookies.Append("access_token", accessToken, cookiesOptions);
        context.Response.Cookies.Append("refresh_token", refreshToken, cookiesOptions);
    }

    public void RemoveAuthCookie()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null) return;
        context.Response.Cookies.Delete("access_token");
        context.Response.Cookies.Delete("refresh_token");
    }
}