// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
#if DUENDE
using Duende.IdentityServer.Services;
#else
using IdentityServer4.Services;
#endif

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
