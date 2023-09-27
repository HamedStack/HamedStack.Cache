// ReSharper disable UnusedType.Global

using System.Collections;
using System.Collections.Concurrent;
using HamedStack.Cache.Abstractions;

namespace HamedStack.Cache.ConcreteTypes;

/// <summary>
/// Provides a thread-safe in-memory cache using ConcurrentDictionary.
/// </summary>
/// <typeparam name="TKey">The type of the keys in the cache.</typeparam>
/// <typeparam name="TValue">The type of the values in the cache.</typeparam>
public class ThreadSafeMemoryCache<TKey, TValue> : ICache<TKey, TValue> where TKey : notnull
{
    private readonly ConcurrentDictionary<TKey, Lazy<TValue?>> _cache = new();

    /// <summary>
    /// Gets a dictionary representing the cache's data.
    /// </summary>
    public Dictionary<TKey, TValue?> CacheData => _cache.ToDictionary(entry => entry.Key, entry => entry.Value.Value);

    /// <summary>
    /// Gets or sets the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the value to get or set.</param>
    public TValue? this[TKey key]
    {
        get => _cache[key].Value;
        set => _cache[key] = new Lazy<TValue?>(() => value, true);
    }

    /// <summary>
    /// Determines whether the cache contains an element with the specified key.
    /// </summary>
    /// <param name="key">The key to locate in the cache.</param>
    /// <returns>true if the cache contains an element with the key; otherwise, false.</returns>
    public bool Contains(TKey key) => _cache.ContainsKey(key);

    /// <summary>
    /// Gets the number of key/value pairs contained in the cache.
    /// </summary>
    public int Count() => _cache.Count;

    /// <summary>
    /// Gets the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the value to get.</param>
    /// <returns>The value associated with the specified key, or default(TValue) if the key does not exist.</returns>
    public TValue? Get(TKey key) => _cache.TryGetValue(key, out var value) ? value.Value : default;

    /// <summary>
    /// Gets the value associated with the specified key or creates and adds it if the key does not exist.
    /// </summary>
    /// <param name="key">The key of the value to get.</param>
    /// <param name="value">The function used to generate a value for the key.</param>
    /// <returns>The value for the key.</returns>
    public TValue? GetOrSet(TKey key, Func<TValue> value)
    {
        return _cache.GetOrAdd(key, new Lazy<TValue?>(value, true)).Value;
    }

    /// <summary>
    /// Returns an enumerator that iterates through the cache.
    /// </summary>
    /// <returns>An enumerator for the cache.</returns>
    public IEnumerator GetEnumerator() => _cache.GetEnumerator();

    /// <summary>
    /// Gets the value associated with the specified key or creates and adds it if the key does not exist.
    /// </summary>
    /// <param name="key">The key of the value to get or set.</param>
    /// <param name="value">The value to associate with the key if the key does not exist.</param>
    /// <returns>The value for the key.</returns>
    public TValue? GetOrSet(TKey key, TValue? value) => _cache.GetOrAdd(key, new Lazy<TValue?>(() => value, true)).Value;

    /// <summary>
    /// Gets the value associated with the specified key or creates and adds it using the provided function if the key does not exist.
    /// </summary>
    /// <param name="key">The key of the value to get or set.</param>
    /// <param name="valueFunc">The function used to generate a value for the key.</param>
    /// <returns>The value for the key.</returns>
    public TValue? GetOrSet(TKey key, Func<TKey, TValue> valueFunc)
    {
        return _cache.GetOrAdd(key, new Lazy<TValue?>(() => valueFunc(key), true)).Value;
    }

    /// <summary>
    /// Gets all the keys in the cache.
    /// </summary>
    /// <returns>A collection of keys in the cache.</returns>
    public IEnumerable<TKey> Keys() => _cache.Keys;

    /// <summary>
    /// Removes the value with the specified key from the cache.
    /// </summary>
    /// <param name="key">The key of the element to remove.</param>
    /// <returns>true if the element is successfully removed; otherwise, false.</returns>
    public bool Remove(TKey key)
    {
        return _cache.TryRemove(key, out _);
    }

    /// <summary>
    /// Removes all keys and values from the cache.
    /// </summary>
    public void RemoveAll() => _cache.Clear();

    /// <summary>
    /// Inserts or updates a key/value pair in the cache.
    /// </summary>
    /// <param name="key">The key of the element to add or update.</param>
    /// <param name="value">The value to be added or updated in the cache.</param>
    public void Set(TKey key, TValue? value) => _cache.AddOrUpdate(key, new Lazy<TValue?>(() => value, true), (_, _) => new Lazy<TValue?>(() => value));

    /// <summary>
    /// Tries to get the value associated with the specified key from the cache.
    /// </summary>
    /// <param name="key">The key of the value to get.</param>
    /// <param name="hasKey">When this method returns, contains true if the cache contains an element with the specified key; otherwise, false.</param>
    /// <returns>The value associated with the specified key, or default(TValue) if the key does not exist.</returns>
    public TValue? TryGet(TKey key, out bool hasKey)
    {
        hasKey = _cache.TryGetValue(key, out var value);
        if (value != null) return hasKey ? value.Value : default;
        return default;
    }

    /// <summary>
    /// Tries to remove the value with the specified key from the cache.
    /// </summary>
    /// <param name="key">The key of the value to remove.</param>
    /// <param name="value">When this method returns, if the key was found, contains the value associated with the key; otherwise, contains default(TValue).</param>
    /// <returns>true if the value was removed successfully; otherwise, false.</returns>
    public bool TryRemove(TKey key, out TValue? value)
    {
        var result = _cache.TryRemove(key, out var lazyValue);
        value = lazyValue != null ? lazyValue.Value : default;
        return result;
    }

    /// <summary>
    /// Tries to insert or update a key/value pair in the cache.
    /// </summary>
    /// <param name="key">The key of the value to insert or update.</param>
    /// <param name="value">The value to be added or updated.</param>
    /// <returns>true if the value was added or updated successfully; otherwise, false.</returns>
    public bool TrySet(TKey key, TValue? value)
    {
        return _cache.TryUpdate(key, new Lazy<TValue?>(() => value, true), _cache[key]);
    }

    /// <summary>
    /// Gets all the values in the cache.
    /// </summary>
    /// <returns>A collection of values in the cache.</returns>
    public ICollection<TValue?> Values() => _cache.Values.Select(x => x.Value).ToList();
}
