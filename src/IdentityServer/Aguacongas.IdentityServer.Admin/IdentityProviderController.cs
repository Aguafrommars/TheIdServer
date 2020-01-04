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
        /// Gets an entity.
        /// </summary>
        /// <param name="id">entity id.</param>
        /// <returns>
        /// An entity.
        /// </returns>
        /// <response code="200">Returns an entity.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="204">No content.</response>
        [HttpGet("{id}")]
        [Description("Gets an entity")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ProblemDetails), 404)]
        [Authorize(Policy = "Is4-Reader")]
        public Task<IdentityProvider> GetAsync(string id)
            => _store.GetAsync(id);



        /// <summary>
        /// Search entities using OData style query string (wihtout $).
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpGet]
        [Description("Search entities using OData style query string (wihtout $).")]
        [Authorize(Policy = "Is4-Reader")]
        public Task<PageResponse<IdentityProvider>> GetAsync(PageRequest request)
            => _store.GetAsync(request);
    }
}
