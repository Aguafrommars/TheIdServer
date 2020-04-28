using Aguacongas.IdentityServer.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.Admin.Hubs
{
    /// <summary>
    /// Provider hub
    /// </summary>
    /// <seealso cref="Hub{IProviderHub}" />
    /// <seealso cref="IProviderHub" />
    [Authorize(Policy = "Is4-Reader")]
    public class ProviderHub : Hub<IProviderHub>, IProviderHub
    {
        /// <summary>
        /// Providers the added.
        /// </summary>
        /// <param name="scheme">The scheme.</param>
        /// <returns></returns>

        [Authorize(Policy = "Is4-Writter")]
        public Task ProviderAdded(string scheme)
        {
            return Clients.Others.ProviderAdded(scheme);
        }

        /// <summary>
        /// Providers the removed.
        /// </summary>
        /// <param name="scheme">The scheme.</param>
        /// <returns></returns>
        [Authorize(Policy = "Is4-Writer")]
        public Task ProviderRemoved(string scheme)
        {
            return Clients.Others.ProviderRemoved(scheme);
        }

        /// <summary>
        /// Providers the updated.
        /// </summary>
        /// <param name="scheme">The scheme.</param>
        /// <returns></returns>
        [Authorize(Policy = "Is4-Writer")]
        public Task ProviderUpdated(string scheme)
        {
            return Clients.Others.ProviderUpdated(scheme);
        }
    }
}
