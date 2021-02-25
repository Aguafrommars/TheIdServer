// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Session;
using System.Collections.Generic;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store.Client
{
    public class ClientClaimStore : ClientSubEntityStoreBase<Entity.ClientClaim>
    {
        public ClientClaimStore(IAsyncDocumentSession session, ILogger<AdminStore<Entity.ClientClaim>> logger) : base(session, logger)
        {
        }

        protected override ICollection<Entity.ClientClaim> GetCollection(Entity.Client client)
        {
            if (client.ClientClaims == null)
            {
                client.ClientClaims = new List<Entity.ClientClaim>();
            }

            return client.ClientClaims;
        }
    }
}
