namespace SoulViet.Shared.Application.Interfaces;

public interface ICacheService
{
    // Get data and auto deserialize it to the specified type
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    // Set data (object) into cache with specified key and expiration time
    Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpireTime = null, TimeSpan? slidingExpireTime = null, CancellationToken cancellationToken = default);

    // Remove data from cache by key
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
}