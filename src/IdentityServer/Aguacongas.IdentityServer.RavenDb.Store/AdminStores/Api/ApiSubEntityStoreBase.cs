// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.RavenDb.Store.Api
{
    public abstract class ApiSubEntityStoreBase<TEntity> : AdminStore<TEntity>
        where TEntity : class, IEntityId, IApiSubEntity, new()
    {
        private readonly IAsyncDocumentSession _session;
        private readonly string _entitybasePath;
        protected ApiSubEntityStoreBase(ScopedAsynDocumentcSession session, ILogger<AdminStore<TEntity>> logger) : base(session, logger)
        {
            _session = session.Session;
            _entitybasePath = typeof(TEntity).Name.ToLowerInvariant() + "/";
        }

        protected override async Task OnCreateEntityAsync(TEntity entity, CancellationToken cancellationToken)
        {
            var api = await _session.LoadAsync<ProtectResource>($"{nameof(ProtectResource).ToLowerInvariant()}/{entity.ApiId}", cancellationToken).ConfigureAwait(false);
            if (api == null)
            {
                throw new InvalidOperationException($"Api '{entity.ApiId}' doesn't exist.");
            }
            GetCollection(api).AddEntityId($"{_entitybasePath}{entity.Id}");
            await base.OnCreateEntityAsync(entity, cancellationToken).ConfigureAwait(false);
        }

        protected override async Task OnDeleteEntityAsync(TEntity entity, CancellationToken cancellationToken)
        {
            var api = await _session.LoadAsync<ProtectResource>($"{nameof(ProtectResource).ToLowerInvariant()}/{entity.ApiId}", cancellationToken).ConfigureAwait(false);
            if (api != null)
            {
                GetCollection(api).RemoveEntityId($"{_entitybasePath}{entity.Id}");
            }    
            await base.OnDeleteEntityAsync(entity, cancellationToken).ConfigureAwait(false);
        }

        protected abstract ICollection<TEntity> GetCollection(ProtectResource api);

    }
}
