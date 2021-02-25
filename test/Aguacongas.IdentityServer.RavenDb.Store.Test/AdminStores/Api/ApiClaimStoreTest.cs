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
    public class ApiClaimStoreTest : ApiSubEntityStoreTestBase<ApiClaim>
    {
        protected override IAdminStore<ApiClaim> CreateSut(IAsyncDocumentSession session, ILogger<AdminStore<ApiClaim>> logger)
        {
            return new ApiClaimStore(session, logger);
        }

        protected override ICollection<ApiClaim> GetCollection(ProtectResource api)
        {
            if (api.ApiClaims == null)
            {
                api.ApiClaims = new List<ApiClaim>();
            }
            return api.ApiClaims;
        }
    }
}
