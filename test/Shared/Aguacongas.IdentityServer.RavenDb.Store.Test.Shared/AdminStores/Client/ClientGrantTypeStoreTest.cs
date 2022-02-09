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
    public class ClientGrantTypeStoreTest : ClientSubEntityStoreTestBase<ClientGrantType>
    {
        protected override IAdminStore<ClientGrantType> CreateSut(IAsyncDocumentSession session, ILogger<AdminStore<ClientGrantType>> logger)
        => new ClientGrantTypeStore(new ScopedAsynDocumentcSession(session), logger);

        protected override ICollection<ClientGrantType> GetCollection(IdentityServer.Store.Entity.Client client)
        {
            if (client.AllowedGrantTypes == null)
            {
                client.AllowedGrantTypes = new List<ClientGrantType>();
            }

            return client.AllowedGrantTypes;
        }
    }
}
