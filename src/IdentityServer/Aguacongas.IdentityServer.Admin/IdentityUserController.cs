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
    /// <typeparam name="TUser">The type of the user.</typeparam>
    /// <seealso cref="GenericApiController{TUser}" />
    public class IdentityUserController<TUser> : GenericApiController<TUser> where TUser: class
    {
        private readonly IIdentityUserStore<TUser> _store;
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityUserController{TUser}"/> class.
        /// </summary>
        /// <param name="store">The store.</param>
        public IdentityUserController(IIdentityUserStore<TUser> store): base(store)
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
        [Description("Adds claim to a user")]
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
        [Description("Removes user's claim")]
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
        [Description("Gets user's claims")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
        [Authorize(Policy = "Id4-Reader")]
        public Task<PageResponse<EntityClaim>> GetClaimsAsync(string id, PageRequest request)
            => _store.GetClaimsAsync(id, request);

        /// <summary>
        /// Adds the role asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="role">The claim.</param>
        /// <returns></returns>
        [HttpPost("{id}/role")]
        [Description("Add role to a user")]
        [ProducesResponseType(201)]
        [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
        [Authorize(Policy = "Id4-Writer")]
        public Task<string> AddRoleAsync(string id, [FromBody] string role)
            => _store.AddRoleAsync(id, role);

        /// <summary>
        /// Removes the role asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="role">The role.</param>
        /// <returns></returns>
        [HttpDelete("{id}/role/{role}")]
        [Description("Removes user's role")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
        [Authorize(Policy = "Id4-Writer")]
        public Task RemoveRoleAsync(string id, string role)
            => _store.RemoveRoleAsync(id, role);

        /// <summary>
        /// Gets user claims asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpGet("{id}/role")]
        [Description("Gets user's roles")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
        [Authorize(Policy = "Id4-Reader")]
        public Task<PageResponse<string>> GetRolesAsync(string id, PageRequest request)
            => _store.GetRolesAsync(id, request);

        /// <summary>
        /// Removes the login asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="login">The login info.</param>
        /// <returns></returns>
        [HttpPost("{id}/login/remove")]
        [Description("Removes user's login")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
        [Authorize(Policy = "Id4-Writer")]
        public Task RemoveLoginAsync(string id, [FromBody] Login login)
            => _store.RemoveLoginAsync(id, login);

        /// <summary>
        /// Gets user logins asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpGet("{id}/login")]
        [Description("Gets user's logins")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
        [Authorize(Policy = "Id4-Reader")]
        public Task<PageResponse<Login>> GetLoginsAsync(string id, PageRequest request)
            => _store.GetLoginsAsync(id, request);
    }
}
