// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store.ApiScope
{
    public class ApiScopeLocalizedResourceStore : ApiScopeSubEntityStoreBase<Entity.ApiScopeLocalizedResource>
    {
        public ApiScopeLocalizedResourceStore(ScopedAsynDocumentcSession session, ILogger<AdminStore<Entity.ApiScopeLocalizedResource>> logger) : base(session, logger)
        {
        }

        protected override ICollection<Entity.ApiScopeLocalizedResource> GetCollection(Entity.ApiScope apiScope)
        {
            if (apiScope.Resources == null)
            {
                apiScope.Resources = new List<Entity.ApiScopeLocalizedResource>();
            }

            return apiScope.Resources;
        }
    }
}
