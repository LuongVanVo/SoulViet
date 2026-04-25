namespace SoulViet.Shared.Application.Interfaces;

public interface ICacheService
{
    // Get data and auto deserialize it to the specified type
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    // Set data (object) into cache with specified key and expiration time
    Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpireTime = null, TimeSpan? slidingExpireTime = null, CancellationToken cancellationToken = default);

    // Remove data from cache by key
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    // Atomically increment a counter by 1. Returns the new value. Uses Redis INCR command.
    Task<long> IncrementAsync(string key, CancellationToken cancellationToken = default);

    // Atomically decrement a counter by 1, floor at 0. Returns the new value. Uses Redis DECR command.
    Task<long> DecrementAsync(string key, CancellationToken cancellationToken = default);
    //Sorted Set
    Task<bool> ZAddAsync<T>(string key, T value, double score, CancellationToken cancellationToken = default);
    Task<bool> ZRemoveAsync<T>(string key, T value, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> ZRangeAsync<T>(string key, int start, int stop, bool descending = false, CancellationToken cancellationToken = default);

}