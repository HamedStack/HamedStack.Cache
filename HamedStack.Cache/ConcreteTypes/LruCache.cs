// ReSharper disable UnusedMember.Global

namespace HamedStack.Cache.ConcreteTypes;

/// <summary>
/// Represents a least recently used (LRU) cache.
/// </summary>
/// <typeparam name="TKey">The type of the keys in the cache. This type parameter is constrained to be non-nullable.</typeparam>
/// <typeparam name="TValue">The type of the values in the cache.</typeparam>
public class LruCache<TKey, TValue> where TKey : notnull
{
    private readonly Dictionary<TKey, LinkedListNode<LruCacheItem>> _cacheMap = new();

    private readonly Action<TValue?>? _dispose;

    private readonly LinkedList<LruCacheItem> _lruList = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="LruCache{TKey, TValue}"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of items the cache can hold.</param>
    /// <param name="dispose">An optional action to perform on items removed from the cache.</param>
    public LruCache(int capacity, Action<TValue?>? dispose = null)
    {
        Capacity = capacity;
        _dispose = dispose;
    }

    /// <summary>
    /// Gets the maximum number of items that the cache can hold.
    /// </summary>
    public int Capacity { get; }

    /// <summary>
    /// Retrieves the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the value to get.</param>
    /// <returns>The value associated with the specified key. If the key is not found, returns the default value for type <typeparamref name="TValue"/>.</returns>
    public TValue? Get(TKey key)
    {
        lock (_cacheMap)
        {
            var status = TryGetValue(key, out var value);
            return status ? value : default;
        }
    }

    /// <summary>
    /// Retrieves the value associated with the specified key. If the key is not present, uses the provided value generator to produce and store the value.
    /// </summary>
    /// <param name="key">The key of the value to get.</param>
    /// <param name="valueGenerator">A function to generate a value for the key if it doesn't exist in the cache.</param>
    /// <returns>The value associated with the specified key or the new value if the key was not present.</returns>
    public TValue? Get(TKey key, Func<TValue> valueGenerator)
    {
        lock (_cacheMap)
        {
            TValue? value;
            if (_cacheMap.TryGetValue(key, out var node))
            {
                value = node.Value.Value;
                _lruList.Remove(node);
                _lruList.AddLast(node);
            }
            else
            {
                value = valueGenerator();
                if (_cacheMap.Count >= Capacity)
                {
                    RemoveFirst();
                }

                var cacheItem = new LruCacheItem(key, value);
                node = new LinkedListNode<LruCacheItem>(cacheItem);
                _lruList.AddLast(node);
                _cacheMap.Add(key, node);
            }

            return value;
        }
    }

    /// <summary>
    /// Inserts or updates a value in the cache with the provided key.
    /// </summary>
    /// <param name="key">The key of the value to set.</param>
    /// <param name="value">The value to set for the specified key.</param>
    public void Set(TKey key, TValue value)
    {
        lock (_cacheMap)
        {
            if (_cacheMap.Count >= Capacity)
            {
                RemoveFirst();
            }

            var cacheItem = new LruCacheItem(key, value);
            var node =
                new LinkedListNode<LruCacheItem>(cacheItem);
            _lruList.AddLast(node);
            _cacheMap.Add(key, node);
        }
    }

    /// <summary>
    /// Tries to get the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the value to get.</param>
    /// <param name="value">When this method returns, contains the value associated with the specified key, if the key is found; otherwise, the default value for type <typeparamref name="TValue"/>.</param>
    /// <returns><c>true</c> if the cache contains an element with the specified key; otherwise, <c>false</c>.</returns>
    public bool TryGetValue(TKey key, out TValue? value)
    {
        lock (_cacheMap)
        {
            if (_cacheMap.TryGetValue(key, out var node))
            {
                value = node.Value.Value;
                _lruList.Remove(node);
                _lruList.AddLast(node);
                return true;
            }

            value = default;
            return false;
        }
    }
    /// <summary>
    /// Removes the least recently used item from the cache.
    /// </summary>
    private void RemoveFirst()
    {
        var node = _lruList.First;
        _lruList.RemoveFirst();
        if (node == null) return;
        _cacheMap.Remove(node.Value.Key);
        _dispose?.Invoke(node.Value.Value);
    }

    /// <summary>
    /// Represents an item in the LRU cache.
    /// </summary>
    private class LruCacheItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LruCacheItem"/> class.
        /// </summary>
        /// <param name="k">The key of the item.</param>
        /// <param name="v">The value of the item.</param>
        public LruCacheItem(TKey k, TValue? v)
        {
            Key = k;
            Value = v;
        }

        /// <summary>
        /// Gets the key of the item.
        /// </summary>
        public TKey Key { get; }

        /// <summary>
        /// Gets the value of the item.
        /// </summary>
        public TValue? Value { get; }
    }
}





