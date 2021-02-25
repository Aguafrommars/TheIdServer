// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.RavenDb.Store.Api;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Session;
using System.Collections.Generic;

namespace Aguacongas.IdentityServer.RavenDb.Store.Test.AdminStores.Api
{
    public class ApiLocalizedResourceStoreTest : ApiSubEntityStoreTestBase<ApiLocalizedResource>
    {
        protected override IAdminStore<ApiLocalizedResource> CreateSut(IAsyncDocumentSession session, ILogger<AdminStore<ApiLocalizedResource>> logger)
        {
            return new ApiLocalizedResourceStore(session, logger);
        }

        protected override ICollection<ApiLocalizedResource> GetCollection(ProtectResource api)
        {
            if (api.Resources == null)
            {
                api.Resources = new List<ApiLocalizedResource>();
            }
            return api.Resources;
        }
    }
}
