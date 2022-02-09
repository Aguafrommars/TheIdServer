// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.IdentityServer.RavenDb.Store.Client;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Session;
using System.Collections.Generic;

namespace Aguacongas.IdentityServer.RavenDb.Store.Test.AdminStores.Client
{
    public class ClientUriStoreTest : ClientSubEntityStoreTestBase<ClientUri>
    {
        protected override IAdminStore<ClientUri> CreateSut(IAsyncDocumentSession session, ILogger<AdminStore<ClientUri>> logger)
        => new ClientUriStore(new ScopedAsynDocumentcSession(session), logger);

        protected override ICollection<ClientUri> GetCollection(IdentityServer.Store.Entity.Client client)
        {
            if (client.RedirectUris == null)
            {
                client.RedirectUris = new List<ClientUri>();
            }

            return client.RedirectUris;
        }
    }
}
