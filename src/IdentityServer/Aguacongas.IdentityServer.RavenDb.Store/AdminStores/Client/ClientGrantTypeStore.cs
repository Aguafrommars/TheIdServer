// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Session;
using System.Linq;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store.Client
{
    public class ClientGrantTypeStore : ClientSubEntityStoreBase<Entity.ClientGrantType>
    {
        public ClientGrantTypeStore(IAsyncDocumentSession session, ILogger<AdminStore<Entity.ClientGrantType>> logger) : base(session, logger)
        {
        }

        protected override void AddSubEntityIdToClient(Entity.Client client, string id)
        {
            client.AllowedGrantTypes.Add(new Entity.ClientGrantType
            {
                Id = id
            });
        }

        protected override void RemoveSubEntityIdFromClient(Entity.Client client, string id)
        {
            client.AllowedGrantTypes.Remove(client.AllowedGrantTypes.First(e => e.Id == id));
        }
    }
}
