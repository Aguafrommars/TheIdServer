// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
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
        protected ApiScopeSubEntityStoreBase(ScopedAsynDocumentcSession session, ILogger<AdminStore<TEntity>> logger) : base(session, logger)
        {
            _session = session.Session;
            _entitybasePath = typeof(TEntity).Name.ToLowerInvariant() + "/";
        }

        protected override async Task OnCreateEntityAsync(TEntity entity, CancellationToken cancellationToken)
        {
            var apiScope = await _session.LoadAsync<Entity.ApiScope>($"{nameof(Entity.ApiScope).ToLowerInvariant()}/{entity.ApiScopeId}", cancellationToken).ConfigureAwait(false);
            if (apiScope == null)
            {
                throw new InvalidOperationException($"ApiScope '{entity.ApiScopeId}' doesn't exist.");
            }
            GetCollection(apiScope).AddEntityId($"{_entitybasePath}{entity.Id}");
            await base.OnCreateEntityAsync(entity, cancellationToken).ConfigureAwait(false);
        }

        protected override async Task OnDeleteEntityAsync(TEntity entity, CancellationToken cancellationToken)
        {
            var apiScope = await _session.LoadAsync<Entity.ApiScope>($"{nameof(Entity.ApiScope).ToLowerInvariant()}/{entity.ApiScopeId}", cancellationToken).ConfigureAwait(false);
            if (apiScope != null)
            {
                GetCollection(apiScope).RemoveEntityId($"{_entitybasePath}{entity.Id}");
            }            
            await base.OnDeleteEntityAsync(entity, cancellationToken).ConfigureAwait(false);
        }

        protected abstract ICollection<TEntity> GetCollection(Entity.ApiScope apiScope);
    }
}
