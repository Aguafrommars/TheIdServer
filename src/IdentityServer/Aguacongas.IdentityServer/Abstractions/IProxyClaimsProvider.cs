// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Abstractions
{
    /// <summary>
    /// Proxy claims provider interface
    /// </summary>
    public interface IProxyClaimsProvider
    {
        /// <summary>
        /// Gets user claims.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="caller">The caller.</param>
        /// <param name="providerTypeName">Name of the provider type.</param>
        /// <returns></returns>
        Task<PageResponse<UserClaim>> GetAsync(string resourceName, string userId, string clientId, string caller, string providerTypeName);
    }
}