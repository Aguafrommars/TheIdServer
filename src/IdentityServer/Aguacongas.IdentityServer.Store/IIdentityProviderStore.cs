// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Store
{
    /// <summary>
    /// Identity provider store
    /// </summary>
    public interface IIdentityProviderStore
    {
        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        Task<IdentityProvider> GetAsync(string id);
        
        /// <summary>
        /// Gets a page of identity provider corresponding to the request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        Task<PageResponse<IdentityProvider>> GetAsync(PageRequest request);
    }
}