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
    public class ClientIdpRestrictionStoreTest : ClientSubEntityStoreTestBase<ClientIdpRestriction>
    {
        protected override IAdminStore<ClientIdpRestriction> CreateSut(IAsyncDocumentSession session, ILogger<AdminStore<ClientIdpRestriction>> logger)
        => new ClientIdpRestrictionStore(new ScopedAsynDocumentcSession(session), logger);

        protected override ICollection<ClientIdpRestriction> GetCollection(IdentityServer.Store.Entity.Client client)
        {
            if (client.IdentityProviderRestrictions == null)
            {
                client.IdentityProviderRestrictions = new List<ClientIdpRestriction>();
            }

            return client.IdentityProviderRestrictions;
        }
    }
}
