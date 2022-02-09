// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.IdentityServer.RavenDb.Store.Identity;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Session;
using System.Collections.Generic;

namespace Aguacongas.IdentityServer.RavenDb.Store.Test.AdminStores.Identity
{
    public class IdentityClaimStoreTest : IdentitySubEntityStoreTestBase<IdentityClaim>
    {
        protected override IAdminStore<IdentityClaim> CreateSut(IAsyncDocumentSession session, ILogger<AdminStore<IdentityClaim>> logger)
        => new IdentityClaimStore(new ScopedAsynDocumentcSession(session), logger);

        protected override ICollection<IdentityClaim> GetCollection(IdentityResource identity)
        {
            if (identity.IdentityClaims == null)
            {
                identity.IdentityClaims = new List<IdentityClaim>();
            }

            return identity.IdentityClaims;
        }
    }
}
