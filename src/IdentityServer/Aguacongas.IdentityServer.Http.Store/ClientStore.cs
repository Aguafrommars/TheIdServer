using Aguacongas.IdentityServer.Store;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using System;
using System.Threading.Tasks;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.Http.Store
{
    public class ClientStore : IClientStore
    {
        private readonly IAdminStore<Entity.Client> _store;

        public ClientStore(IAdminStore<Entity.Client> store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }
        public async Task<Client> FindClientByIdAsync(string clientId)
        {
            var entity = await _store.GetAsync(clientId, new GetRequest
            {
                Expand = "IdentityProviderRestrictions,ClientClaims,ClientSecrets,AllowedGrantTypes,RedirectUris,AllowedScopes,Properties"
            }).ConfigureAwait(false);
            return entity.ToClient();
        }
    }
}
