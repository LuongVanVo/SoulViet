using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;
using System.Collections.Concurrent;
using SoulViet.Modules.Social.Social.Application.Interfaces.Services;

namespace SoulViet.Modules.Social.Social.Infrastructure.Services;

public class PostLikeEventService : ILikeEventService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly SseConnectionManager _sseManager;
    private readonly ILogger<PostLikeEventService> _logger;
    private readonly ConcurrentDictionary<Guid, int> _subscriptionCounts = new();
    private readonly object _lock = new();

    public PostLikeEventService(
        IConnectionMultiplexer redis,
        SseConnectionManager sseManager,
        ILogger<PostLikeEventService> logger)
    {
        _redis = redis;
        _sseManager = sseManager;
        _logger = logger;
    }

    public async Task PublishLikeChangedAsync(Guid postId, object likePayload, CancellationToken cancellationToken = default)
    {
        var channel = $"likes:{postId}";
        var json = JsonSerializer.Serialize(likePayload, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        try
        {
            var db = _redis.GetSubscriber();
            await db.PublishAsync(RedisChannel.Literal(channel), json);
            _logger.LogDebug("[SSE] Published like event to channel {Channel}", channel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[SSE] Error publishing like event for post {PostId}", postId);
            throw;
        }
    }

    public async Task SubscribeAsync(Guid postId)
    {
        var channel = $"likes:{postId}";
        bool shouldSubscribe = false;

        lock (_lock)
        {
            var count = _subscriptionCounts.AddOrUpdate(postId, 1, (_, c) => c + 1);
            if (count == 1)
            {
                shouldSubscribe = true;
            }
        }

        if (shouldSubscribe)
        {
            var db = _redis.GetSubscriber();
            await db.SubscribeAsync(RedisChannel.Literal(channel), OnRedisMessageReceived);
            _logger.LogInformation("[SSE] Subscribed to Redis channel '{Channel}'", channel);
        }
    }

    public async Task UnsubscribeAsync(Guid postId)
    {
        var channel = $"likes:{postId}";
        bool shouldUnsubscribe = false;

        lock (_lock)
        {
            if (_subscriptionCounts.TryGetValue(postId, out var count))
            {
                if (count <= 1)
                {
                    _subscriptionCounts.TryRemove(postId, out _);
                    shouldUnsubscribe = true;
                }
                else
                {
                    _subscriptionCounts[postId] = count - 1;
                }
            }
        }

        if (shouldUnsubscribe)
        {
            var db = _redis.GetSubscriber();
            await db.UnsubscribeAsync(RedisChannel.Literal(channel), OnRedisMessageReceived);
            _logger.LogInformation("[SSE] Unsubscribed from Redis channel '{Channel}'", channel);
        }
    }

    private void OnRedisMessageReceived(RedisChannel channel, RedisValue message)
    {
        var channelStr = channel.ToString();
        var parts = channelStr.Split(':');
        if (parts.Length != 2 || !Guid.TryParse(parts[1], out var postId))
        {
            _logger.LogWarning("[SSE] Received message on unexpected channel: {Channel}", channelStr);
            return;
        }

        var clientCount = _sseManager.GetClientCount(postId);
        if (clientCount == 0)
        {
            return;
        }

        var id = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
        _ = _sseManager.BroadcastAsync(postId, "like", message.ToString(), id).ContinueWith(t =>
        {
            if (t.IsFaulted)
                _logger.LogError(t.Exception, "[SSE] Error broadcasting like event to SSE clients for post {PostId}", postId);
        }, TaskContinuationOptions.ExecuteSynchronously);
    }
}
