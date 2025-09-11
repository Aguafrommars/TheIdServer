// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store.Client
{
    public class ClientGrantTypeStore : ClientSubEntityStoreBase<Entity.ClientGrantType>
    {
        public ClientGrantTypeStore(ScopedAsynDocumentcSession session, ILogger<AdminStore<Entity.ClientGrantType>> logger) : base(session, logger)
        {
        }

        protected override ICollection<Entity.ClientGrantType> GetCollection(Entity.Client client)
        {
            if (client.AllowedGrantTypes == null)
            {
                client.AllowedGrantTypes = new List<Entity.ClientGrantType>();
            }

            return client.AllowedGrantTypes;
        }
    }
}
