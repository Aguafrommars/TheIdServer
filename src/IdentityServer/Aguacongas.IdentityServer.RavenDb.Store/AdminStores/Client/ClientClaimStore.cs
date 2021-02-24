// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Session;
using System.Linq;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store.Client
{
    public class ClientClaimStore : ClientSubEntityStoreBase<Entity.ClientClaim>
    {
        public ClientClaimStore(IAsyncDocumentSession session, ILogger<AdminStore<Entity.ClientClaim>> logger) : base(session, logger)
        {
        }

        protected override void AddSubEntityIdToClient(Entity.Client client, string id)
        {
            client.ClientClaims.Add(new Entity.ClientClaim
            {
                Id = id
            });
        }

        protected override void RemoveSubEntityIdFromClient(Entity.Client client, string id)
        {
            client.ClientClaims.Remove(client.ClientClaims.First(e => e.Id == id));
        }
    }
}
