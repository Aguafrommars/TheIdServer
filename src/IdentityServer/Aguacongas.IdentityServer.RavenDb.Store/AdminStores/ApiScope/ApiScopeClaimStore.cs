// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Session;
using System.Linq;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store.ApiScope
{
    public class ApiScopeClaimStore : ApiScopeSubEntityStoreBase<Entity.ApiScopeClaim>
    {
        public ApiScopeClaimStore(IAsyncDocumentSession session, ILogger<AdminStore<Entity.ApiScopeClaim>> logger) : base(session, logger)
        {
        }

        protected override void AddSubEntityIdToApiScope(Entity.ApiScope apiScope, string id)
        {
            apiScope.ApiScopeClaims.Add(new Entity.ApiScopeClaim
            {
                Id = id
            });
        }

        protected override void RemoveSubEntityIdFromApiScope(Entity.ApiScope apiScope, string id)
        {
            apiScope.ApiScopeClaims.Remove(apiScope.ApiScopeClaims.First(e => e.Id == id));
        }
    }
}
