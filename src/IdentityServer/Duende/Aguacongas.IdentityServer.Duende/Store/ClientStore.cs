// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Stores;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Store
{
    public class ClientStore : IClientStore
    {
        private readonly IAdminStore<Entity.Client> _store;

        public ClientStore(IAdminStore<Entity.Client> store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }
        public async Task<Client> FindClientByIdAsync(string clientId, CancellationToken ct)
        {
            var entity = await _store.GetAsync(clientId, new GetRequest
            {
                Expand = $"{nameof(Entity.Client.IdentityProviderRestrictions)},{nameof(Entity.Client.ClientClaims)},{nameof(Entity.Client.ClientSecrets)},{nameof(Entity.Client.AllowedGrantTypes)},{nameof(Entity.Client.RedirectUris)},{nameof(Entity.Client.AllowedScopes)},{nameof(Entity.Client.Properties)},{nameof(Entity.Client.Resources)},{nameof(Entity.Client.AllowedIdentityTokenSigningAlgorithms)}"
            }, ct).ConfigureAwait(false);
            return entity.ToClient();
        }

        public async IAsyncEnumerable<Client> GetAllClientsAsync([EnumeratorCancellation] CancellationToken ct)
        {
            var result = await _store.GetAsync(new PageRequest
            {
                Expand = $"{nameof(Entity.Client.IdentityProviderRestrictions)},{nameof(Entity.Client.ClientClaims)},{nameof(Entity.Client.ClientSecrets)},{nameof(Entity.Client.AllowedGrantTypes)},{nameof(Entity.Client.RedirectUris)},{nameof(Entity.Client.AllowedScopes)},{nameof(Entity.Client.Properties)},{nameof(Entity.Client.Resources)},{nameof(Entity.Client.AllowedIdentityTokenSigningAlgorithms)}"
            }, ct).ConfigureAwait(false);

            foreach (var item in result.Items)
            {
                yield return item.ToClient();
            }
        }
    }
}
