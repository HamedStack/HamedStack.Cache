// ReSharper disable UnusedMember.Global

using HamedStack.Cache.Abstractions;
using Microsoft.Extensions.Caching.Distributed;

namespace HamedStack.Cache.ConcreteTypes.Adapters;

/// <summary>
/// Provides an adapter implementation over IDistributedCache to match the ICacheStrategy interface.
/// </summary>
public class DistributedCacheAdapter : ICacheStrategy<string, byte[]>
{
    private readonly IDistributedCache _cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="DistributedCacheAdapter"/> class.
    /// </summary>
    /// <param name="cache">The underlying distributed cache instance.</param>
    public DistributedCacheAdapter(IDistributedCache cache)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    /// <summary>
    /// Determines if the cache contains an entry with the specified key.
    /// </summary>
    /// <param name="key">The key to search for.</param>
    /// <returns><c>true</c> if the cache contains an entry with the key; otherwise, <c>false</c>.</returns>
    public bool Contains(string key) => _cache.Get(key) != null;

    /// <summary>
    /// Retrieves the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the item to retrieve.</param>
    /// <returns>The byte array associated with the specified key, if found; otherwise, <c>null</c>.</returns>
    public byte[]? Get(string key) => _cache.Get(key);

    /// <summary>
    /// Adds or updates the value in the cache with the specified key.
    /// </summary>
    /// <param name="key">The key of the item to set.</param>
    /// <param name="value">The value to cache.</param>
    /// <param name="expiration">Optional expiration for the cached item.</param>
    public void Set(string key, byte[]? value, TimeSpan? expiration)
    {
        var options = new DistributedCacheEntryOptions();
        if (expiration.HasValue)
        {
            options.SetAbsoluteExpiration(expiration.Value);
        }
        _cache.Set(key, value ?? throw new ArgumentNullException(nameof(value)), options);
    }

    /// <summary>
    /// Removes the item from the cache with the specified key.
    /// </summary>
    /// <param name="key">The key of the item to remove.</param>
    /// <returns><c>true</c> if the item was successfully removed; otherwise, <c>false</c>.</returns>
    public bool Remove(string key)
    {
        _cache.Remove(key);
        return !Contains(key);
    }
}