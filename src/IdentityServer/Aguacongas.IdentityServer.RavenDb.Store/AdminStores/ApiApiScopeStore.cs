// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Session;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store.Api
{
    public abstract class ApiApiScopeStore : AdminStore<Entity.ApiApiScope>
    {
        private readonly IAsyncDocumentSession _session;
        private readonly string _entitybasePath;
        protected ApiApiScopeStore(IAsyncDocumentSession session, ILogger<AdminStore<Entity.ApiApiScope>> logger) : base(session, logger)
        {
            _session = session;
            _entitybasePath = typeof(Entity.ApiApiScope).Name.ToLowerInvariant() + "/";
        }

        protected override async Task OnCreateEntityAsync(Entity.ApiApiScope entity, CancellationToken cancellationToken)
        {
            var api = await LoadApiAsync(entity, cancellationToken).ConfigureAwait(false);
            var apiScope = await LoadApiScopeAsync(entity, cancellationToken).ConfigureAwait(false);

            var index = new Entity.ApiApiScope
            {
                Id = $"{_entitybasePath}{entity.Id}"
            };
            api.ApiScopes.Add(index);
            apiScope.Apis.Add(index);

            await base.OnCreateEntityAsync(entity, cancellationToken).ConfigureAwait(false);
        }

        protected override async Task OnDeleteEntityAsync(Entity.ApiApiScope entity, CancellationToken cancellationToken)
        {
            var api = await LoadApiAsync(entity, cancellationToken).ConfigureAwait(false);
            var apiScope = await LoadApiScopeAsync(entity, cancellationToken).ConfigureAwait(false);

            var id = $"{_entitybasePath}{entity.Id}";
            api.ApiScopes.Remove(api.ApiScopes.First(e => e.Id == id));
            apiScope.Apis.Remove(apiScope.Apis.First(e => e.Id == id));

            await base.OnDeleteEntityAsync(entity, cancellationToken).ConfigureAwait(false);
        }

        private async Task<Entity.ProtectResource> LoadApiAsync(Entity.ApiApiScope entity, CancellationToken cancellationToken)
        {
            var api = await _session.LoadAsync<Entity.ProtectResource>($"{nameof(Entity.ProtectResource).ToLowerInvariant()}/{entity.ApiId}", cancellationToken).ConfigureAwait(false);
            if (api == null)
            {
                throw new InvalidOperationException($"Api '{entity.ApiId}' doesn't exist.");
            }
            return api;
        }

        private async Task<Entity.ApiScope> LoadApiScopeAsync(Entity.ApiApiScope entity, CancellationToken cancellationToken)
        {
            var apiScope = await _session.LoadAsync<Entity.ApiScope>($"{nameof(Entity.ApiScope).ToLowerInvariant()}/{entity.ApiScopeId}", cancellationToken).ConfigureAwait(false);
            if (apiScope == null)
            {
                throw new InvalidOperationException($"ApiScope '{entity.ApiScopeId}' doesn't exist.");
            }
            return apiScope;
        }
    }
}
