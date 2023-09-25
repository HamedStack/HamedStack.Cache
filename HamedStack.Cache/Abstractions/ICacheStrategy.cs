namespace HamedStack.Cache.Abstractions;

/// <summary>
/// Defines the operations of a caching strategy.
/// </summary>
/// <typeparam name="TKey">The type of the key used to identify cached items.</typeparam>
/// <typeparam name="TValue">The type of the value stored in the cache.</typeparam>
public interface ICacheStrategy<in TKey, TValue>
{
    /// <summary>
    /// Determines if the cache contains an entry with the specified key.
    /// </summary>
    /// <param name="key">The key to search for.</param>
    /// <returns><c>true</c> if the cache contains an entry with the key; otherwise, <c>false</c>.</returns>
    bool Contains(TKey key);

    /// <summary>
    /// Retrieves the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the item to retrieve.</param>
    /// <returns>The value associated with the specified key, if found; otherwise, <c>null</c>.</returns>
    TValue? Get(TKey key);

    /// <summary>
    /// Adds or updates the value in the cache with the specified key.
    /// </summary>
    /// <param name="key">The key of the item to set.</param>
    /// <param name="value">The value to cache.</param>
    /// <param name="expiration">Optional expiration for the cached item.</param>
    void Set(TKey key, TValue? value, TimeSpan? expiration);

    /// <summary>
    /// Removes the item from the cache with the specified key.
    /// </summary>
    /// <param name="key">The key of the item to remove.</param>
    /// <returns><c>true</c> if the item was successfully removed; otherwise, <c>false</c>.</returns>
    bool Remove(TKey key);
}