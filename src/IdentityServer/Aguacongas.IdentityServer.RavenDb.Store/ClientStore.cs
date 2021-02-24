// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using IdentityServer4.Stores;
using Raven.Client.Documents.Session;
using System;
using System.Threading.Tasks;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store
{
    public class ClientStore : IClientStore
    {
        private readonly IAsyncDocumentSession _session;

        public ClientStore(IAsyncDocumentSession session)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
        }
        public async Task<IdentityServer4.Models.Client> FindClientByIdAsync(string clientId)
        {
            var entity = await _session.LoadAsync<Entity.Client>($"{nameof(Entity.Client).ToLowerInvariant()}/{clientId}")
                .ConfigureAwait(false);
            return entity.ToClient();
        }
    }
}
