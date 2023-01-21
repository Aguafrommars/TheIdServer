// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.RavenDb.Store.Identity
{
    public abstract class IdentitySubEntityStoreBase<TEntity> : AdminStore<TEntity>
        where TEntity : class, IEntityId, IIdentitySubEntity, new()
    {
        private readonly IAsyncDocumentSession _session;
        private readonly string _entitybasePath;
        protected IdentitySubEntityStoreBase(ScopedAsynDocumentcSession session, ILogger<AdminStore<TEntity>> logger) : base(session, logger)
        {
            _session = session.Session;
            _entitybasePath = typeof(TEntity).Name.ToLowerInvariant() + "/";
        }

        protected override async Task OnCreateEntityAsync(TEntity entity, CancellationToken cancellationToken)
        {
            var identity = await _session.LoadAsync<IdentityResource>($"{nameof(IdentityResource).ToLowerInvariant()}/{entity.IdentityId}", cancellationToken).ConfigureAwait(false);
            if (identity == null)
            {
                throw new InvalidOperationException($"Identity '{entity.IdentityId}' doesn't exist.");
            }
            GetCollection(identity).AddEntityId($"{_entitybasePath}{entity.Id}");
            await base.OnCreateEntityAsync(entity, cancellationToken).ConfigureAwait(false);
        }

        protected override async Task OnDeleteEntityAsync(TEntity entity, CancellationToken cancellationToken)
        {
            var identity = await _session.LoadAsync<IdentityResource>($"{nameof(IdentityResource).ToLowerInvariant()}/{entity.IdentityId}", cancellationToken).ConfigureAwait(false);
            if (identity != null)
            {
                GetCollection(identity).RemoveEntityId($"{_entitybasePath}{entity.Id}");
            }            
            await base.OnDeleteEntityAsync(entity, cancellationToken).ConfigureAwait(false);
        }

        protected abstract ICollection<TEntity> GetCollection(IdentityResource identity);
    }
}
