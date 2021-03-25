// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store.Client
{
    public class ClientScopeStore : ClientSubEntityStoreBase<Entity.ClientScope>
    {
        public ClientScopeStore(ScopedAsynDocumentcSession session, ILogger<AdminStore<Entity.ClientScope>> logger) : base(session, logger)
        {
        }

        protected override ICollection<Entity.ClientScope> GetCollection(Entity.Client client)
        {
            if (client.AllowedScopes == null)
            {
                client.AllowedScopes = new List<Entity.ClientScope>();
            }

            return client.AllowedScopes;
        }
    }
}
