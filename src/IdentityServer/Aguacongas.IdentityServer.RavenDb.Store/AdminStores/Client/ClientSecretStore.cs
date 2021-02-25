// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Session;
using System.Collections.Generic;
using System.Linq;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store.Client
{
    public class ClientSecretStore : ClientSubEntityStoreBase<Entity.ClientSecret>
    {
        public ClientSecretStore(IAsyncDocumentSession session, ILogger<AdminStore<Entity.ClientSecret>> logger) : base(session, logger)
        {
        }

        protected override ICollection<Entity.ClientSecret> GetCollection(Entity.Client client)
        {
            if (client.ClientSecrets == null)
            {
                client.ClientSecrets = new List<Entity.ClientSecret>();
            }

            return client.ClientSecrets;
        }
    }
}
