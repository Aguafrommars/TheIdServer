// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.RavenDb.Store.ApiScope;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Session;
using System.Collections.Generic;

namespace Aguacongas.IdentityServer.RavenDb.Store.Test.AdminStores.ApiScope
{
    public class ClientClaimStoreTest : ApiScopeSubEntityStoreTestBase<ApiScopeClaim>
    {
        protected override IAdminStore<ApiScopeClaim> CreateSut(IAsyncDocumentSession session, ILogger<AdminStore<ApiScopeClaim>> logger)
        => new ApiScopeClaimStore(session, logger);

        protected override ICollection<ApiScopeClaim> GetCollection(IdentityServer.Store.Entity.ApiScope apiScope)
        {
            if (apiScope.ApiScopeClaims == null)
            {
                apiScope.ApiScopeClaims = new List<ApiScopeClaim>();
            }

            return apiScope.ApiScopeClaims;
        }
    }
}
