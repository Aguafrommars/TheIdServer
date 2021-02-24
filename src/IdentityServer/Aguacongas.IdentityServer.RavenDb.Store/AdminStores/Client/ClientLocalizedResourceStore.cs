// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Session;
using System.Linq;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store.Client
{
    public class ClientLocalizedResourceStore : ClientSubEntityStoreBase<Entity.ClientLocalizedResource>
    {
        public ClientLocalizedResourceStore(IAsyncDocumentSession session, ILogger<AdminStore<Entity.ClientLocalizedResource>> logger) : base(session, logger)
        {
        }

        protected override void AddSubEntityIdToClient(Entity.Client client, string id)
        {
            client.Resources.Add(new Entity.ClientLocalizedResource
            {
                Id = id
            });
        }

        protected override void RemoveSubEntityIdFromClient(Entity.Client client, string id)
        {
            client.Resources.Remove(client.Resources.First(e => e.Id == id));
        }
    }
}
