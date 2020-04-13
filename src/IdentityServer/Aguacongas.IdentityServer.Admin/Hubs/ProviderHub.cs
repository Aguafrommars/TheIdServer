using Aguacongas.IdentityServer.Abstractions;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.Admin.Hubs
{
    /// <summary>
    /// Provider hub
    /// </summary>
    /// <seealso cref="Hub{IProviderHub}" />
    /// <seealso cref="IProviderHub" />
    public class ProviderHub : Hub<IProviderHub>, IProviderHub
    {
        /// <summary>
        /// Providers the added.
        /// </summary>
        /// <param name="scheme">The scheme.</param>
        /// <returns></returns>
        public Task ProviderAdded(string scheme)
        {
            return Clients.Others.ProviderAdded(scheme);
        }

        /// <summary>
        /// Providers the removed.
        /// </summary>
        /// <param name="scheme">The scheme.</param>
        /// <returns></returns>
        public Task ProviderRemoved(string scheme)
        {
            return Clients.Others.ProviderRemoved(scheme);
        }

        /// <summary>
        /// Providers the updated.
        /// </summary>
        /// <param name="scheme">The scheme.</param>
        /// <returns></returns>
        public Task ProviderUpdated(string scheme)
        {
            return Clients.Others.ProviderUpdated(scheme);
        }
    }
}
