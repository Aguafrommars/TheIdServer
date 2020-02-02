using Aguacongas.IdentityServer.Store;
using IdentityServer4.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.Http.Store
{
    /// <summary>
    /// <see cref="ICorsPolicyService"/> implementation
    /// </summary>
    /// <seealso cref="ICorsPolicyService" />
    public class CorsPolicyService : ICorsPolicyService
    {
        private readonly IAdminStore<Entity.ClientUri> _store;

        /// <summary>
        /// Initializes a new instance of the <see cref="CorsPolicyService"/> class.
        /// </summary>
        /// <param name="store">The store.</param>
        /// <exception cref="ArgumentNullException">context</exception>
        public CorsPolicyService(IAdminStore<Entity.ClientUri> store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        /// <summary>
        /// Determines whether origin is allowed.
        /// </summary>
        /// <param name="origin">The origin.</param>
        /// <returns></returns>
        public async Task<bool> IsOriginAllowedAsync(string origin)
        {
            var url = origin.ToUpperInvariant();
            var corsUri = new Uri(origin);
            var corsValue = (int)Entity.UriKinds.Cors;
            var response = await _store.GetAsync(new PageRequest
            {
                Filter = $"startswith(toupper(Uri), '{url}')"
            }).ConfigureAwait(false);
            return response.Items.Any(o => (o.Kind & corsValue) == corsValue && corsUri.CorsMatch(o.Uri));
        }

    }
}
