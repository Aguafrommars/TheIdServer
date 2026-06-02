// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using System.Threading;
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
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<WsFederationConfiguration> GenerateAsync(string wsfedEndpoint, CancellationToken cancellationToken);
    }
}