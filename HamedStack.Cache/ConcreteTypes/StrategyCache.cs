// ReSharper disable UnusedMember.Global

using System.Collections;
using HamedStack.Cache.Abstractions;

namespace HamedStack.Cache.ConcreteTypes;

/// <summary>
/// Represents a cache system that is driven by a specific caching strategy.
/// </summary>
/// <typeparam name="TKey">The type of the keys used to identify cached items.</typeparam>
/// <typeparam name="TValue">The type of the values stored in the cache.</typeparam>
public class StrategyCache<TKey, TValue> : ICache<TKey, TValue> where TKey : notnull
{
    private readonly ICacheStrategy<TKey, TValue> _strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="StrategyCache{TKey, TValue}"/> class with a specified caching strategy.
    /// </summary>
    /// <param name="strategy">The caching strategy to use.</param>
    public StrategyCache(ICacheStrategy<TKey, TValue> strategy)
    {
        _strategy = strategy;
    }

    /// <summary>
    /// Gets or sets the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the item to get or set.</param>
    /// <returns>The value associated with the specified key.</returns>
    public TValue? this[TKey key]
    {
        get => _strategy.Contains(key) ? _strategy.Get(key) : default;
        set => _strategy.Set(key, value, null);
    }

    /// <summary>
    /// Checks if the cache contains an item with the specified key.
    /// </summary>
    /// <param name="key">The key to check.</param>
    /// <returns><c>true</c> if the cache contains an item with the key; otherwise, <c>false</c>.</returns>
    public bool Contains(TKey key) => _strategy.Contains(key);

    /// <summary>
    /// Retrieves the number of entries in the cache. By default, this operation is not supported for this cache type.
    /// Override this method in a subclass if a specific caching strategy supports this operation.
    /// </summary>
    /// <returns>The total number of entries in the cache.</returns>
    /// <exception cref="NotSupportedException">Thrown when trying to retrieve the count, unless overridden in a subclass.</exception>
    public virtual int Count() => throw new NotSupportedException();

    /// <summary>
    /// Retrieves the value from the cache for the given key.
    /// </summary>
    /// <param name="key">The key whose value to retrieve.</param>
    /// <returns>The value associated with the key if found; otherwise, the default value.</returns>
    public TValue? Get(TKey key) => _strategy.Get(key);

    /// <summary>
    /// Retrieves the value for the given key or sets it using the provided delegate.
    /// </summary>
    /// <param name="key">The key to retrieve or set.</param>
    /// <param name="value">A delegate that produces the value for the key.</param>
    /// <returns>The existing or new value for the key.</returns>
    public TValue? GetOrSet(TKey key, Func<TValue> value)
    {
        if (!_strategy.Contains(key))
        {
            _strategy.Set(key, value(), null);
        }
        return _strategy.Get(key);
    }

    /// <summary>
    /// Retrieves the value for the given key or sets it using the provided delegate that accepts the key.
    /// </summary>
    /// <param name="key">The key to retrieve or set.</param>
    /// <param name="value">A delegate that produces the value for the key based on the key itself.</param>
    /// <returns>The existing or new value for the key.</returns>
    public TValue? GetOrSet(TKey key, Func<TKey, TValue> value)
    {
        if (!_strategy.Contains(key))
        {
            _strategy.Set(key, value(key), null);
        }
        return _strategy.Get(key);
    }

    /// <summary>
    /// Retrieves the value for the given key or sets it to the provided value.
    /// </summary>
    /// <param name="key">The key to retrieve or set.</param>
    /// <param name="value">The value to set if the key doesn't exist.</param>
    /// <returns>The existing or new value for the key.</returns>
    public TValue? GetOrSet(TKey key, TValue? value)
    {
        if (!_strategy.Contains(key))
        {
            _strategy.Set(key, value, null);
        }
        return _strategy.Get(key);
    }

    /// <summary>
    /// Removes the item with the specified key from the cache.
    /// </summary>
    /// <param name="key">The key of the item to remove.</param>
    /// <returns><c>true</c> if the item was successfully removed; otherwise, <c>false</c>.</returns>
    public bool Remove(TKey key) => _strategy.Remove(key);

    /// <summary>
    /// Sets the value for the given key in the cache.
    /// </summary>
    /// <param name="key">The key of the item to set.</param>
    /// <param name="value">The value to set for the key.</param>
    public void Set(TKey key, TValue? value) => _strategy.Set(key, value, null);

    /// <summary>
    /// Attempts to retrieve the value for a given key.
    /// </summary>
    /// <param name="key">The key to retrieve.</param>
    /// <param name="hasKey">Out parameter indicating if the key exists in the cache.</param>
    /// <returns>The value for the key if found; otherwise, the default value of TValue.</returns>
    public TValue? TryGet(TKey key, out bool hasKey)
    {
        hasKey = _strategy.Contains(key);
        return hasKey ? _strategy.Get(key) : default;
    }

    /// <summary>
    /// Attempts to remove the item with the given key from the cache.
    /// </summary>
    /// <param name="key">The key of the item to remove.</param>
    /// <param name="value">Out parameter for the value of the removed item.</param>
    /// <returns><c>true</c> if the item was successfully removed; otherwise, <c>false</c>.</returns>
    public bool TryRemove(TKey key, out TValue? value)
    {
        value = _strategy.Get(key);
        return _strategy.Remove(key);
    }

    /// <summary>
    /// Attempts to set the value for a given key in the cache.
    /// </summary>
    /// <param name="key">The key of the item to set.</param>
    /// <param name="value">The value to set for the key.</param>
    /// <returns><c>true</c> if the value was successfully set; otherwise, <c>false</c>.</returns>
    public bool TrySet(TKey key, TValue? value)
    {
        try
        {
            _strategy.Set(key, value, null);
            return true;
        }
        catch
        {
            return false;
        }
    }
    /// <summary>
    /// Retrieves all the keys in the cache. By default, this operation is not supported for this cache type.
    /// Override this method in a subclass if a specific caching strategy supports this operation.
    /// </summary>
    /// <returns>An <see cref="IEnumerable{TKey}"/> of keys in the cache.</returns>
    /// <exception cref="NotSupportedException">Thrown when trying to retrieve the keys, unless overridden in a subclass.</exception>
    public virtual IEnumerable<TKey> Keys() => throw new NotSupportedException();

    /// <summary>
    /// Removes all items from the cache. By default, this operation is not supported for this cache type.
    /// Override this method in a subclass if a specific caching strategy supports this operation.
    /// </summary>
    /// <exception cref="NotSupportedException">Thrown when trying to remove all items, unless overridden in a subclass.</exception>
    public virtual void RemoveAll() => throw new NotSupportedException();

    /// <summary>
    /// Retrieves all the values in the cache. By default, this operation is not supported for this cache type.
    /// Override this method in a subclass if a specific caching strategy supports this operation.
    /// </summary>
    /// <returns>An <see cref="ICollection{TValue}"/> of values in the cache.</returns>
    /// <exception cref="NotSupportedException">Thrown when trying to retrieve the values, unless overridden in a subclass.</exception>
    public virtual ICollection<TValue?> Values() => throw new NotSupportedException();

    /// <summary>
    /// Returns an enumerator that iterates through the cache. By default, this operation is not supported for this cache type.
    /// Override this method in a subclass if a specific caching strategy supports this operation.
    /// </summary>
    /// <returns>An enumerator for the cache.</returns>
    /// <exception cref="NotSupportedException">Thrown when trying to get the enumerator, unless overridden in a subclass.</exception>
    public virtual IEnumerator GetEnumerator() => throw new NotSupportedException();

    /// <summary>
    /// Gets a dictionary representation of the cache. By default, this operation is not supported for this cache type.
    /// Override this method in a subclass if a specific caching strategy supports this operation.
    /// </summary>
    /// <value>A dictionary containing the key-value pairs of the cache.</value>
    /// <exception cref="NotSupportedException">Thrown when trying to get the dictionary representation, unless overridden in a subclass.</exception>
    public virtual Dictionary<TKey, TValue?> CacheData => throw new NotSupportedException();

}