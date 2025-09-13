// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Store
{
    /// <summary>
    /// Keys store interface
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IKeyStore<T>
    {
        /// <summary>
        /// Gets keys.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<PageResponse<Key>> GetAllKeysAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Revokes a key.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="reason">The reason.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task RevokeKeyAsync(string id, string reason, CancellationToken cancellationToken = default);
    }
}
