// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global
// ReSharper disable UnusedType.Global
// ReSharper disable CheckNamespace

using System.Runtime.Caching;
using Microsoft.Extensions.Caching.Memory;

namespace HamedStack.Cache.Extensions.MemoryCacheExtended;

/// <summary>
/// Provides extension methods for working with <see cref="MemoryCache"/>.
/// </summary>
public static class MemoryCacheExtensions
{
    /// <summary>
    /// Adds a cache entry into the cache using the specified key and value. If the key already exists, returns the existing value.
    /// </summary>
    /// <typeparam name="TValue">The type of the cached value.</typeparam>
    /// <param name="cache">The memory cache instance.</param>
    /// <param name="key">The unique identifier for the cache entry.</param>
    /// <param name="value">The value to be cached.</param>
    /// <returns>The cached value associated with the specified key.</returns>
    public static TValue? AddOrGet<TValue>(this MemoryCache cache, string key, TValue? value)
    {
        if (value == null) return value;

        var item = cache.AddOrGetExisting(key, value, new CacheItemPolicy());
        return (TValue?)item;
    }

    /// <summary>
    /// Adds a cache entry into the cache using the specified key and a value factory. If the key already exists, returns the existing value.
    /// </summary>
    /// <typeparam name="TValue">The type of the cached value.</typeparam>
    /// <param name="cache">The memory cache instance.</param>
    /// <param name="key">The unique identifier for the cache entry.</param>
    /// <param name="valueFactory">The function to produce the cached value.</param>
    /// <returns>The cached value associated with the specified key.</returns>
    public static TValue AddOrGet<TValue>(this MemoryCache cache, string key, Func<string, TValue> valueFactory)
    {
        var lazy = new Lazy<TValue>(() => valueFactory(key));

        var item = (Lazy<TValue>)cache.AddOrGetExisting(key, lazy, new CacheItemPolicy());

        return item.Value;
    }

    /// <summary>
    /// Adds a cache entry into the cache or retrieves an existing one with the specified key, value factory, policy, and optionally, region name.
    /// </summary>
    /// <typeparam name="TValue">The type of the cached value.</typeparam>
    /// <param name="cache">The memory cache instance to which this method is extending.</param>
    /// <param name="key">The unique identifier for the cache entry.</param>
    /// <param name="valueFactory">A function to produce the cached value. The function takes a string argument, which is the cache key, and returns the cached value.</param>
    /// <param name="policy">The policy to be applied when adding the cache entry.</param>
    /// <param name="regionName">Optional. Specifies the region in which to add or get the cache entry. If this is null, the default region is used.</param>
    /// <returns>The cached value associated with the specified key. If the key is not found, a new entry is added using the value factory, and its value is returned.</returns>
    public static TValue AddOrGet<TValue>(this MemoryCache cache, string key, Func<string, TValue> valueFactory, CacheItemPolicy policy, string? regionName = null)
    {
        var lazy = new Lazy<TValue>(() => valueFactory(key));
        switch (regionName)
        {
            case null:
                var result = (Lazy<TValue>)cache.AddOrGetExisting(key, lazy, policy);
                return result.Value;
            default:
                var item = (Lazy<TValue>)cache.AddOrGetExisting(key, lazy, policy, regionName);
                return item.Value;
        }
    }

    /// <summary>
    /// Adds or retrieves a cache entry with the specified key, value factory, expiration time, and region name.
    /// </summary>
    /// <typeparam name="TValue">The type of the cached value.</typeparam>
    /// <param name="cache">The memory cache instance.</param>
    /// <param name="key">The unique identifier for the cache entry.</param>
    /// <param name="valueFactory">The function to produce the cached value.</param>
    /// <param name="absoluteExpiration">The time at which the cache entry should expire.</param>
    /// <param name="regionName">The region where the cache entry should be stored.</param>
    /// <returns>The cached value associated with the specified key.</returns>
    public static TValue AddOrGet<TValue>(this MemoryCache cache, string key, Func<string, TValue> valueFactory, DateTimeOffset absoluteExpiration, string? regionName = null)
    {
        var lazy = new Lazy<TValue>(() => valueFactory(key));

        switch (regionName)
        {
            case null:
                var result = (Lazy<TValue>)cache.AddOrGetExisting(key, lazy, absoluteExpiration);
                return result.Value;
            default:
                var item = (Lazy<TValue>)cache.AddOrGetExisting(key, lazy, absoluteExpiration, regionName);
                return item.Value;
        }
    }

    /// <summary>
    /// Safely retrieves a cache entry with the specified key or sets it using the provided function if not present.
    /// Throws an exception if the cached value is null.
    /// </summary>
    /// <typeparam name="TReturn">The type of the cached value.</typeparam>
    /// <param name="memoryCache">The memory cache instance.</param>
    /// <param name="key">The unique identifier for the cache entry.</param>
    /// <param name="objectToCache">The function producing the cached value.</param>
    /// <returns>The cached value associated with the specified key.</returns>
    public static TReturn? SafeGet<TReturn>(this MemoryCache memoryCache, string key, Func<TReturn> objectToCache)
    {
        if (memoryCache.Contains(key))
            return (TReturn)memoryCache[key];

        return (TReturn?)(memoryCache[key] = objectToCache() ?? throw new InvalidOperationException($"The objectToCache function for key '{key}' returned null, which is not expected."));
    }

    /// <summary>
    /// Safely retrieves a cache entry with the specified key or sets it using the provided function if not present.
    /// Throws an exception if the cached value is null.
    /// </summary>
    /// <typeparam name="TReturn">The type of the cached value.</typeparam>
    /// <param name="memoryCache">The memory cache instance.</param>
    /// <param name="key">The unique identifier for the cache entry.</param>
    /// <param name="objectToCache">The value to be cached.</param>
    /// <returns>The cached value associated with the specified key.</returns>
    public static TReturn? SafeGet<TReturn>(this MemoryCache memoryCache, string key, TReturn objectToCache)
    {
        if (memoryCache.Contains(key))
            return (TReturn)memoryCache[key];

        return (TReturn?)(memoryCache[key] = objectToCache ??
                                              throw new InvalidOperationException(
                                                  $"The objectToCache for key '{key}' returned null, which is not expected."));
    }

    /// <summary>
    /// Safely retrieves a cache entry with the specified key or sets it using the provided function if not present.
    /// </summary>
    /// <param name="memoryCache">The memory cache instance.</param>
    /// <param name="key">The unique identifier for the cache entry.</param>
    /// <param name="objectToCache">The function producing the cached value.</param>
    /// <returns>The cached value associated with the specified key.</returns>
    public static object SafeGet(this MemoryCache memoryCache, string key, Func<object> objectToCache)
    {
        if (memoryCache.Contains(key))
            return memoryCache[key];

        return memoryCache[key] = objectToCache();
    }

    /// <summary>
    /// Safely retrieves a cache entry with the specified key or sets it if not present.
    /// </summary>
    /// <param name="memoryCache">The memory cache instance.</param>
    /// <param name="key">The unique identifier for the cache entry.</param>
    /// <param name="objectToCache">The value to be cached.</param>
    /// <returns>The cached value associated with the specified key.</returns>
    public static object SafeGet(this MemoryCache memoryCache, string key, object objectToCache)
    {
        if (memoryCache.Contains(key))
            return memoryCache[key];

        return memoryCache[key] = objectToCache;
    }

    /// <summary>
    /// Safely retrieves a cache entry with the specified key or sets it using the provided function if not present.
    /// </summary>
    /// <typeparam name="TReturn">The type of the cached value.</typeparam>
    /// <param name="memoryCache">The memory cache instance.</param>
    /// <param name="key">The unique identifier for the cache entry.</param>
    /// <param name="objectToCache">The function producing the cached value.</param>
    /// <returns>The cached value associated with the specified key.</returns>
    public static TReturn? SafeGet<TReturn>(this IMemoryCache memoryCache, string key, Func<TReturn> objectToCache)
    {
        if (memoryCache.TryGetValue(key, out TReturn? cached))
        {
            return cached;
        }

        var data = objectToCache();
        memoryCache.Set(key, data);
        return data;
    }

    /// <summary>
    /// Safely retrieves a cache entry with the specified key or sets it if not present.
    /// </summary>
    /// <typeparam name="TReturn">The type of the cached value.</typeparam>
    /// <param name="memoryCache">The memory cache instance.</param>
    /// <param name="key">The unique identifier for the cache entry.</param>
    /// <param name="objectToCache">The value to be cached.</param>
    /// <returns>The cached value associated with the specified key.</returns>
    public static TReturn? SafeGet<TReturn>(this IMemoryCache memoryCache, string key, TReturn objectToCache)
    {
        if (memoryCache.TryGetValue(key, out TReturn? cached))
        {
            return cached;
        }

        memoryCache.Set(key, objectToCache);
        return objectToCache;
    }

    /// <summary>
    /// Safely retrieves a cache entry with the specified key or sets it using the provided function if not present.
    /// </summary>
    /// <param name="memoryCache">The memory cache instance.</param>
    /// <param name="key">The unique identifier for the cache entry.</param>
    /// <param name="objectToCache">The function producing the cached value.</param>
    /// <returns>The cached value associated with the specified key.</returns>
    public static object? SafeGet(this IMemoryCache memoryCache, string key, Func<object> objectToCache)
    {

        if (memoryCache.TryGetValue(key, out var cached))
        {
            return cached;
        }

        var data = objectToCache();
        memoryCache.Set(key, data);
        return data;

    }

    /// <summary>
    /// Safely retrieves a cache entry with the specified key or sets it if not present.
    /// </summary>
    /// <param name="memoryCache">The memory cache instance.</param>
    /// <param name="key">The unique identifier for the cache entry.</param>
    /// <param name="objectToCache">The value to be cached.</param>
    /// <returns>The cached value associated with the specified key.</returns>
    public static object? SafeGet(this IMemoryCache memoryCache, string key, object objectToCache)
    {
        if (memoryCache.TryGetValue(key, out var cached))
        {
            return cached;
        }

        memoryCache.Set(key, objectToCache);
        return objectToCache;
    }
}