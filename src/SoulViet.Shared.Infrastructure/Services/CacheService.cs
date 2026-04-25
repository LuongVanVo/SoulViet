using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using SoulViet.Shared.Application.Interfaces;

namespace SoulViet.Shared.Infrastructure.Services;

public class CacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly IConnectionMultiplexer _redis;
    private readonly JsonSerializerOptions _jsonOptions;

    public CacheService(IDistributedCache cache, IConnectionMultiplexer redis)
    {
        _cache = cache;
        _redis = redis;
        _jsonOptions = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var cachedData = await _cache.GetStringAsync(key, cancellationToken);

        if (string.IsNullOrEmpty(cachedData))
            return default;

        return JsonSerializer.Deserialize<T>(cachedData, _jsonOptions);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpireTime = null, TimeSpan? slidingExpireTime = null,
        CancellationToken cancellationToken = default)
    {
        var options = new DistributedCacheEntryOptions();

        // Absolute: Hết hạn tính từ lúc tạo
        if (absoluteExpireTime.HasValue)
            options.AbsoluteExpirationRelativeToNow = absoluteExpireTime;

        // Sliding: Hết hạn nếu không ai đụng vào (ví dụ: 15 phút không request thì cache sẽ hết hạn)
        if (slidingExpireTime.HasValue)
            options.SlidingExpiration = slidingExpireTime;

        var serializedData = JsonSerializer.Serialize(value, _jsonOptions);

        await _cache.SetStringAsync(key, serializedData, options, cancellationToken);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _cache.RemoveAsync(key, cancellationToken);
    }
    public async Task<long> IncrementAsync(string key, CancellationToken cancellationToken = default)
    {
        var db = _redis.GetDatabase();
        return await db.StringIncrementAsync(key);
    }
    public async Task<long> DecrementAsync(string key, CancellationToken cancellationToken = default)
    {
        var db = _redis.GetDatabase();
        var script = @"
            local val = redis.call('GET', KEYS[1])
            if val and tonumber(val) > 0 then
                return redis.call('DECR', KEYS[1])
            else
                return 0
            end";
        var result = await db.ScriptEvaluateAsync(script, new RedisKey[] { key });
        return (long)result;
    }
    public async Task<bool> ZAddAsync<T>(string key, T value, double score, CancellationToken cancellationToken = default)
    {
        var db = _redis.GetDatabase();

        var serialized = JsonSerializer.Serialize(value, _jsonOptions);

        return await db.SortedSetAddAsync(key, serialized, score);
    }
    public async Task<bool> ZRemoveAsync<T>(string key, T value, CancellationToken cancellationToken = default)
    {
        var db = _redis.GetDatabase();

        var serialized = JsonSerializer.Serialize(value, _jsonOptions);

        return await db.SortedSetRemoveAsync(key, serialized);
    }
    public async Task<IEnumerable<T>> ZRangeAsync<T>(string key, int start, int stop, bool descending = false, CancellationToken cancellationToken = default)
    {
        var db = _redis.GetDatabase();

        var values = await db.SortedSetRangeByRankAsync(
            key,
            start,
            stop,
            descending ? Order.Descending : Order.Ascending
        );

        var result = new List<T>();

        foreach (var value in values)
        {
            if (!value.IsNullOrEmpty)
            {
                var deserialized = JsonSerializer.Deserialize<T>((string)value!, _jsonOptions);
                if (deserialized != null)
                    result.Add(deserialized);
            }
        }

        return result;
    }
}