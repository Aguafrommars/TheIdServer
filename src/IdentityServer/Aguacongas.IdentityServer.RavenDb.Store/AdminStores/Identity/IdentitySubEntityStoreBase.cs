// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Session;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.RavenDb.Store.Identity
{
    public abstract class IdentitySubEntityStoreBase<TEntity> : AdminStore<TEntity>
        where TEntity : class, IEntityId, IIdentitySubEntity, new()
    {
        private readonly IAsyncDocumentSession _session;
        private readonly string _entitybasePath;
        protected IdentitySubEntityStoreBase(IAsyncDocumentSession session, ILogger<AdminStore<TEntity>> logger) : base(session, logger)
        {
            _session = session;
            _entitybasePath = typeof(TEntity).Name.ToLowerInvariant() + "/";
        }

        protected override async Task OnCreateEntityAsync(TEntity entity, CancellationToken cancellationToken)
        {
            var identity = await LoadIdentityAsync(entity, cancellationToken).ConfigureAwait(false);
            AddSubEntityIdToIdentity(identity, $"{_entitybasePath}{entity.Id}");
            await base.OnCreateEntityAsync(entity, cancellationToken).ConfigureAwait(false);
        }

        protected override async Task OnDeleteEntityAsync(TEntity entity, CancellationToken cancellationToken)
        {
            var identity = await LoadIdentityAsync(entity, cancellationToken).ConfigureAwait(false);
            RemoveSubEntityIdFromIdentity(identity, $"{_entitybasePath}{entity.Id}");
            await base.OnDeleteEntityAsync(entity, cancellationToken).ConfigureAwait(false);
        }

        protected abstract void AddSubEntityIdToIdentity(IdentityResource identity, string id);
        protected abstract void RemoveSubEntityIdFromIdentity(IdentityResource identity, string id);

        private async Task<IdentityResource> LoadIdentityAsync(TEntity entity, CancellationToken cancellationToken)
        {
            var identity = await _session.LoadAsync<IdentityResource>($"{nameof(IdentityResource).ToLowerInvariant()}/{entity.IdentityId}", cancellationToken).ConfigureAwait(false);
            if (identity == null)
            {
                throw new InvalidOperationException($"Identity '{entity.IdentityId}' doesn't exist.");
            }
            return identity;
        }
    }
}
