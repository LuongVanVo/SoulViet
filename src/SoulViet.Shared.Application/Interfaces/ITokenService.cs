using System.Security.Claims;
using SoulViet.Shared.Domain.Entities;

namespace SoulViet.Shared.Application.Interfaces;

public interface ITokenService
{
    (string AccessToken, string JwtId) GenerateAccessToken(User user, IList<string> roles);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}