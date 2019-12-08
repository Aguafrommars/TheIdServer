using Aguacongas.IdentityServer.Store.Entity;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Store
{
    /// <summary>
    /// Identity user store interface
    /// </summary>
    /// <typeparam name="TUser">The type of the user.</typeparam>
    /// <seealso cref="IAdminStore{TUser}" />
    public interface IIdentityUserStore<TUser> : IAdminStore<TUser> where TUser : class
    {
        /// <summary>
        /// Adds the claim asynchronous.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="claim">The claim.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<EntityClaim> AddClaimAsync(string userId, EntityClaim claim, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes the claim asynchronous.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="claim">The claim.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task RemoveClaimAsync(string userId, EntityClaim claim, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the claims asynchronous.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<PageResponse<EntityClaim>> GetClaimsAsync(string userId, PageRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes the login asynchronous.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="login">The login.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task RemoveLoginAsync(string userId, Login login, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the logins asynchronous.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<PageResponse<Login>> GetLoginsAsync(string userId, PageRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds the role asynchronous.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="role">The role.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<string> AddRoleAsync(string userId, string role, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes the role asynchronous.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="role">The role.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task RemoveRoleAsync(string userId, string role, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the roles asynchronous.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<PageResponse<string>> GetRolesAsync(string userId, PageRequest request, CancellationToken cancellationToken = default);
    }
}
