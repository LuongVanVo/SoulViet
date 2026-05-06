using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

namespace SoulViet.Modules.SoulMap.SoulMap.Presentation.Helpers;

public static class UserHelper
{
    public static Guid GetCurrentUserId(this ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
            throw new UnauthorizedAccessException("User ID not found in token.");

        return Guid.Parse(userIdClaim);
    }

    public static string GetCurrentUserEmail(this ClaimsPrincipal user)
    {
        var emailClaim = user.FindFirst(ClaimTypes.Email)?.Value
                         ?? user.FindFirst(JwtRegisteredClaimNames.Email)?.Value;

        if (string.IsNullOrEmpty(emailClaim))
            throw new UnauthorizedAccessException("User email not found in token.");

        return emailClaim;
    }

    public static string GetFullName(this ClaimsPrincipal user)
    {
        var fullNameClaim = user.FindFirst("full_name")?.Value
                            ?? user.FindFirst("name")?.Value;

        if (string.IsNullOrEmpty(fullNameClaim))
            throw new UnauthorizedAccessException("User full name not found in token.");

        return fullNameClaim;
    }
}