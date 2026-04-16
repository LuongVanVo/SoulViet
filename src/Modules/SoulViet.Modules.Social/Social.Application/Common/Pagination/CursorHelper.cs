using System;
using System.Text;

namespace SoulViet.Modules.Social.Social.Application.Common.Pagination;
public static class CursorHelper
{
    private const char Delimiter = '|';
    public static string Encode(Guid id, DateTime createdAt, string sortBy = "newest", int? likeCount = null)
    {
        var ticks = createdAt.ToUniversalTime().Ticks;

        var raw = likeCount is not null
            ? $"{sortBy}{Delimiter}{ticks}{Delimiter}{id:N}{Delimiter}{likeCount}"
            : $"{sortBy}{Delimiter}{ticks}{Delimiter}{id:N}";

        var byteCount = Encoding.UTF8.GetByteCount(raw);

        Span<byte> utf8 = byteCount <= 256
            ? stackalloc byte[byteCount]
            : new byte[byteCount];

        Encoding.UTF8.GetBytes(raw, utf8);

        return Base64Url.Encode(utf8);
    }
    public static (Guid Id, DateTime CreatedAt, string SortBy, int? LikeCount)? Decode(string? cursor)
    {
        if (string.IsNullOrWhiteSpace(cursor))
            return null;

        var maxBytes = (cursor.Length / 4 + 1) * 3;

        Span<byte> utf8 = maxBytes <= 256
            ? stackalloc byte[maxBytes]
            : new byte[maxBytes];

        if (!Base64Url.TryDecode(cursor, utf8, out var bytesWritten))
            return null;

        var raw = Encoding.UTF8.GetString(utf8[..bytesWritten]);

        var parts = raw.Split(Delimiter);
        if (parts.Length is < 3 or > 4)
            return null;

        var sortBy = parts[0];

        if (sortBy is not ("newest" or "oldest" or "top"))
            return null;

        if (!long.TryParse(parts[1], out var ticks))
            return null;

        if (ticks <= 0 || ticks > DateTime.MaxValue.Ticks)
            return null;

        if (!Guid.TryParseExact(parts[2], "N", out var id))
            return null;

        int? likeCount = null;

        if (parts.Length == 4)
        {
            if (!int.TryParse(parts[3], out var parsed) || parsed < 0)
                return null;

            likeCount = parsed;
        }

        return (id, new DateTime(ticks, DateTimeKind.Utc), sortBy, likeCount);
    }
}
