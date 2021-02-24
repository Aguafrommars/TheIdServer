// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Session;
using System.Linq;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store.Client
{
    public class ClientScopeStore : ClientSubEntityStoreBase<Entity.ClientScope>
    {
        public ClientScopeStore(IAsyncDocumentSession session, ILogger<AdminStore<Entity.ClientScope>> logger) : base(session, logger)
        {
        }

        protected override void AddSubEntityIdToClient(Entity.Client client, string id)
        {
            client.AllowedScopes.Add(new Entity.ClientScope
            {
                Id = id
            });
        }

        protected override void RemoveSubEntityIdFromClient(Entity.Client client, string id)
        {
            client.AllowedScopes.Remove(client.AllowedScopes.First(e => e.Id == id));
        }
    }
}
