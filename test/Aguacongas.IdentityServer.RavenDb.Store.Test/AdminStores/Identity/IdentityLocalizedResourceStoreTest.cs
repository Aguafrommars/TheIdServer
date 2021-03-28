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
    public class IdentityLocalizedResourceStoreTest : IdentitySubEntityStoreTestBase<IdentityLocalizedResource>
    {
        protected override IAdminStore<IdentityLocalizedResource> CreateSut(IAsyncDocumentSession session, ILogger<AdminStore<IdentityLocalizedResource>> logger)
        => new IdentityLocalizedResourceStore(new ScopedAsynDocumentcSession(session), logger);

        protected override ICollection<IdentityLocalizedResource> GetCollection(IdentityResource identity)
        {
            if (identity.Resources == null)
            {
                identity.Resources = new List<IdentityLocalizedResource>();
            }

            return identity.Resources;
        }
    }
}
