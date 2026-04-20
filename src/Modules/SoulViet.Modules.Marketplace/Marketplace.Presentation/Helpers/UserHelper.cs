using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

namespace SoulViet.Modules.Marketplace.Marketplace.Presentation.Helpers;

public static class UserHelper
{
    public static Guid GetCurrentUserId(this ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (string.IsNullOrEmpty(userIdClaim))
            throw new UnauthorizedAccessException("User ID not found in token.");

        return Guid.Parse(userIdClaim);
    }

    public static string GetCurrentUserEmail(this ClaimsPrincipal user)
    {
        var emailClaim = user.FindFirstValue(ClaimTypes.Email) ?? user.FindFirstValue(JwtRegisteredClaimNames.Email);

        if (string.IsNullOrEmpty(emailClaim))
            throw new UnauthorizedAccessException("User email not found in token.");

        return emailClaim;
    }
}