// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.RavenDb.Store.Identity;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Session;
using System.Collections.Generic;

namespace Aguacongas.IdentityServer.RavenDb.Store.Test.AdminStores.Identity
{
    public class IdentityPropertyStoreTest : IdentitySubEntityStoreTestBase<IdentityProperty>
    {
        protected override IAdminStore<IdentityProperty> CreateSut(IAsyncDocumentSession session, ILogger<AdminStore<IdentityProperty>> logger)
        => new IdentityPropertyStore(new ScopedAsynDocumentcSession(session), logger);

        protected override ICollection<IdentityProperty> GetCollection(IdentityResource identity)
        {
            if (identity.Properties == null)
            {
                identity.Properties = new List<IdentityProperty>();
            }

            return identity.Properties;
        }
    }
}
