// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store.Client
{
    public class ClientPropertyStore : ClientSubEntityStoreBase<Entity.ClientProperty>
    {
        public ClientPropertyStore(ScopedAsynDocumentcSession session, ILogger<AdminStore<Entity.ClientProperty>> logger) : base(session, logger)
        {
        }

        protected override ICollection<Entity.ClientProperty> GetCollection(Entity.Client client)
        {
            if (client.Properties == null)
            {
                client.Properties = new List<Entity.ClientProperty>();
            }

            return client.Properties;
        }
    }
}
