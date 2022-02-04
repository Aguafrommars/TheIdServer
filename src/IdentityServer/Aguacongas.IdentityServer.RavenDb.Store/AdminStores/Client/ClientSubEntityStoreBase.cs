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

namespace Aguacongas.IdentityServer.RavenDb.Store.Client
{
    public abstract class ClientSubEntityStoreBase<TEntity> : AdminStore<TEntity>
        where TEntity : class, Entity.IEntityId, Entity.IClientSubEntity, new()
    {
        private readonly IAsyncDocumentSession _session;
        private readonly string _entitybasePath;
        protected ClientSubEntityStoreBase(ScopedAsynDocumentcSession session, ILogger<AdminStore<TEntity>> logger) : base(session, logger)
        {
            _session = session.Session;
            _entitybasePath = typeof(TEntity).Name.ToLowerInvariant() + "/";
        }

        protected override async Task OnCreateEntityAsync(TEntity entity, CancellationToken cancellationToken)
        {
            var client = await _session.LoadAsync<Entity.Client>($"{nameof(Entity.Client).ToLowerInvariant()}/{entity.ClientId}", cancellationToken).ConfigureAwait(false);
            if (client == null)
            {
                throw new InvalidOperationException($"Client '{entity.ClientId}' doesn't exist.");
            }
            GetCollection(client).AddEntityId($"{_entitybasePath}{entity.Id}");
            await base.OnCreateEntityAsync(entity, cancellationToken).ConfigureAwait(false);
        }

        protected override async Task OnDeleteEntityAsync(TEntity entity, CancellationToken cancellationToken)
        {
            var client = await _session.LoadAsync<Entity.Client>($"{nameof(Entity.Client).ToLowerInvariant()}/{entity.ClientId}", cancellationToken).ConfigureAwait(false);
            if (client != null)
            {
                GetCollection(client).RemoveEntityId($"{_entitybasePath}{entity.Id}");
            }
            await base.OnDeleteEntityAsync(entity, cancellationToken).ConfigureAwait(false);
        }

        protected abstract ICollection<TEntity> GetCollection(Entity.Client client);
    }
}
