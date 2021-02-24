// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Session;
using System.Linq;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store.ApiScope
{
    public class ApiScopePropertyStore : ApiScopeSubEntityStoreBase<Entity.ApiScopeProperty>
    {
        public ApiScopePropertyStore(IAsyncDocumentSession session, ILogger<AdminStore<Entity.ApiScopeProperty>> logger) : base(session, logger)
        {
        }

        protected override void AddSubEntityIdToApiScope(Entity.ApiScope apiScope, string id)
        {
            apiScope.Properties.Add(new Entity.ApiScopeProperty
            {
                Id = id
            });
        }

        protected override void RemoveSubEntityIdFromApiScope(Entity.ApiScope apiScope, string id)
        {
            apiScope.Properties.Remove(apiScope.Properties.First(e => e.Id == id));
        }
    }
}
