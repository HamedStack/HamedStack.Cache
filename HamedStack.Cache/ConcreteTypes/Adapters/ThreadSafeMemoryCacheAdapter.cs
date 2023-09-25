// ReSharper disable UnusedMember.Global

using HamedStack.Cache.Abstractions;

namespace HamedStack.Cache.ConcreteTypes.Adapters;

/// <summary>
/// An adapter class for the ThreadSafeMemoryCache that implements the ICacheStrategy interface.
/// </summary>
/// <typeparam name="TKey">The type of the key.</typeparam>
/// <typeparam name="TValue">The type of the value.</typeparam>
public class ThreadSafeMemoryCacheAdapter<TKey, TValue> : ICacheStrategy<TKey, TValue> where TKey : notnull
{
    private readonly ThreadSafeMemoryCache<TKey, TValue?> _cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="ThreadSafeMemoryCacheAdapter{TKey, TValue}"/> class.
    /// </summary>
    /// <param name="cache">The instance of ThreadSafeMemoryCache to adapt.</param>
    public ThreadSafeMemoryCacheAdapter(ThreadSafeMemoryCache<TKey, TValue?> cache)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    /// <summary>
    /// Checks if the cache contains an entry with the specified key.
    /// </summary>
    /// <param name="key">The key to check.</param>
    /// <returns>true if the cache contains an entry with the key; otherwise, false.</returns>
    public bool Contains(TKey key) => _cache.Contains(key);

    /// <summary>
    /// Gets the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the value to retrieve.</param>
    /// <returns>The value associated with the key if found; otherwise, the default value for the type TValue.</returns>
    public TValue? Get(TKey key) => _cache.Get(key) ?? default;

    /// <summary>
    /// Inserts or updates the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the value to insert or update.</param>
    /// <param name="value">The value to be inserted or updated.</param>
    /// <param name="expiration">The expiration time for the value. Note: This implementation does not support expiration.</param>
    /// <exception cref="NotSupportedException">Thrown if an expiration value is provided.</exception>
    public void Set(TKey key, TValue? value, TimeSpan? expiration)
    {
        if (expiration != null)
        {
            throw new NotSupportedException();
        }
        _cache.Set(key, value);
    }

    /// <summary>
    /// Removes the value with the specified key from the cache.
    /// </summary>
    /// <param name="key">The key of the value to remove.</param>
    /// <returns>true if the value is removed successfully; otherwise, false.</returns>
    public bool Remove(TKey key) => _cache.Remove(key);
}
