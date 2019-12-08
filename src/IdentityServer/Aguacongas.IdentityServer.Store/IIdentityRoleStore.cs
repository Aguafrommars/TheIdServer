using Aguacongas.IdentityServer.Store.Entity;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Store
{
    /// <summary>
    /// Identity role store interface
    /// </summary>
    /// <typeparam name="TRole">The type of the role.</typeparam>
    /// <seealso cref="IAdminStore{TRole}" />
    public interface IIdentityRoleStore<TRole> : IAdminStore<TRole> where TRole : class
    {
        /// <summary>
        /// Adds the claim asynchronous.
        /// </summary>
        /// <param name="roleId">The role identifier.</param>
        /// <param name="claim">The claim.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<EntityClaim> AddClaimAsync(string roleId, EntityClaim claim, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes the claim asynchronous.
        /// </summary>
        /// <param name="roleId">The role identifier.</param>
        /// <param name="claim">The claim.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task RemoveClaimAsync(string roleId, EntityClaim claim, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the claims asynchronous.
        /// </summary>
        /// <param name="roleId">The role identifier.</param>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<PageResponse<EntityClaim>> GetClaimsAsync(string roleId, PageRequest request, CancellationToken cancellationToken = default);

    }
}
