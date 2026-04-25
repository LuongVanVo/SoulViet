using Microsoft.AspNetCore.Http;
using System.Text;

namespace SoulViet.Modules.Social.Social.Infrastructure.Services;

public static class SseWriter
{
    public static async Task WriteEventAsync(HttpResponse response, string eventName, string jsonData, string? id = null)
    {
        var message = new StringBuilder();
        if (!string.IsNullOrEmpty(id))
        {
            message.Append($"id: {id}\n");
        }
        message.Append($"event: {eventName}\n");
        message.Append($"data: {jsonData}\n");
        message.Append("\n");

        await response.WriteAsync(message.ToString(), Encoding.UTF8);
        await response.Body.FlushAsync();
    }

    public static async Task WriteKeepAliveAsync(HttpResponse response)
    {
        await response.WriteAsync(": ping\n\n", Encoding.UTF8);
        await response.Body.FlushAsync();
    }

    public static void SetSseHeaders(HttpResponse response)
    {
        response.Headers["Content-Type"] = "text/event-stream";
        response.Headers["Cache-Control"] = "no-cache";
        response.Headers["Connection"] = "keep-alive";
        response.Headers["X-Accel-Buffering"] = "no";
    }
}
