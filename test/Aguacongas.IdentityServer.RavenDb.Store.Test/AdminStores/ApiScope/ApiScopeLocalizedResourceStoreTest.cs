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
    public class ApiScopeLocalizedResourceStoreTest : ApiScopeSubEntityStoreTestBase<ApiScopeLocalizedResource>
    {
        protected override IAdminStore<ApiScopeLocalizedResource> CreateSut(IAsyncDocumentSession session, ILogger<AdminStore<ApiScopeLocalizedResource>> logger)
        => new ApiScopeLocalizedResourceStore(new ScopedAsynDocumentcSession(session), logger);

        protected override ICollection<ApiScopeLocalizedResource> GetCollection(IdentityServer.Store.Entity.ApiScope apiScope)
        {
            if (apiScope.Resources == null)
            {
                apiScope.Resources = new List<ApiScopeLocalizedResource>();
            }

            return apiScope.Resources;
        }
    }
}
