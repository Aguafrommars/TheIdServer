// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Aguacongas.IdentityServer.RavenDb.Store.Api
{
    public class ApiClaimStore : ApiSubEntityStoreBase<ApiClaim>
    {
        public ApiClaimStore(ScopedAsynDocumentcSession session, ILogger<AdminStore<ApiClaim>> logger) : base(session, logger)
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
