// ReSharper disable UnusedMember.Global
// ReSharper disable CheckNamespace
// ReSharper disable UnusedType.Global

using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace HamedStack.Cache.Extensions.DistributedCacheExtended;

/// <summary>
/// Provides extension methods for <see cref="IDistributedCache"/> to support storing and retrieving complex objects.
/// </summary>
public static class DistributedCacheExtensions
{
    /// <summary>
    /// Serializes and sets an object in the cache using a typed key.
    /// </summary>
    /// <typeparam name="TKey">The type of the cache key.</typeparam>
    /// <typeparam name="TValue">The type of the cache value.</typeparam>
    /// <param name="cache">The distributed cache instance.</param>
    /// <param name="key">The key for the cache entry.</param>
    /// <param name="value">The object to store in the cache.</param>
    /// <param name="options">Optional serializer settings.</param>
    /// <param name="entryOptions">Optional cache entry options.</param>
    public static void SetObject<TKey, TValue>(this IDistributedCache cache, TKey key, TValue value, JsonSerializerOptions? options = null, DistributedCacheEntryOptions? entryOptions = null) where TKey : notnull
    {
        try
        {
            var jsonKey = JsonSerializer.Serialize(key, options);
            var jsonData = JsonSerializer.Serialize(value, options);
            if (entryOptions == null)
                cache.SetString(jsonKey, jsonData);
            else
                cache.SetString(jsonKey, jsonData, entryOptions);
        }
        catch (Exception e)
        {
            throw new Exception("Failed to serialize and write to cache.", e);
        }
    }

    /// <summary>
    /// Gets the cached object associated with the specified key from the distributed cache. If the object does not exist,
    /// sets the object in the cache and returns it.
    /// </summary>
    /// <typeparam name="TKey">The type of the cache key.</typeparam>
    /// <typeparam name="TValue">The type of the cached object.</typeparam>
    /// <param name="cache">The distributed cache instance.</param>
    /// <param name="key">The key of the object to retrieve or set.</param>
    /// <param name="value">The value to set in the cache if the object does not exist.</param>
    /// <param name="options">The options used to serialize and deserialize the cached object.</param>
    /// <param name="entryOptions">The options used when adding the object to the cache.</param>
    /// <returns>The cached object associated with the key, or the newly set object if it does not exist.</returns>
    /// <remarks>
    /// This method synchronously retrieves the object from the cache using the specified key. If the object does not
    /// exist, it is set in the cache using the provided value. The method returns either the cached object or the newly
    /// set object, depending on whether it existed in the cache prior to this method call.
    /// </remarks>
    public static TValue GetOrSetObject<TKey, TValue>(this IDistributedCache cache, TKey key, TValue value,
        JsonSerializerOptions? options = null, DistributedCacheEntryOptions? entryOptions = null) where TKey : notnull
    {
        var cachedData = cache.GetObject<TKey, TValue>(key, options);
        if (cachedData != null)
        {
            return cachedData;
        }

        cache.SetObject(key, value, options, entryOptions);

        return value;
    }

    /// <summary>
    /// Gets the cached object associated with the specified key from the distributed cache. If the object does not exist,
    /// sets the object in the cache and returns it.
    /// </summary>
    /// <typeparam name="TValue">The type of the cached object.</typeparam>
    /// <param name="cache">The distributed cache instance.</param>
    /// <param name="key">The key of the object to retrieve or set.</param>
    /// <param name="value">The value to set in the cache if the object does not exist.</param>
    /// <param name="options">The options used to serialize and deserialize the cached object.</param>
    /// <param name="entryOptions">The options used when adding the object to the cache.</param>
    /// <returns>The cached object associated with the key, or the newly set object if it does not exist.</returns>
    /// <remarks>
    /// This method synchronously retrieves the object from the cache using the specified key. If the object does not
    /// exist, it is set in the cache using the provided value. The method returns either the cached object or the newly
    /// set object, depending on whether it existed in the cache prior to this method call.
    /// </remarks>
    public static TValue GetOrSetObject<TValue>(this IDistributedCache cache, string key, TValue value,
        JsonSerializerOptions? options = null, DistributedCacheEntryOptions? entryOptions = null)
    {
        var cachedData = cache.GetObject<TValue>(key, options);
        if (cachedData != null)
        {
            return cachedData;
        }

        cache.SetObject(key, value, options, entryOptions);

        return value;
    }


    /// <summary>
    /// Retrieves an object from the cache using a typed key and deserializes it.
    /// </summary>
    /// <typeparam name="TKey">The type of the cache key.</typeparam>
    /// <typeparam name="TValue">The type of the cache value.</typeparam>
    /// <param name="cache">The distributed cache instance.</param>
    /// <param name="key">The key used in the cache lookup.</param>
    /// <param name="options">Optional serializer settings.</param>
    /// <returns>The deserialized object from the cache or default if not found.</returns>
    public static TValue? GetObject<TKey, TValue>(this IDistributedCache cache, TKey key, JsonSerializerOptions? options = null) where TKey : notnull
    {
        try
        {
            var jsonKey = JsonSerializer.Serialize(key, options);
            var jsonData = cache.GetString(jsonKey);
            return jsonData == null ? default : JsonSerializer.Deserialize<TValue>(jsonData, options);
        }
        catch (Exception e)
        {
            throw new Exception("Failed to read from cache and deserialize.", e);
        }
    }

    /// <summary>
    /// Asynchronously serializes and sets an object in the cache using a typed key.
    /// </summary>
    /// <typeparam name="TKey">The type of the cache key.</typeparam>
    /// <typeparam name="TValue">The type of the cache value.</typeparam>
    /// <param name="cache">The distributed cache instance.</param>
    /// <param name="key">The key for the cache entry.</param>
    /// <param name="value">The object to store in the cache.</param>
    /// <param name="options">Optional serializer settings.</param>
    /// <param name="entryOptions">Optional cache entry options.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    public static async Task SetObjectAsync<TKey, TValue>(this IDistributedCache cache, TKey key, TValue value, JsonSerializerOptions? options = null, DistributedCacheEntryOptions? entryOptions = null, CancellationToken cancellationToken = default) where TKey : notnull
    {
        try
        {
            var jsonKey = JsonSerializer.Serialize(key, options);
            var jsonData = JsonSerializer.Serialize(value, options);
            if (entryOptions == null)
                await cache.SetStringAsync(jsonKey, jsonData, cancellationToken);
            else
                await cache.SetStringAsync(jsonKey, jsonData, entryOptions, cancellationToken);
        }
        catch (Exception e)
        {
            throw new Exception("Failed to serialize and write to cache.", e);
        }
    }

    /// <summary>
    /// Gets the cached object associated with the specified key from the distributed cache. If the object does not exist,
    /// sets the object in the cache and returns it.
    /// </summary>
    /// <typeparam name="TKey">The type of the cache key.</typeparam>
    /// <typeparam name="TValue">The type of the cached object.</typeparam>
    /// <param name="cache">The distributed cache instance.</param>
    /// <param name="key">The key of the object to retrieve or set.</param>
    /// <param name="value">The value to set in the cache if the object does not exist.</param>
    /// <param name="options">The options used to serialize and deserialize the cached object.</param>
    /// <param name="entryOptions">The options used when adding the object to the cache.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The cached object associated with the key, or the newly set object if it does not exist.</returns>
    /// <remarks>
    /// This method asynchronously retrieves the object from the cache using the specified key. If the object does not
    /// exist, it is set in the cache using the provided value. The method returns either the cached object or the newly
    /// set object, depending on whether it existed in the cache prior to this method call.
    /// </remarks>
    public static async Task<TValue?> GetOrSetObjectAsync<TKey, TValue>(this IDistributedCache cache, TKey key, TValue value,
        JsonSerializerOptions? options = null, DistributedCacheEntryOptions? entryOptions = null,
        CancellationToken cancellationToken = default) where TKey : notnull
    {
        var cachedData = await cache.GetObjectAsync<TKey, TValue>(key, options, cancellationToken);
        if (cachedData != null)
        {
            return cachedData;
        }

        await cache.SetObjectAsync(key, value, options, entryOptions, cancellationToken);

        return value;
    }

    /// <summary>
    /// Gets the cached object associated with the specified key from the distributed cache. If the object does not exist,
    /// sets the object in the cache and returns it.
    /// </summary>
    /// <typeparam name="TValue">The type of the cached object.</typeparam>
    /// <param name="cache">The distributed cache instance.</param>
    /// <param name="key">The key of the object to retrieve or set.</param>
    /// <param name="value">The value to set in the cache if the object does not exist.</param>
    /// <param name="options">The options used to serialize and deserialize the cached object.</param>
    /// <param name="entryOptions">The options used when adding the object to the cache.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The cached object associated with the key, or the newly set object if it does not exist.</returns>
    /// <remarks>
    /// This method asynchronously retrieves the object from the cache using the specified key. If the object does not
    /// exist, it is set in the cache using the provided value. The method returns either the cached object or the newly
    /// set object, depending on whether it existed in the cache prior to this method call.
    /// </remarks>
    public static async Task<TValue?> GetOrSetObjectAsync<TValue>(this IDistributedCache cache, string key, TValue value,
        JsonSerializerOptions? options = null, DistributedCacheEntryOptions? entryOptions = null,
        CancellationToken cancellationToken = default)
    {
        var cachedData = await cache.GetObjectAsync<TValue>(key, options, cancellationToken);
        if (cachedData != null)
        {
            return cachedData;
        }

        await cache.SetObjectAsync(key, value, options, entryOptions, cancellationToken);

        return value;
    }


    /// <summary>
    /// Asynchronously retrieves an object from the cache using a typed key and deserializes it.
    /// </summary>
    /// <typeparam name="TKey">The type of the cache key.</typeparam>
    /// <typeparam name="TValue">The type of the cache value.</typeparam>
    /// <param name="cache">The distributed cache instance.</param>
    /// <param name="key">The key used in the cache lookup.</param>
    /// <param name="options">Optional serializer settings.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>The deserialized object from the cache or default if not found.</returns>
    public static async Task<TValue?> GetObjectAsync<TKey, TValue>(this IDistributedCache cache, TKey key, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default) where TKey : notnull
    {
        try
        {
            var jsonKey = JsonSerializer.Serialize(key, options);
            var jsonData = await cache.GetStringAsync(jsonKey, cancellationToken);

            return jsonData == null ? default : JsonSerializer.Deserialize<TValue>(jsonData, options);
        }
        catch (Exception e)
        {
            throw new Exception("Failed to read from cache and deserialize.", e);
        }
    }

    /// <summary>
    /// Serializes and sets an object in the cache using a string key.
    /// </summary>
    /// <typeparam name="T">The type of the cache value.</typeparam>
    /// <param name="cache">The distributed cache instance.</param>
    /// <param name="key">The key for the cache entry.</param>
    /// <param name="value">The object to store in the cache.</param>
    /// <param name="options">Optional serializer settings.</param>
    /// <param name="entryOptions">Optional cache entry options.</param>
    public static void SetObject<T>(this IDistributedCache cache, string key, T value, JsonSerializerOptions? options = null, DistributedCacheEntryOptions? entryOptions = null)
    {
        try
        {
            var jsonData = JsonSerializer.Serialize(value, options);
            if (entryOptions == null)
                cache.SetString(key, jsonData);
            else
                cache.SetString(key, jsonData, entryOptions);
        }
        catch (Exception e)
        {
            throw new Exception("Failed to serialize and write to cache.", e);
        }
    }

    /// <summary>
    /// Retrieves an object from the cache using a string key and deserializes it.
    /// </summary>
    /// <typeparam name="T">The type of the cache value.</typeparam>
    /// <param name="cache">The distributed cache instance.</param>
    /// <param name="key">The key used in the cache lookup.</param>
    /// <param name="options">Optional serializer settings.</param>
    /// <returns>The deserialized object from the cache or default if not found.</returns>
    public static T? GetObject<T>(this IDistributedCache cache, string key, JsonSerializerOptions? options = null)
    {
        try
        {
            var jsonData = cache.GetString(key);
            return jsonData == null ? default : JsonSerializer.Deserialize<T>(jsonData, options);
        }
        catch (Exception e)
        {
            throw new Exception("Failed to read from cache and deserialize.", e);
        }
    }

    /// <summary>
    /// Asynchronously serializes and sets an object in the cache using a string key.
    /// </summary>
    /// <typeparam name="T">The type of the cache value.</typeparam>
    /// <param name="cache">The distributed cache instance.</param>
    /// <param name="key">The key for the cache entry.</param>
    /// <param name="value">The object to store in the cache.</param>
    /// <param name="options">Optional serializer settings.</param>
    /// <param name="entryOptions">Optional cache entry options.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    public static async Task SetObjectAsync<T>(this IDistributedCache cache, string key, T value, JsonSerializerOptions? options = null, DistributedCacheEntryOptions? entryOptions = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var jsonData = JsonSerializer.Serialize(value, options);
            if (entryOptions == null)
                await cache.SetStringAsync(key, jsonData, token: cancellationToken);
            else
                await cache.SetStringAsync(key, jsonData, entryOptions, token: cancellationToken);
        }
        catch (Exception e)
        {
            throw new Exception("Failed to serialize and write to cache.", e);
        }
    }

    /// <summary>
    /// Asynchronously retrieves an object from the cache using a string key and deserializes it.
    /// </summary>
    /// <typeparam name="T">The type of the cache value.</typeparam>
    /// <param name="cache">The distributed cache instance.</param>
    /// <param name="key">The key used in the cache lookup.</param>
    /// <param name="options">Optional serializer settings.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>The deserialized object from the cache or default if not found.</returns>
    public static async Task<T?> GetObjectAsync<T>(this IDistributedCache cache, string key, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var jsonData = await cache.GetStringAsync(key, cancellationToken);

            return jsonData == null ? default : JsonSerializer.Deserialize<T>(jsonData, options);
        }
        catch (Exception e)
        {
            throw new Exception("Failed to read from cache and deserialize.", e);
        }
    }

    /// <summary>
    /// Retrieves a boolean value asynchronously from the cache.
    /// </summary>
    /// <param name="cache">The distributed cache instance.</param>
    /// <param name="key">The key used in the cache lookup.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A boolean from the cache or null if not found.</returns>
    public static async Task<bool?> GetBooleanAsync(this IDistributedCache cache, string key, CancellationToken cancellationToken = default)
    {
        if (cache == null)
        {
            throw new ArgumentNullException(nameof(cache));
        }
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }
        var bytes = await cache.GetAsync(key, cancellationToken).ConfigureAwait(false);
        if (bytes == null)
        {
            return null;
        }

        using var memoryStream = new MemoryStream(bytes);
        var binaryReader = new BinaryReader(memoryStream);
        return binaryReader.ReadBoolean();
    }

    /// <summary>
    /// Retrieves a char value asynchronously from the cache.
    /// </summary>
    /// <param name="cache">The distributed cache instance.</param>
    /// <param name="key">The key used in the cache lookup.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A char from the cache or null if not found.</returns>
    public static async Task<char?> GetCharAsync(this IDistributedCache cache, string key, CancellationToken cancellationToken = default)
    {
        if (cache == null)
        {
            throw new ArgumentNullException(nameof(cache));
        }
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }
        var bytes = await cache.GetAsync(key, cancellationToken).ConfigureAwait(false);
        if (bytes == null)
        {
            return null;
        }

        using var memoryStream = new MemoryStream(bytes);
        var binaryReader = new BinaryReader(memoryStream);
        return binaryReader.ReadChar();
    }

    /// <summary>
    /// Retrieves a decimal value asynchronously from the cache.
    /// </summary>
    /// <param name="cache">The distributed cache instance.</param>
    /// <param name="key">The key used in the cache lookup.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A decimal from the cache or null if not found.</returns>
    public static async Task<decimal?> GetDecimalAsync(this IDistributedCache cache, string key, CancellationToken cancellationToken = default)
    {
        if (cache == null)
        {
            throw new ArgumentNullException(nameof(cache));
        }
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }
        var bytes = await cache.GetAsync(key, cancellationToken).ConfigureAwait(false);
        if (bytes == null)
        {
            return null;
        }

        using var memoryStream = new MemoryStream(bytes);
        var binaryReader = new BinaryReader(memoryStream);
        return binaryReader.ReadDecimal();
    }

    /// <summary>
    /// Retrieves a double value from the distributed cache.
    /// </summary>
    /// <param name="cache">The distributed cache instance.</param>
    /// <param name="key">The key of the cached item.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>The double value if exists; otherwise, null.</returns>
    public static async Task<double?> GetDoubleAsync(this IDistributedCache cache, string key, CancellationToken cancellationToken = default)
    {
        if (cache == null)
        {
            throw new ArgumentNullException(nameof(cache));
        }
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }
        var bytes = await cache.GetAsync(key, cancellationToken).ConfigureAwait(false);
        if (bytes == null)
        {
            return null;
        }

        using var memoryStream = new MemoryStream(bytes);
        var binaryReader = new BinaryReader(memoryStream);
        return binaryReader.ReadDouble();
    }

    /// <summary>
    /// Retrieves a float value from the distributed cache.
    /// </summary>
    /// <param name="cache">The distributed cache instance.</param>
    /// <param name="key">The key of the cached item.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>The float value if exists; otherwise, null.</returns>
    public static async Task<float?> GetFloatAsync(this IDistributedCache cache, string key, CancellationToken cancellationToken = default)
    {
        if (cache == null)
        {
            throw new ArgumentNullException(nameof(cache));
        }
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }
        var bytes = await cache.GetAsync(key, cancellationToken).ConfigureAwait(false);
        if (bytes == null)
        {
            return null;
        }

        using var memoryStream = new MemoryStream(bytes);
        var binaryReader = new BinaryReader(memoryStream);
        return binaryReader.ReadSingle();
    }

    /// <summary>
    /// Retrieves an integer value from the distributed cache.
    /// </summary>
    /// <param name="cache">The distributed cache instance.</param>
    /// <param name="key">The key of the cached item.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>The integer value if exists; otherwise, null.</returns>
    public static async Task<int?> GetIntAsync(this IDistributedCache cache, string key, CancellationToken cancellationToken = default)
    {
        if (cache == null)
        {
            throw new ArgumentNullException(nameof(cache));
        }
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }
        var bytes = await cache.GetAsync(key, cancellationToken).ConfigureAwait(false);
        if (bytes == null)
        {
            return null;
        }

        using var memoryStream = new MemoryStream(bytes);
        var binaryReader = new BinaryReader(memoryStream);
        return binaryReader.ReadInt32();
    }

    /// <summary>
    /// Retrieves a long value from the distributed cache.
    /// </summary>
    /// <param name="cache">The distributed cache instance.</param>
    /// <param name="key">The key of the cached item.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>The long value if exists; otherwise, null.</returns>
    public static async Task<long?> GetLongAsync(this IDistributedCache cache, string key, CancellationToken cancellationToken = default)
    {
        if (cache == null)
        {
            throw new ArgumentNullException(nameof(cache));
        }
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }
        var bytes = await cache.GetAsync(key, cancellationToken).ConfigureAwait(false);
        if (bytes == null)
        {
            return null;
        }

        using var memoryStream = new MemoryStream(bytes);
        var binaryReader = new BinaryReader(memoryStream);
        return binaryReader.ReadInt64();
    }

    /// <summary>
    /// Retrieves a short value from the distributed cache.
    /// </summary>
    /// <param name="cache">The distributed cache instance.</param>
    /// <param name="key">The key of the cached item.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>The short value if exists; otherwise, null.</returns>
    public static async Task<short?> GetShortAsync(this IDistributedCache cache, string key, CancellationToken cancellationToken = default)
    {
        if (cache == null)
        {
            throw new ArgumentNullException(nameof(cache));
        }
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }
        var bytes = await cache.GetAsync(key, cancellationToken).ConfigureAwait(false);
        if (bytes == null)
        {
            return null;
        }

        using var memoryStream = new MemoryStream(bytes);
        var binaryReader = new BinaryReader(memoryStream);
        return binaryReader.ReadInt16();
    }

    /// <summary>
    /// Retrieves a string value from the distributed cache.
    /// </summary>
    /// <param name="cache">The distributed cache instance.</param>
    /// <param name="key">The key of the cached item.</param>
    /// <param name="encoding">Optional text encoding (defaults to UTF-8).</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>The string value if exists; otherwise, null.</returns>
    public static async Task<string?> GetStringAsync(this IDistributedCache cache, string key, Encoding? encoding = null, CancellationToken cancellationToken = default)
    {
        if (cache == null)
        {
            throw new ArgumentNullException(nameof(cache));
        }
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }
        encoding ??= Encoding.UTF8;
        var bytes = await cache.GetAsync(key, cancellationToken).ConfigureAwait(false);
        return bytes == null ? null : encoding.GetString(bytes);
    }

    /// <summary>
    /// Sets a boolean value in the distributed cache.
    /// </summary>
    /// <param name="cache">The distributed cache instance.</param>
    /// <param name="key">The key of the item to cache.</param>
    /// <param name="value">The boolean value to cache.</param>
    /// <param name="options">Optional cache entry options.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static Task SetAsync(
        this IDistributedCache cache,
        string key,
        bool value,
        DistributedCacheEntryOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (cache == null)
        {
            throw new ArgumentNullException(nameof(cache));
        }
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }
        options ??= new DistributedCacheEntryOptions();
        byte[] bytes;
        using (var memoryStream = new MemoryStream())
        {
            var binaryWriter = new BinaryWriter(memoryStream);
            binaryWriter.Write(value);
            bytes = memoryStream.ToArray();
        }
        return cache.SetAsync(key, bytes, options, cancellationToken);
    }

    /// <summary>
    /// Sets a char value in the distributed cache.
    /// </summary>
    /// <param name="cache">The distributed cache instance.</param>
    /// <param name="key">The key of the item to cache.</param>
    /// <param name="value">The char value to cache.</param>
    /// <param name="options">Optional cache entry options.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static Task SetAsync(
        this IDistributedCache cache,
        string key,
        char value,
        DistributedCacheEntryOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (cache == null)
        {
            throw new ArgumentNullException(nameof(cache));
        }
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }
        options ??= new DistributedCacheEntryOptions();
        byte[] bytes;
        using (var memoryStream = new MemoryStream())
        {
            var binaryWriter = new BinaryWriter(memoryStream);
            binaryWriter.Write(value);
            bytes = memoryStream.ToArray();
        }
        return cache.SetAsync(key, bytes, options, cancellationToken);
    }

    /// <summary>
    /// Sets a decimal value in the distributed cache.
    /// </summary>
    /// <param name="cache">The distributed cache instance.</param>
    /// <param name="key">The key of the item to cache.</param>
    /// <param name="value">The decimal value to cache.</param>
    /// <param name="options">Optional cache entry options.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static Task SetAsync(
        this IDistributedCache cache,
        string key,
        decimal value,
        DistributedCacheEntryOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (cache == null)
        {
            throw new ArgumentNullException(nameof(cache));
        }
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }
        options ??= new DistributedCacheEntryOptions();
        byte[] bytes;
        using (var memoryStream = new MemoryStream())
        {
            var binaryWriter = new BinaryWriter(memoryStream);
            binaryWriter.Write(value);
            bytes = memoryStream.ToArray();
        }
        return cache.SetAsync(key, bytes, options, cancellationToken);
    }

    /// <summary>
    /// Sets a double value in the distributed cache.
    /// </summary>
    /// <param name="cache">The distributed cache instance.</param>
    /// <param name="key">The key of the item to cache.</param>
    /// <param name="value">The double value to cache.</param>
    /// <param name="options">Optional cache entry options.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static Task SetAsync(
        this IDistributedCache cache,
        string key,
        double value,
        DistributedCacheEntryOptions? options, CancellationToken cancellationToken = default)
    {
        if (cache == null)
        {
            throw new ArgumentNullException(nameof(cache));
        }
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }
        options ??= new DistributedCacheEntryOptions();
        byte[] bytes;
        using (var memoryStream = new MemoryStream())
        {
            var binaryWriter = new BinaryWriter(memoryStream);
            binaryWriter.Write(value);
            bytes = memoryStream.ToArray();
        }
        return cache.SetAsync(key, bytes, options, cancellationToken);
    }

    /// <summary>
    /// Sets a short value in the distributed cache.
    /// </summary>
    /// <param name="cache">The distributed cache instance.</param>
    /// <param name="key">The key of the item to cache.</param>
    /// <param name="value">The short value to cache.</param>
    /// <param name="options">Optional cache entry options.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static Task SetAsync(
        this IDistributedCache cache,
        string key,
        short value,
        DistributedCacheEntryOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (cache == null)
        {
            throw new ArgumentNullException(nameof(cache));
        }
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }
        options ??= new DistributedCacheEntryOptions();
        byte[] bytes;
        using (var memoryStream = new MemoryStream())
        {
            var binaryWriter = new BinaryWriter(memoryStream);
            binaryWriter.Write(value);
            bytes = memoryStream.ToArray();
        }
        return cache.SetAsync(key, bytes, options, cancellationToken);
    }

    /// <summary>
    /// Sets an integer value in the distributed cache.
    /// </summary>
    /// <param name="cache">The distributed cache instance.</param>
    /// <param name="key">The key of the item to cache.</param>
    /// <param name="value">The integer value to cache.</param>
    /// <param name="options">Optional cache entry options.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static Task SetAsync(
        this IDistributedCache cache,
        string key,
        int value,
        DistributedCacheEntryOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (cache == null)
        {
            throw new ArgumentNullException(nameof(cache));
        }
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }
        options ??= new DistributedCacheEntryOptions();
        byte[] bytes;
        using (var memoryStream = new MemoryStream())
        {
            var binaryWriter = new BinaryWriter(memoryStream);
            binaryWriter.Write(value);
            bytes = memoryStream.ToArray();
        }
        return cache.SetAsync(key, bytes, options, cancellationToken);
    }

    /// <summary>
    /// Sets a long value in the distributed cache.
    /// </summary>
    /// <param name="cache">The distributed cache instance.</param>
    /// <param name="key">The key of the item to cache.</param>
    /// <param name="value">The long value to cache.</param>
    /// <param name="options">Optional cache entry options.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static Task SetAsync(
        this IDistributedCache cache,
        string key,
        long value,
        DistributedCacheEntryOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (cache == null)
        {
            throw new ArgumentNullException(nameof(cache));
        }
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }
        options ??= new DistributedCacheEntryOptions();
        byte[] bytes;
        using (var memoryStream = new MemoryStream())
        {
            var binaryWriter = new BinaryWriter(memoryStream);
            binaryWriter.Write(value);
            bytes = memoryStream.ToArray();
        }
        return cache.SetAsync(key, bytes, options, cancellationToken);
    }

    /// <summary>
    /// Sets a float value in the distributed cache.
    /// </summary>
    /// <param name="cache">The distributed cache instance.</param>
    /// <param name="key">The key of the item to cache.</param>
    /// <param name="value">The float value to cache.</param>
    /// <param name="options">Optional cache entry options.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static Task SetAsync(
        this IDistributedCache cache,
        string key,
        float value,
        DistributedCacheEntryOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (cache == null)
        {
            throw new ArgumentNullException(nameof(cache));
        }
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }
        options ??= new DistributedCacheEntryOptions();
        byte[] bytes;
        using (var memoryStream = new MemoryStream())
        {
            var binaryWriter = new BinaryWriter(memoryStream);
            binaryWriter.Write(value);
            bytes = memoryStream.ToArray();
        }
        return cache.SetAsync(key, bytes, options, cancellationToken);
    }

    /// <summary>
    /// Sets a string value in the distributed cache.
    /// </summary>
    /// <param name="cache">The distributed cache instance.</param>
    /// <param name="key">The key of the item to cache.</param>
    /// <param name="value">The string value to cache.</param>
    /// <param name="options">Optional cache entry options.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static Task SetAsync(
        this IDistributedCache cache,
        string key,
        string value,
        DistributedCacheEntryOptions? options = null, CancellationToken cancellationToken = default)
    {
        return SetAsync(cache, key, value, null, options, cancellationToken);
    }

    /// <summary>
    /// Sets a string value in the distributed cache with optional encoding.
    /// </summary>
    /// <param name="cache">The distributed cache instance.</param>
    /// <param name="key">The key of the item to cache.</param>
    /// <param name="value">The string value to cache.</param>
    /// <param name="encoding">Optional text encoding (defaults to UTF-8).</param>
    /// <param name="options">Optional cache entry options.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static Task SetAsync(
        this IDistributedCache cache,
        string key,
        string value,
        Encoding? encoding = null,
        DistributedCacheEntryOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (cache == null)
        {
            throw new ArgumentNullException(nameof(cache));
        }
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }
        encoding ??= Encoding.UTF8;
        options ??= new DistributedCacheEntryOptions();
        var bytes = encoding.GetBytes(value);
        return cache.SetAsync(key, bytes, options, cancellationToken);
    }
}