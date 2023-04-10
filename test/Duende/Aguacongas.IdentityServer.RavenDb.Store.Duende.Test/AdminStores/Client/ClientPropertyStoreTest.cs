// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Aguacongas.IdentityServer.RavenDb.Store.Client;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Session;
using System.Collections.Generic;

namespace Aguacongas.IdentityServer.RavenDb.Store.Test.AdminStores.Client
{
    public class ClientPropertyStoreTest : ClientSubEntityStoreTestBase<ClientProperty>
    {
        protected override IAdminStore<ClientProperty> CreateSut(IAsyncDocumentSession session, ILogger<AdminStore<ClientProperty>> logger)
        => new ClientPropertyStore(new ScopedAsynDocumentcSession(session), logger);

        protected override ICollection<ClientProperty> GetCollection(IdentityServer.Store.Entity.Client client)
        {
            if (client.Properties == null)
            {
                client.Properties = new List<ClientProperty>();
            }

            return client.Properties;
        }
    }
}
