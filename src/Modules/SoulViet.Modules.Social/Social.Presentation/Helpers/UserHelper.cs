using Microsoft.IdentityModel.JsonWebTokens;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace SoulViet.Modules.Social.Social.Presentation.Helpers
{
    public static class UserHelper
    {
        public static Guid GetCurrentUserId(this ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException("User ID not found in token.");

            return Guid.Parse(userIdClaim);
        }
    }
}
