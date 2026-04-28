using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SoulViet.Shared.Application.Interfaces;
using SoulViet.Shared.Domain.Entities;

namespace SoulViet.Shared.Infrastructure.Authentication;

public class TokenService : ITokenService
{
    private readonly IJwtKeyProvider _jwtKeyProvider;
    private readonly IConfiguration _configuration;

    public TokenService(IJwtKeyProvider jwtKeyProvider, IConfiguration configuration)
    {
        _jwtKeyProvider = jwtKeyProvider;
        _configuration = configuration;
    }

    public (string AccessToken, string JwtId) GenerateAccessToken(User user, IList<string> roles)
    {
        var key = _jwtKeyProvider.GetPrivateKey();
        var credentials = new SigningCredentials(new RsaSecurityKey(key), SecurityAlgorithms.RsaSha256);
        var jwtId = Guid.NewGuid().ToString();

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new Claim("full_name", $"{user.FullName}".Trim()),
            new Claim(JwtRegisteredClaimNames.Jti, jwtId)
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var expiryMinutueStr = _configuration["JWT_EXPIRY_MINUTES"] ?? "30";
        _ = double.TryParse(expiryMinutueStr, out var expiryMinutes);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(expiryMinutes),
            SigningCredentials = credentials
        };

        var tokenHander = new JwtSecurityTokenHandler();
        var token = tokenHander.CreateToken(tokenDescriptor);

        return (tokenHander.WriteToken(token), jwtId);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new RsaSecurityKey(_jwtKeyProvider.GetPublicKey()),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.RsaSha256,
                StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }
}