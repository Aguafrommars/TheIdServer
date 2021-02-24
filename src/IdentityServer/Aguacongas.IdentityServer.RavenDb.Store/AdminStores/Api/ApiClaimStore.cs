// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Session;
using System.Linq;

namespace Aguacongas.IdentityServer.RavenDb.Store.Api
{
    public class ApiClaimStore : ApiSubEntityStoreBase<ApiClaim>
    {
        public ApiClaimStore(IAsyncDocumentSession session, ILogger<AdminStore<ApiClaim>> logger) : base(session, logger)
        {
        }

        protected override void AddSubEntityIdToApi(ProtectResource api, string id)
        {
            api.ApiClaims.Add(new ApiClaim
            {
                Id = id
            });
        }

        protected override void RemoveSubEntityIdFromApi(ProtectResource api, string id)
        {
            api.ApiClaims.Remove(api.ApiClaims.First(e => e.Id == id));
        }
    }
}
