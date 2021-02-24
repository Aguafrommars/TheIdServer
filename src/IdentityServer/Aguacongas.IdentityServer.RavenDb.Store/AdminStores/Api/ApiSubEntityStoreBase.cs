// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Session;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.RavenDb.Store.Api
{
    public abstract class ApiSubEntityStoreBase<TEntity> : AdminStore<TEntity>
        where TEntity : class, IEntityId, IApiSubEntity, new()
    {
        private readonly IAsyncDocumentSession _session;
        private readonly string _entitybasePath;
        protected ApiSubEntityStoreBase(IAsyncDocumentSession session, ILogger<AdminStore<TEntity>> logger) : base(session, logger)
        {
            _session = session;
            _entitybasePath = typeof(TEntity).Name.ToLowerInvariant() + "/";
        }

        protected override async Task OnCreateEntityAsync(TEntity entity, CancellationToken cancellationToken)
        {
            var api = await LoadApiAsync(entity, cancellationToken).ConfigureAwait(false);
            AddSubEntityIdToApi(api, $"{_entitybasePath}{entity.Id}");
            await base.OnCreateEntityAsync(entity, cancellationToken).ConfigureAwait(false);
        }

        protected override async Task OnDeleteEntityAsync(TEntity entity, CancellationToken cancellationToken)
        {
            var api = await LoadApiAsync(entity, cancellationToken).ConfigureAwait(false);
            RemoveSubEntityIdFromApi(api, $"{_entitybasePath}{entity.Id}");
            await base.OnDeleteEntityAsync(entity, cancellationToken).ConfigureAwait(false);
        }

        protected abstract void AddSubEntityIdToApi(ProtectResource api, string id);
        protected abstract void RemoveSubEntityIdFromApi(ProtectResource api, string id);

        private async Task<ProtectResource> LoadApiAsync(TEntity entity, CancellationToken cancellationToken)
        {
            var api = await _session.LoadAsync<ProtectResource>($"{nameof(ProtectResource).ToLowerInvariant()}/{entity.ApiId}", cancellationToken).ConfigureAwait(false);
            if (api == null)
            {
                throw new InvalidOperationException($"Api '{entity.ApiId}' doesn't exist.");
            }
            return api;
        }
    }
}
