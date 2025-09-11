// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Duende.IdentityServer.Services;

namespace Aguacongas.IdentityServer.Abstractions
{
    public interface IFlushableCache<T> : ICache<T> where T: class
    {
        /// <summary>
        /// Flushes this instance.
        /// </summary>
        void Flush();
    }
}
