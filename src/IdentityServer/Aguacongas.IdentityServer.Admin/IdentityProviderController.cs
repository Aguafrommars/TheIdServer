using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Admin
{
    /// <summary>
    /// Identity provider controller
    /// </summary>
    /// <seealso cref="Controller" />
    [Produces("application/json")]
    [Route("[controller]")]
    public class IdentityProviderController : Controller
    {
        private readonly IIdentityProviderStore _store;

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityProviderController"/> class.
        /// </summary>
        /// <param name="store">The store.</param>
        /// <exception cref="System.ArgumentNullException">store</exception>
        public IdentityProviderController(IIdentityProviderStore store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        /// <summary>
        /// Search entities using OData style query string (wihtout $).
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpGet]
        [Description("Search entities using OData style query string (wihtout $).")]
        [Authorize(Policy = "Id4-Reader")]
        public Task<PageResponse<IdentityProvider>> GetAsync(PageRequest request)
            => _store.GetAsync(request);
    }
}
