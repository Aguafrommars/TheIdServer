// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store.ApiScope
{
    public class ApiScopePropertyStore : ApiScopeSubEntityStoreBase<Entity.ApiScopeProperty>
    {
        public ApiScopePropertyStore(ScopedAsynDocumentcSession session, ILogger<AdminStore<Entity.ApiScopeProperty>> logger) : base(session, logger)
        {
        }

        protected override ICollection<Entity.ApiScopeProperty> GetCollection(Entity.ApiScope apiScope)
        {
            if (apiScope.Properties == null)
            {
                apiScope.Properties = new List<Entity.ApiScopeProperty>();
            }

            return apiScope.Properties;
        }
    }
}
