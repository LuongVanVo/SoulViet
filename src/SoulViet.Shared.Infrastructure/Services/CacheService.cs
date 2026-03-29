using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using SoulViet.Shared.Application.Interfaces;

namespace SoulViet.Shared.Infrastructure.Services;

public class CacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    public CacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var cachedData = await _cache.GetStringAsync(key, cancellationToken);

        if (string.IsNullOrEmpty(cachedData))
            return default;

        return JsonSerializer.Deserialize<T>(cachedData);
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

        var serializedData = JsonSerializer.Serialize(value);

        await _cache.SetStringAsync(key, serializedData, options, cancellationToken);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _cache.RemoveAsync(key, cancellationToken);
    }
}