// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Session;
using System.Linq;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store.Client
{
    public class ClientUriStore : ClientSubEntityStoreBase<Entity.ClientUri>
    {
        public ClientUriStore(IAsyncDocumentSession session, ILogger<AdminStore<Entity.ClientUri>> logger) : base(session, logger)
        {
        }

        protected override void AddSubEntityIdToClient(Entity.Client client, string id)
        {
            client.RedirectUris.Add(new Entity.ClientUri
            {
                Id = id
            });
        }

        protected override void RemoveSubEntityIdFromClient(Entity.Client client, string id)
        {
            client.RedirectUris.Remove(client.RedirectUris.First(e => e.Id == id));
        }
    }
}
