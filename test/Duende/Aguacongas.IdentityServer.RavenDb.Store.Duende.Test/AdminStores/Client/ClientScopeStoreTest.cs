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
    public class ClientScopeStoreTest : ClientSubEntityStoreTestBase<ClientScope>
    {
        protected override IAdminStore<ClientScope> CreateSut(IAsyncDocumentSession session, ILogger<AdminStore<ClientScope>> logger)
        => new ClientScopeStore(new ScopedAsynDocumentcSession(session), logger);

        protected override ICollection<ClientScope> GetCollection(IdentityServer.Store.Entity.Client client)
        {
            if (client.AllowedScopes == null)
            {
                client.AllowedScopes = new List<ClientScope>();
            }

            return client.AllowedScopes;
        }
    }
}
