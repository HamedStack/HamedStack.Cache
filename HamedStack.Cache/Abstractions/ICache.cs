// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global

using System.Collections;

namespace HamedStack.Cache.Abstractions;

/// <summary>
/// Represents a cache interface with key-value pairs.
/// </summary>
/// <typeparam name="TKey">The type of the key.</typeparam>
/// <typeparam name="TValue">The type of the value.</typeparam>
public interface ICache<TKey, TValue> : IEnumerable where TKey : notnull
{
    /// <summary>
    /// Gets the underlying data of the cache as a dictionary.
    /// </summary>
    Dictionary<TKey, TValue?> CacheData { get; }

    /// <summary>
    /// Gets or sets the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the value to get or set.</param>
    /// <returns>The value associated with the specified key.</returns>
    TValue? this[TKey key] { get; set; }

    /// <summary>
    /// Determines whether the cache contains an element with the specified key.
    /// </summary>
    /// <param name="key">The key to locate in the cache.</param>
    /// <returns>true if the cache contains an element with the key; otherwise, false.</returns>
    bool Contains(TKey key);

    /// <summary>
    /// Gets the number of elements contained in the cache.
    /// </summary>
    /// <returns>The number of elements contained in the cache.</returns>
    int Count();

    /// <summary>
    /// Gets the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the value to get.</param>
    /// <returns>The value associated with the specified key.</returns>
    TValue? Get(TKey key);

    /// <summary>
    /// Gets the value associated with the specified key or sets it using the provided delegate.
    /// </summary>
    /// <param name="key">The key of the value to get or set.</param>
    /// <param name="value">The delegate to use if the key is not found.</param>
    /// <returns>The value associated with the specified key.</returns>
    TValue? GetOrSet(TKey key, Func<TValue> value);

    /// <summary>
    /// Gets the value associated with the specified key or sets it using the provided delegate.
    /// </summary>
    /// <param name="key">The key of the value to get or set.</param>
    /// <param name="value">The delegate to use if the key is not found.</param>
    /// <returns>The value associated with the specified key.</returns>
    TValue? GetOrSet(TKey key, Func<TKey, TValue> value);

    /// <summary>
    /// Gets the value associated with the specified key or sets it if it does not exist.
    /// </summary>
    /// <param name="key">The key of the value to get or set.</param>
    /// <param name="value">The value to set if the key is not found.</param>
    /// <returns>The value associated with the specified key.</returns>
    TValue? GetOrSet(TKey key, TValue? value);

    /// <summary>
    /// Returns a collection containing the keys in the cache.
    /// </summary>
    /// <returns>A collection of keys.</returns>
    IEnumerable<TKey> Keys();

    /// <summary>
    /// Removes the element with the specified key from the cache.
    /// </summary>
    /// <param name="key">The key of the element to remove.</param>
    /// <returns>true if the element is successfully removed; otherwise, false.</returns>
    bool Remove(TKey key);

    /// <summary>
    /// Removes all keys and values from the cache.
    /// </summary>
    void RemoveAll();

    /// <summary>
    /// Sets the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the value to set.</param>
    /// <param name="value">The value to set for the specified key.</param>
    void Set(TKey key, TValue? value);

    /// <summary>
    /// Tries to get the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the value to get.</param>
    /// <param name="hasKey">true if the cache contains an element with the key; otherwise, false.</param>
    /// <returns>The value associated with the specified key, if it exists; otherwise, default.</returns>
    TValue? TryGet(TKey key, out bool hasKey);

    /// <summary>
    /// Tries to remove the element with the specified key from the cache.
    /// </summary>
    /// <param name="key">The key of the element to remove.</param>
    /// <param name="value">When this method returns, contains the removed value, if the key was found; otherwise, default.</param>
    /// <returns>true if the element is successfully removed; otherwise, false.</returns>
    bool TryRemove(TKey key, out TValue? value);

    /// <summary>
    /// Tries to set the value for the specified key.
    /// </summary>
    /// <param name="key">The key of the value to set.</param>
    /// <param name="value">The value to set for the specified key.</param>
    /// <returns>true if the value was set successfully; otherwise, false.</returns>
    bool TrySet(TKey key, TValue? value);

    /// <summary>
    /// Returns a collection containing the values in the cache.
    /// </summary>
    /// <returns>A collection of values.</returns>
    ICollection<TValue?> Values();
}