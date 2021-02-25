// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Session;
using System.Collections.Generic;

namespace Aguacongas.IdentityServer.RavenDb.Store.Api
{
    public class ApiClaimStore : ApiSubEntityStoreBase<ApiClaim>
    {
        public ApiClaimStore(IAsyncDocumentSession session, ILogger<AdminStore<ApiClaim>> logger) : base(session, logger)
        {
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
