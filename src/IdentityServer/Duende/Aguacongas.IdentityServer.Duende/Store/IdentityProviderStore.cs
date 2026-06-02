using Aguacongas.IdentityServer.Store.Entity;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Store
{
    public class IdentityProviderStore : IIdentityProviderStore
    {
        private readonly IAdminStore<ExternalProvider> _store;

        public IdentityProviderStore(IAdminStore<ExternalProvider> store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public async Task<IEnumerable<IdentityProviderName>> GetAllSchemeNamesAsync()
        {
            var page = await _store.GetAsync(new PageRequest()).ConfigureAwait(false);
            return page.Items.Select(p => new IdentityProviderName
            {
                DisplayName = p.DisplayName,
                Scheme = p.Id,
                Enabled = true
            });
        }

        public Task<IReadOnlyCollection<IdentityProviderName>> GetAllSchemeNamesAsync(CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public async Task<IdentityProvider> GetBySchemeAsync(string scheme, CancellationToken ct)
        {
            var provider = await _store.GetAsync(scheme, new GetRequest(), ct).ConfigureAwait(false);
            return new IdentityProvider("oidc")
            {
                DisplayName = provider.DisplayName,
                Enabled = true,
                Scheme = scheme
            };
        }
    }
}
