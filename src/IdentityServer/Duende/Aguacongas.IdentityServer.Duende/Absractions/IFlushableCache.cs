// Project: Aguafrommars/TheIdServer
// Copyright (c) 2026 @Olivier Lefebvre

using System;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Abstractions
{
    public interface IFlushableCache<T> where T : class
    {
        /// <summary>
        /// Flushes this instance.
        /// </summary>
        void Flush();

        Task<T> GetOrAddAsync(string key, TimeSpan duration, Func<Task<T>> get);
    }
}
