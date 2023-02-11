// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store.AdminStores.Role
{
    public class RoleClaimStore : AdminStore<Entity.RoleClaim>
    {
        private readonly IAsyncDocumentSession _session;
        public RoleClaimStore(ScopedAsynDocumentcSession session, ILogger<AdminStore<Entity.RoleClaim>> logger) : base(session, logger)
        {
            _session = session.Session;
        }

        protected override async Task OnCreateEntityAsync(Entity.RoleClaim entity, CancellationToken cancellationToken)
        {
            var role = await _session.LoadAsync<Entity.Role>($"{nameof(Entity.Role).ToLowerInvariant()}/{entity.RoleId}", cancellationToken).ConfigureAwait(false);
            if (role == null)
            {
                throw new InvalidOperationException($"Role '{entity.RoleId}' doesn't exist.");
            }

            var collection = role.RoleClaims ?? new List<Entity.RoleClaim>();
            collection.Add(new Entity.RoleClaim
            {
                Id = $"{nameof(Entity.RoleClaim).ToLowerInvariant()}/{entity.Id}"
            });
            role.RoleClaims = collection;

            await base.OnCreateEntityAsync(entity, cancellationToken).ConfigureAwait(false);
        }

        protected override async Task OnDeleteEntityAsync(Entity.RoleClaim entity, CancellationToken cancellationToken)
        {
            var role = await _session.LoadAsync<Entity.Role>($"{nameof(Entity.Role).ToLowerInvariant()}/{entity.RoleId}", cancellationToken).ConfigureAwait(false);
            if (role == null)
            {
                return;
            }

            var collection = role.RoleClaims ?? new List<Entity.RoleClaim>();
            var id = $"{nameof(Entity.RoleClaim).ToLowerInvariant()}/{entity.Id}";
            var toDeleteList = collection.Where(t => t.Id == id).ToList();
            foreach (var item in toDeleteList)
            {
                collection.Remove(item);
            }
            await base.OnDeleteEntityAsync(entity, cancellationToken).ConfigureAwait(false);
        }
    }
}
