// ReSharper disable UnusedMember.Global

using HamedStack.Cache.Abstractions;
using Microsoft.Extensions.Caching.Memory;

namespace HamedStack.Cache.ConcreteTypes.Adapters;

/// <summary>
/// Provides an adapter implementation over IMemoryCache to match the ICacheStrategy interface.
/// </summary>
/// <typeparam name="TKey">The type of the key used to identify cached items.</typeparam>
/// <typeparam name="TValue">The type of the value stored in the cache.</typeparam>
public class MemoryCacheAdapter<TKey, TValue> : ICacheStrategy<TKey, TValue> where TKey : notnull
{
    private readonly IMemoryCache _cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryCacheAdapter{TKey, TValue}"/> class.
    /// </summary>
    /// <param name="cache">The underlying memory cache instance.</param>
    public MemoryCacheAdapter(IMemoryCache cache)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    /// <summary>
    /// Determines if the cache contains an entry with the specified key.
    /// </summary>
    /// <param name="key">The key to search for.</param>
    /// <returns><c>true</c> if the cache contains an entry with the key; otherwise, <c>false</c>.</returns>
    public bool Contains(TKey key) => _cache.TryGetValue(key, out TValue? _);

    /// <summary>
    /// Retrieves the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the item to retrieve.</param>
    /// <returns>The value of type <typeparamref name="TValue"/> associated with the specified key, if found; otherwise, <c>null</c>.</returns>
    public TValue? Get(TKey key) => _cache.Get<TValue>(key);

    /// <summary>
    /// Adds or updates the value in the cache with the specified key.
    /// </summary>
    /// <param name="key">The key of the item to set.</param>
    /// <param name="value">The value to cache.</param>
    /// <param name="expiration">Optional expiration for the cached item.</param>
    public void Set(TKey key, TValue? value, TimeSpan? expiration)
    {
        if (expiration.HasValue)
        {
            _cache.Set(key, value, expiration.Value);
        }
        else
        {
            _cache.Set(key, value);
        }
    }

    /// <summary>
    /// Removes the item from the cache with the specified key.
    /// </summary>
    /// <param name="key">The key of the item to remove.</param>
    /// <returns><c>true</c> if the item was successfully removed; otherwise, <c>false</c>.</returns>
    public bool Remove(TKey key)
    {
        _cache.Remove(key);
        return !Contains(key);
    }
}
