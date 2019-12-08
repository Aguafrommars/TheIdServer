using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Admin
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TRole">The type of the role.</typeparam>
    /// <seealso cref="GenericApiController{TRole}" />
    public class IdentityRoleController<TRole> : GenericApiController<TRole> where TRole: class
    {
        private readonly IIdentityRoleStore<TRole> _store;

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityRoleController{TUser}"/> class.
        /// </summary>
        /// <param name="store">The store.</param>
        public IdentityRoleController(IIdentityRoleStore<TRole> store): base(store)
        {
            _store = store;
        }

        /// <summary>
        /// Adds the claim asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="claim">The claim.</param>
        /// <returns></returns>
        [HttpPost("{id}/claim/add")]
        [Description("Adds claim to a role")]
        [ProducesResponseType(201)]
        [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
        [Authorize(Policy = "Id4-Writer")]
        public Task<EntityClaim> AddClaimAsync(string id, [FromBody] EntityClaim claim)
            => _store.AddClaimAsync(id, claim);

        /// <summary>
        /// Removes the claim asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="claim">The claim.</param>
        /// <returns></returns>
        [HttpPost("{id}/claim/remove")]
        [Description("Removes role's claim")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
        [Authorize(Policy = "Id4-Writer")]
        public Task RemoveClaimAsync(string id, [FromBody] EntityClaim claim)
            => _store.RemoveClaimAsync(id, claim);

        /// <summary>
        /// Gets user claims asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpGet("{id}/claim")]
        [Description("Gets roles's claims")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
        [Authorize(Policy = "Id4-Reader")]
        public Task<PageResponse<EntityClaim>> GetClaimsAsync(string id, PageRequest request)
            => _store.GetClaimsAsync(id, request);

    }
}
