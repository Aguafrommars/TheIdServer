using Aguacongas.IdentityServer.Abstractions;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Admin
{
    /// <summary>
    /// Claims provider controller
    /// </summary>
    /// <seealso cref="Controller" />
    [Produces("application/json")]
    [Route("[controller]")]

    public class ClaimsProviderController
    {
        private readonly IProxyClaimsProvider _proxyClaimsProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClaimsProviderController"/> class.
        /// </summary>
        /// <param name="proxyClaimsProvider">The proxy claims provider.</param>
        /// <exception cref="ArgumentNullException">proxyClaimsProvider</exception>
        public ClaimsProviderController(IProxyClaimsProvider proxyClaimsProvider)
        {
            _proxyClaimsProvider = proxyClaimsProvider ?? throw new ArgumentNullException(nameof(proxyClaimsProvider));
        }

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="caller">The caller.</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Policy = "Is4-Reader")]
        public Task<PageResponse<UserClaim>> GetAsync(string resourceName, string userId, string clientId, string caller)
            => _proxyClaimsProvider.GetAsync(resourceName, userId, clientId, caller);
    }
}
