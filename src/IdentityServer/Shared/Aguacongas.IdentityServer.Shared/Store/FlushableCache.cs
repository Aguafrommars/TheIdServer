// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.IdentityServer.Abstractions;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Store
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="IFlushableCache{T}" />
    /// <seealso cref="IDisposable" />
    public class FlushableCache<T> : IFlushableCache<T>, IDisposable where T : class
    {
        private IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());
        private bool disposedValue;


        /// <summary>
        /// Flushes this instance.
        /// </summary>
        public void Flush()
        {
            _cache.Dispose();
            _cache = new MemoryCache(new MemoryCacheOptions());
        }


        /// <summary>
        /// Gets the cached data based upon a key index.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// The cached item, or <c>null</c> if no item matches the key.
        /// </returns>
        public Task<T> GetAsync(string key)
        => Task.FromResult(_cache.Get<T>(GetKey(key)));


        /// <summary>
        /// Caches the data based upon a key
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="item">The item.</param>
        /// <param name="expiration">The expiration.</param>
        /// <returns></returns>
        public Task SetAsync(string key, T item, TimeSpan expiration)
        {
            _cache.Set(GetKey(key), item, expiration);
            return Task.CompletedTask;
        }

        /// <summary>
        ///  the cached data based upon a key index. If the item is not found, the get
        ///     function is used to obtain the item and populate the cache.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="get">The function to obtain the item.</param>
        /// <returns>The cached item.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<T> GetOrAddAsync(string key, TimeSpan duration, Func<Task<T>> get)
        => _cache.GetOrCreateAsync(key, entry =>
        {
            entry.SetAbsoluteExpiration(duration);
            return get();
        });

        /// <summary>
        /// Removes the cached data based upon a key index.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task RemoveAsync(string key)
        {
            _cache.Remove(key);
            return Task.CompletedTask;
        }


        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _cache.Dispose();
                }
                disposedValue = true;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private static string GetKey(string key)
        => $"{typeof(T).FullName}:{key}";
    }
}
