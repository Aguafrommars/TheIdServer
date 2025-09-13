// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.IdentityServer.RavenDb.Store.Client;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Session;
using System.Collections.Generic;

namespace Aguacongas.IdentityServer.RavenDb.Store.Test.AdminStores.Client
{
    public class ClientClaimStoreTest : ClientSubEntityStoreTestBase<ClientClaim>
    {
        protected override IAdminStore<ClientClaim> CreateSut(IAsyncDocumentSession session, ILogger<AdminStore<ClientClaim>> logger)
        => new ClientClaimStore(new ScopedAsynDocumentcSession(session), logger);

        protected override ICollection<ClientClaim> GetCollection(IdentityServer.Store.Entity.Client client)
        {
            if (client.ClientClaims == null)
            {
                client.ClientClaims = new List<ClientClaim>();
            }

            return client.ClientClaims;
        }
    }
}
