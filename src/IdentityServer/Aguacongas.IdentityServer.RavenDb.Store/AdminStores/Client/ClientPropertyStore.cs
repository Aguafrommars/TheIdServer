// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Session;
using System.Linq;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store.Client
{
    public class ClientPropertyStore : ClientSubEntityStoreBase<Entity.ClientProperty>
    {
        public ClientPropertyStore(IAsyncDocumentSession session, ILogger<AdminStore<Entity.ClientProperty>> logger) : base(session, logger)
        {
        }

        protected override void AddSubEntityIdToClient(Entity.Client client, string id)
        {
            client.Properties.Add(new Entity.ClientProperty
            {
                Id = id
            });
        }

        protected override void RemoveSubEntityIdFromClient(Entity.Client client, string id)
        {
            client.Properties.Remove(client.Properties.First(e => e.Id == id));
        }
    }
}
