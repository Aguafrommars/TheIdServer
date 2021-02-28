// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using IdentityServer4.Stores;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store
{
    public class ClientStore : IClientStore
    {
        private readonly IAsyncDocumentSession _session;
        private static readonly string[] _expandPropertyList = new[]
        {
            nameof(Entity.Client.AllowedGrantTypes),
            nameof(Entity.Client.AllowedScopes),
            nameof(Entity.Client.ClientClaims),
            nameof(Entity.Client.ClientSecrets),
            nameof(Entity.Client.IdentityProviderRestrictions),
            nameof(Entity.Client.Properties),
            nameof(Entity.Client.RedirectUris),
            nameof(Entity.Client.Resources)
        };

        public ClientStore(IAsyncDocumentSession session)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
        }
        public async Task<IdentityServer4.Models.Client> FindClientByIdAsync(string clientId)
        {
            var entity = await _session
                .LoadAsync<Entity.Client>($"{nameof(Entity.Client).ToLowerInvariant()}/{clientId}", 
                    builder => builder.Expand(string.Join(",", _expandPropertyList)))
                .ConfigureAwait(false);

            entity.AllowedGrantTypes = await _session.GetSubEntitiesAsync(entity.AllowedGrantTypes).ConfigureAwait(false);
            entity.AllowedScopes = await _session.GetSubEntitiesAsync(entity.AllowedScopes).ConfigureAwait(false);
            entity.ClientClaims = await _session.GetSubEntitiesAsync(entity.ClientClaims).ConfigureAwait(false);
            entity.ClientSecrets = await _session.GetSubEntitiesAsync(entity.ClientSecrets).ConfigureAwait(false);
            entity.IdentityProviderRestrictions = await _session.GetSubEntitiesAsync(entity.IdentityProviderRestrictions).ConfigureAwait(false);
            entity.Properties = await _session.GetSubEntitiesAsync(entity.Properties).ConfigureAwait(false);
            entity.RedirectUris = await _session.GetSubEntitiesAsync(entity.RedirectUris).ConfigureAwait(false);
            entity.Resources = await _session.GetSubEntitiesAsync(entity.Resources).ConfigureAwait(false);

            return entity.ToClient();
        }
    }
}
