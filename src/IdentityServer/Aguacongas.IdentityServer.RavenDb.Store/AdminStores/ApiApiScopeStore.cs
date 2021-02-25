// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store
{
    public class ApiApiScopeStore : AdminStore<Entity.ApiApiScope>
    {
        private readonly IAsyncDocumentSession _session;
        private readonly string _entitybasePath;
        public ApiApiScopeStore(IAsyncDocumentSession session, ILogger<AdminStore<Entity.ApiApiScope>> logger) : base(session, logger)
        {
            _session = session;
            _entitybasePath = typeof(Entity.ApiApiScope).Name.ToLowerInvariant() + "/";
        }

        protected override async Task OnCreateEntityAsync(Entity.ApiApiScope entity, CancellationToken cancellationToken)
        {
            var api = await _session.LoadAsync<Entity.ProtectResource>($"{nameof(Entity.ProtectResource).ToLowerInvariant()}/{entity.ApiId}", cancellationToken).ConfigureAwait(false);
            if (api == null)
            {
                throw new InvalidOperationException($"Api '{entity.ApiId}' doesn't exist.");
            }
            var apiScope = await _session.LoadAsync<Entity.ApiScope>($"{nameof(Entity.ApiScope).ToLowerInvariant()}/{entity.ApiScopeId}", cancellationToken).ConfigureAwait(false);
            if (apiScope == null)
            {
                throw new InvalidOperationException($"ApiScope '{entity.ApiScopeId}' doesn't exist.");
            }

            var index = new Entity.ApiApiScope
            {
                Id = $"{_entitybasePath}{entity.Id}"
            };

            if (api.ApiScopes == null)
            {
                api.ApiScopes = new List<Entity.ApiApiScope>();
            }
            if (apiScope.Apis == null)
            {
                apiScope.Apis = new List<Entity.ApiApiScope>();
            }

            api.ApiScopes.Add(index);
            apiScope.Apis.Add(index);

            await base.OnCreateEntityAsync(entity, cancellationToken).ConfigureAwait(false);
        }

        protected override async Task OnDeleteEntityAsync(Entity.ApiApiScope entity, CancellationToken cancellationToken)
        {
            var id = $"{_entitybasePath}{entity.Id}";
            var api = await _session.LoadAsync<Entity.ProtectResource>($"{nameof(Entity.ProtectResource).ToLowerInvariant()}/{entity.ApiId}", cancellationToken).ConfigureAwait(false);
            if (api != null)
            {
                api.ApiScopes.Remove(api.ApiScopes.First(e => e.Id == id));
            }
            var apiScope = await _session.LoadAsync<Entity.ApiScope>($"{nameof(Entity.ApiScope).ToLowerInvariant()}/{entity.ApiScopeId}", cancellationToken).ConfigureAwait(false);
            if (apiScope != null)
            {
                apiScope.Apis.Remove(apiScope.Apis.First(e => e.Id == id));
            }

            await base.OnDeleteEntityAsync(entity, cancellationToken).ConfigureAwait(false);
        }
    }
}
