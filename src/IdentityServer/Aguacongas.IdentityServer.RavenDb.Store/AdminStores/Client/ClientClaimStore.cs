// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store.Client
{
    public class ClientClaimStore : ClientSubEntityStoreBase<Entity.ClientClaim>
    {
        public ClientClaimStore(ScopedAsynDocumentcSession session, ILogger<AdminStore<Entity.ClientClaim>> logger) : base(session, logger)
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
