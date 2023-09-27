// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global
// ReSharper disable CommentTypo
// ReSharper disable UnusedType.Global
// ReSharper disable CheckNamespace

using System.Runtime.Caching;

namespace HamedStack.Cache.Extensions.ObjectCacheExtended;

/// <summary>
/// Provides extension methods for the <see cref="ObjectCache"/> to enhance its functionalities.
/// </summary>
public static class ObjectCacheExtensions
{
    /// <summary>
    /// Adds a key/value pair to the cache if the key does not already exist, or retrieves the current value for the given key.
    /// </summary>
    /// <typeparam name="TKey">The type of the key used to identify cached items.</typeparam>
    /// <typeparam name="TValue">The type of the value stored in the cache.</typeparam>
    /// <param name="this">The extended <see cref="ObjectCache"/> instance.</param>
    /// <param name="key">The key of the item to add or get.</param>
    /// <param name="valueFactory">A function to produce the value for the given key, used if the key does not already exist in cache.</param>
    /// <param name="policy">The policy to be used for the cache item.</param>
    /// <returns>The value for the specified key, whether it was retrieved from the cache or added using the value factory.</returns>
    /// <exception cref="ArgumentNullException">Thrown when key or valueFactory is null.</exception>
    public static TValue AddOrGet<TKey, TValue>(this ObjectCache @this, TKey key, Func<TKey, TValue> valueFactory, CacheItemPolicy policy) where TKey : notnull
    {
        if (key == null) throw new ArgumentNullException(nameof(key));
        if (valueFactory == null) throw new ArgumentNullException(nameof(valueFactory));

        var lazy = new Lazy<TValue>(() => valueFactory(key), true);
        return ((Lazy<TValue>)@this.AddOrGetExisting(key.ToString() ?? string.Empty, lazy, policy)).Value;
    }
}
