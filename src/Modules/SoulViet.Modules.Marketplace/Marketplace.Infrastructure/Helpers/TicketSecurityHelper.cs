using System.Security.Cryptography;
using System.Text;

namespace SoulViet.Modules.Marketplace.Marketplace.Infrastructure.Helpers;

public class TicketSecurityHelper
{
    private static readonly string SecretKey = Environment.GetEnvironmentVariable("SECRET_KEY_TICKET") ?? string.Empty;

    public static string GenerateTicketCode(Guid orderItemId, Guid partnerId)
    {
        var payload = $"{orderItemId}:{partnerId}";
        var signature = GenerateSignature(payload);
        return $"{payload}.{signature}";
    }

    public static bool VerifyTicketCode(string ticketCode, out Guid orderItemId)
    {
        orderItemId = Guid.Empty;
        var parts = ticketCode.Split('.');

        if (parts.Length != 2) return false;

        var payload = parts[0];
        var providedSignature = parts[1];

        var expectedSignature = GenerateSignature(payload);

        if (providedSignature == expectedSignature)
        {
            var payloadParts = payload.Split(':');
            if (Guid.TryParse(payloadParts[0], out orderItemId))
            {
                return true;
            }
        }

        return false;
    }

    private static string GenerateSignature(string payload)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(SecretKey));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        return Convert.ToBase64String(hash);
    }
}