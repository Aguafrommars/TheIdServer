// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store.ApiScope
{
    public class ApiScopeClaimStore : ApiScopeSubEntityStoreBase<Entity.ApiScopeClaim>
    {
        public ApiScopeClaimStore(ScopedAsynDocumentcSession session, ILogger<AdminStore<Entity.ApiScopeClaim>> logger) : base(session, logger)
        {
        }

        protected override ICollection<Entity.ApiScopeClaim> GetCollection(Entity.ApiScope apiScope)
        {
            if (apiScope.ApiScopeClaims == null)
            {
                apiScope.ApiScopeClaims = new List<Entity.ApiScopeClaim>();
            }

            return apiScope.ApiScopeClaims;
        }
    }
}
