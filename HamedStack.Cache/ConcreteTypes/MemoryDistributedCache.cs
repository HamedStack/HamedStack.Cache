// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace HamedStack.Cache.ConcreteTypes;

/// <summary>
/// An implementation of <see cref="IDistributedCache"/> that uses an underlying <see cref="IMemoryCache"/>
/// for in-memory caching and supports serialization and deserialization of objects.
/// </summary>
public class MemoryDistributedCache : IDistributedCache
{
    private readonly IMemoryCache _memoryCache;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryDistributedCache"/> class.
    /// </summary>
    /// <param name="memoryCache">The underlying <see cref="IMemoryCache"/> instance to use for caching.</param>
    /// <param name="jsonOptions">Optional JSON serialization options. If not provided, default options will be used.</param>
    public MemoryDistributedCache(IMemoryCache memoryCache, JsonSerializerOptions? jsonOptions = null)
    {
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _jsonOptions = jsonOptions ?? new JsonSerializerOptions();
    }
    /// <inheritdoc/>
    public byte[]? Get(string key)
    {
        return _memoryCache.TryGetValue(key, out byte[]? value) ? value : null;
    }
    /// <inheritdoc/>
    public Task<byte[]?> GetAsync(string key, CancellationToken token = new())
    {
        return Task.FromResult(Get(key));
    }
    /// <inheritdoc/>
    public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options,
        CancellationToken token = new())
    {
        Set(key, value, options);
        return Task.CompletedTask;
    }
    /// <inheritdoc/>
    public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        var cacheEntryOptions = new MemoryCacheEntryOptions();

        if (options.AbsoluteExpirationRelativeToNow.HasValue)
        {
            cacheEntryOptions.AbsoluteExpirationRelativeToNow = options.AbsoluteExpirationRelativeToNow;
        }
        else if (options.AbsoluteExpiration.HasValue)
        {
            cacheEntryOptions.AbsoluteExpiration = options.AbsoluteExpiration.Value;
        }
        else if (options.SlidingExpiration.HasValue)
        {
            cacheEntryOptions.SlidingExpiration = options.SlidingExpiration.Value;
        }

        _memoryCache.Set(key, value, cacheEntryOptions);
    }
    
    /// <summary>
    /// Gets the cached value associated with the specified key and deserializes it to an object of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize.</typeparam>
    /// <param name="key">The key of the cached item.</param>
    /// <returns>
    /// An object of type <typeparamref name="T"/> if the key was found in the cache; otherwise, <c>null</c>.
    /// </returns>
    public T? Get<T>(string key)
    {
        var value = Get(key);
        return JsonSerializer.Deserialize<T>(value, _jsonOptions);
    }

    /// <summary>
    /// Sets the cached value associated with the specified key and serializes the provided object of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize and cache.</typeparam>
    /// <param name="key">The key to associate with the cached item.</param>
    /// <param name="value">The object to serialize and cache.</param>
    /// <param name="options">The cache entry options for the item.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <c>null</c>.</exception>
    public void Set<T>(string key, T value, DistributedCacheEntryOptions options)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        var serializedValue = JsonSerializer.Serialize(value, _jsonOptions);
        Set(key, Encoding.UTF8.GetBytes(serializedValue),options);
    }
    /// <inheritdoc/>
    public void Refresh(string key)
    {
        // No need to refresh in MemoryCache
    }
    /// <inheritdoc/>
    public Task RefreshAsync(string key, CancellationToken token = new())
    {
        // No need to refresh async in MemoryCache
        return Task.CompletedTask;
    }
    /// <inheritdoc/>
    public void Remove(string key)
    {
        _memoryCache.Remove(key);
    }
    /// <inheritdoc/>
    public Task RemoveAsync(string key, CancellationToken token = new())
    {
        Remove(key);
        return Task.CompletedTask;

    }
}