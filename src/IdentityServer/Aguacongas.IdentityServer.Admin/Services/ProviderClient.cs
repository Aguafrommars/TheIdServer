// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.IdentityServer.Abstractions;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Admin.Services
{
    /// <summary>
    /// Provider hub client
    /// </summary>
    /// <seealso cref="IProviderClient" />
    public class ProviderClient : IProviderClient
    {
        private readonly HubConnectionFactory _hubConnectionFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProviderClient"/> class.
        /// </summary>
        /// <param name="hubConnectionFactory">The hub connection factory.</param>
        /// <exception cref="ArgumentNullException">
        /// context
        /// or
        /// hubConnectionFactory
        /// </exception>
        public ProviderClient(HubConnectionFactory hubConnectionFactory)
        {
            _hubConnectionFactory = hubConnectionFactory ?? throw new ArgumentNullException(nameof(hubConnectionFactory));
        }

        /// <summary>
        /// Key revoked.
        /// </summary>
        /// <param name="kind">The key kind.</param>
        /// <param name="id">The key identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public Task KeyRevokedAsync(string kind, Guid id, CancellationToken cancellationToken = default)
        {            
            return GetConnection(cancellationToken).SendAsync(nameof(IProviderHub.KeyRevoked), kind, id, cancellationToken);
        }

        /// <summary>
        /// Providers the added.
        /// </summary>
        /// <param name="scheme">The scheme.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public Task ProviderAddedAsync(string scheme, CancellationToken cancellationToken = default)
        {
            return GetConnection(cancellationToken).SendAsync(nameof(IProviderHub.ProviderAdded), scheme, cancellationToken);
        }

        /// <summary>
        /// Providers the removed.
        /// </summary>
        /// <param name="scheme">The scheme.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public Task ProviderRemovedAsync(string scheme, CancellationToken cancellationToken = default)
        {
            return GetConnection(cancellationToken).SendAsync(nameof(IProviderHub.ProviderRemoved), scheme, cancellationToken);
        }

        /// <summary>
        /// Providers the updated.
        /// </summary>
        /// <param name="scheme">The scheme.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public Task ProviderUpdatedAsync(string scheme, CancellationToken cancellationToken = default)
        {
            return GetConnection(cancellationToken).SendAsync(nameof(IProviderHub.ProviderUpdated), scheme, cancellationToken);
        }

        private HubConnection GetConnection(CancellationToken cancellationToken)
        {
            return _hubConnectionFactory.GetConnection(cancellationToken);
        }
    }
}
