// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Microsoft.IdentityModel.Protocols.WsFederation;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.WsFederation
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMetadataResponseGenerator
    {
        /// <summary>
        /// Generates the asynchronous.
        /// </summary>
        /// <param name="wsfedEndpoint">The wsfed endpoint.</param>
        /// <returns></returns>
        Task<WsFederationConfiguration> GenerateAsync(string wsfedEndpoint);
    }
}