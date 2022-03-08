// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.WsFederation.Metadata
{
    /// <summary>
    /// Classes implementing this interace serializes WS-Federation metadata.
    /// </summary>
    public interface IMetatdataSerializer
    {
        /// <summary>
        /// Serializes WS-Federation metadata.
        /// </summary>
        /// <param name="configuration">The WS-Federation configuration.</param>
        /// <returns></returns>
        Task<string> SerializeAsync(WsFederationConfiguration configuration);
    }
}