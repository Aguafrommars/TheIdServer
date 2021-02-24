// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Session;
using System.Linq;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store.ApiScope
{
    public class ApiScopeLocalizedResourceStore : ApiScopeSubEntityStoreBase<Entity.ApiScopeLocalizedResource>
    {
        public ApiScopeLocalizedResourceStore(IAsyncDocumentSession session, ILogger<AdminStore<Entity.ApiScopeLocalizedResource>> logger) : base(session, logger)
        {
        }

        protected override void AddSubEntityIdToApiScope(Entity.ApiScope apiScope, string id)
        {
            apiScope.Resources.Add(new Entity.ApiScopeLocalizedResource
            {
                Id = id
            });
        }

        protected override void RemoveSubEntityIdFromApiScope(Entity.ApiScope apiScope, string id)
        {
            apiScope.Resources.Remove(apiScope.Resources.First(e => e.Id == id));
        }
    }
}
