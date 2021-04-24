// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using IdentityServer4.Services;

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
