using System;

namespace SoulViet.Modules.Social.Social.Application.Common.Pagination;
internal static class Base64Url
{
    public static string Encode(ReadOnlySpan<byte> bytes)
    {
        var base64Len = ((bytes.Length + 2) / 3) * 4;

        Span<char> buffer = base64Len <= 512
            ? stackalloc char[base64Len]
            : new char[base64Len];

        Convert.TryToBase64Chars(bytes, buffer, out var written);

        var trimLen = written;
        while (trimLen > 0 && buffer[trimLen - 1] == '=')
            trimLen--;

        return string.Create(trimLen, buffer[..trimLen], static (span, source) =>
        {
            for (var i = 0; i < span.Length; i++)
            {
                span[i] = source[i] switch
                {
                    '+' => '-',
                    '/' => '_',
                    var c => c
                };
            }
        });
    }

    public static bool TryDecode(ReadOnlySpan<char> input, Span<byte> output, out int bytesWritten)
    {
        bytesWritten = 0;
        if (input.IsEmpty) return false;

        var padLen = (4 - input.Length % 4) % 4;
        var totalLen = input.Length + padLen;

        Span<char> buffer = totalLen <= 512
            ? stackalloc char[totalLen]
            : new char[totalLen];

        for (var i = 0; i < input.Length; i++)
        {
            buffer[i] = input[i] switch
            {
                '-' => '+',
                '_' => '/',
                var c => c
            };
        }

        buffer[input.Length..].Fill('=');

        return Convert.TryFromBase64Chars(buffer, output, out bytesWritten);
    }
}
