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
        private readonly HubConnection _connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProviderClient"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <exception cref="ArgumentNullException">connection</exception>
        public ProviderClient(HubConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        /// <summary>
        /// Providers the added.
        /// </summary>
        /// <param name="scheme">The scheme.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public Task ProviderAdded(string scheme, CancellationToken cancellationToken = default)
        {
            return _connection.InvokeAsync(nameof(IProviderHub.ProviderAdded), scheme, cancellationToken);
        }

        /// <summary>
        /// Providers the removed.
        /// </summary>
        /// <param name="scheme">The scheme.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public Task ProviderRemoved(string scheme, CancellationToken cancellationToken = default)
        {
            return _connection.InvokeAsync(nameof(IProviderHub.ProviderRemoved), scheme, cancellationToken);
        }

        /// <summary>
        /// Providers the updated.
        /// </summary>
        /// <param name="scheme">The scheme.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public Task ProviderUpdated(string scheme, CancellationToken cancellationToken = default)
        {
            return _connection.InvokeAsync(nameof(IProviderHub.ProviderUpdated), scheme, cancellationToken);
        }
    }
}
