// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Store
{
    /// <summary>
    /// Supported external provider kinds store interface
    /// </summary>
    public interface IExternalProviderKindStore
    {
        /// <summary>
        /// Gets a page of extenal provider kinds supported corresponding to the request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        Task<PageResponse<ExternalProviderKind>> GetAsync(PageRequest request);

    }
}
