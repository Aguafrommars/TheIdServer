// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.IdentityServer.RavenDb.Store.ApiScope;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Session;
using System.Collections.Generic;

namespace Aguacongas.IdentityServer.RavenDb.Store.Test.AdminStores.ApiScope
{
    public class ApiScopePropertyStoreTest : ApiScopeSubEntityStoreTestBase<ApiScopeProperty>
    {
        protected override IAdminStore<ApiScopeProperty> CreateSut(IAsyncDocumentSession session, ILogger<AdminStore<ApiScopeProperty>> logger)
        => new ApiScopePropertyStore(new ScopedAsynDocumentcSession(session), logger);

        protected override ICollection<ApiScopeProperty> GetCollection(IdentityServer.Store.Entity.ApiScope apiScope)
        {
            if (apiScope.Properties == null)
            {
                apiScope.Properties = new List<ApiScopeProperty>();
            }

            return apiScope.Properties;
        }
    }
}
