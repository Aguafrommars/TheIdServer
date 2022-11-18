// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.IdentityServer.RavenDb.Store.Api;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Session;
using System.Collections.Generic;

namespace Aguacongas.IdentityServer.RavenDb.Store.Test.AdminStores.Api
{
    public class ApiSecretStoreTest : ApiSubEntityStoreTestBase<ApiSecret>
    {
        protected override IAdminStore<ApiSecret> CreateSut(IAsyncDocumentSession session, ILogger<AdminStore<ApiSecret>> logger)
        => new ApiSecretStore(new ScopedAsynDocumentcSession(session), logger);

        protected override ICollection<ApiSecret> GetCollection(ProtectResource api)
        {
            if (api.Secrets == null)
            {
                api.Secrets = new List<ApiSecret>();
            }
            return api.Secrets;
        }
    }
}
