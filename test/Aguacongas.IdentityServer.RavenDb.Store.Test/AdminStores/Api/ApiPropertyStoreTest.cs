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
    public class ApiPropertyStoreTest : ApiSubEntityStoreTestBase<ApiProperty>
    {
        protected override IAdminStore<ApiProperty> CreateSut(IAsyncDocumentSession session, ILogger<AdminStore<ApiProperty>> logger)
        {
            return new ApiPropertyStore(session, logger);
        }

        protected override ICollection<ApiProperty> GetCollection(ProtectResource api)
        {
            if (api.Properties == null)
            {
                api.Properties = new List<ApiProperty>();
            }
            return api.Properties;
        }
    }
}
