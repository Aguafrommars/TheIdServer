using Aguacongas.IdentityServer.Store.Entity;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Store
{
    /// <summary>
    /// Admin store interface
    /// </summary>
    public interface IAdminStore
    {
        /// <summary>
        /// Creates an entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<IEntityId> CreateAsync(IEntityId entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<IEntityId> UpdateAsync(IEntityId entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Class implementing this interface is an admin store
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IAdminStore<T> : IAdminStore where T:class
    {
        /// <summary>
        /// Creates an entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets an entity.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="request">OData style get request</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<T> GetAsync(string id, GetRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a page of entities.
        /// </summary>
        /// <param name="request">Odata style get request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<PageResponse<T>> GetAsync(PageRequest request, CancellationToken cancellationToken = default);
    }
}
