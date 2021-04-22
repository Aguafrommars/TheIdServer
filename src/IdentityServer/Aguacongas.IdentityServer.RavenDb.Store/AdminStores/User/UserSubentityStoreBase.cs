// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store.AdminStores.User
{
    public abstract class UserSubEntityStoreBase<TEntity> : AdminStore<TEntity>
        where TEntity : class, Entity.IEntityId, Entity.IUserSubEntity, new()
    {
        private readonly IAsyncDocumentSession _session;
        private readonly string _entitybasePath;
        protected UserSubEntityStoreBase(ScopedAsynDocumentcSession session, ILogger<AdminStore<TEntity>> logger) : base(session, logger)
        {
            _session = session.Session;
            _entitybasePath = typeof(TEntity).Name.ToLowerInvariant() + "/";
        }

        protected override async Task OnCreateEntityAsync(TEntity entity, CancellationToken cancellationToken)
        {
            var user = await _session.LoadAsync<Entity.User>($"{nameof(Entity.User).ToLowerInvariant()}/{entity.UserId}", cancellationToken).ConfigureAwait(false);
            if (user == null)
            {
                throw new InvalidOperationException($"User '{entity.UserId}' doesn't exist.");
            }

            GetCollection(user).AddEntityId($"{_entitybasePath}{entity.Id}");
            await base.OnCreateEntityAsync(entity, cancellationToken).ConfigureAwait(false);
        }

        protected override async Task OnDeleteEntityAsync(TEntity entity, CancellationToken cancellationToken)
        {
            var user = await _session.LoadAsync<Entity.User>($"{nameof(Entity.User).ToLowerInvariant()}/{entity.UserId}", cancellationToken).ConfigureAwait(false);
            if (user != null)
            {
                GetCollection(user).RemoveEntityId($"{_entitybasePath}{entity.Id}");
            }

            await base.OnDeleteEntityAsync(entity, cancellationToken).ConfigureAwait(false);
        }

        protected abstract ICollection<TEntity> GetCollection(Entity.User user);
    }
}
