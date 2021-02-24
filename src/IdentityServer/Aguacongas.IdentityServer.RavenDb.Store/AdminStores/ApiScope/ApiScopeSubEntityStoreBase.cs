// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Session;
using System;
using System.Threading;
using System.Threading.Tasks;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store.ApiScope
{
    public abstract class ApiScopeSubEntityStoreBase<TEntity> : AdminStore<TEntity>
        where TEntity : class, Entity.IEntityId, Entity.IApiScopeSubEntity, new()
    {
        private readonly IAsyncDocumentSession _session;
        private readonly string _entitybasePath;
        protected ApiScopeSubEntityStoreBase(IAsyncDocumentSession session, ILogger<AdminStore<TEntity>> logger) : base(session, logger)
        {
            _session = session;
            _entitybasePath = typeof(TEntity).Name.ToLowerInvariant() + "/";
        }

        protected override async Task OnCreateEntityAsync(TEntity entity, CancellationToken cancellationToken)
        {
            var apiScope = await LoadApiScopeAsync(entity, cancellationToken).ConfigureAwait(false);
            AddSubEntityIdToApiScope(apiScope, $"{_entitybasePath}{entity.Id}");
            await base.OnCreateEntityAsync(entity, cancellationToken).ConfigureAwait(false);
        }

        protected override async Task OnDeleteEntityAsync(TEntity entity, CancellationToken cancellationToken)
        {
            var apiScope = await LoadApiScopeAsync(entity, cancellationToken).ConfigureAwait(false);
            RemoveSubEntityIdFromApiScope(apiScope, $"{_entitybasePath}{entity.Id}");
            await base.OnDeleteEntityAsync(entity, cancellationToken).ConfigureAwait(false);
        }

        protected abstract void AddSubEntityIdToApiScope(Entity.ApiScope apiScope, string id);
        protected abstract void RemoveSubEntityIdFromApiScope(Entity.ApiScope apiScope, string id);

        private async Task<Entity.ApiScope> LoadApiScopeAsync(TEntity entity, CancellationToken cancellationToken)
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
