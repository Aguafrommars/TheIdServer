// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store.AdminStores.RelyingParty
{
    public class RelyingPartyClaimMappingStore : AdminStore<Entity.RelyingPartyClaimMapping>
    {
        private readonly IAsyncDocumentSession _session;

        public RelyingPartyClaimMappingStore(ScopedAsynDocumentcSession session, ILogger<AdminStore<Entity.RelyingPartyClaimMapping>> logger) : base(session, logger)
        {
            _session = session.Session;
        }

        protected override async Task OnCreateEntityAsync(Entity.RelyingPartyClaimMapping entity, CancellationToken cancellationToken)
        {
            var provider = await _session.LoadAsync<Entity.RelyingParty>($"{nameof(RelyingParty).ToLowerInvariant()}/{entity.RelyingPartyId}", cancellationToken).ConfigureAwait(false);
            if (provider == null)
            {
                throw new InvalidOperationException($"Scheme '{entity.RelyingPartyId}' doesn't exist.");
            }

            var collection = provider.ClaimMappings ?? new List<Entity.RelyingPartyClaimMapping>();
            collection.Add(new Entity.RelyingPartyClaimMapping
            {
                Id = $"{nameof(Entity.RelyingPartyClaimMapping).ToLowerInvariant()}/{entity.Id}"
            });
            provider.ClaimMappings = collection;

            await base.OnCreateEntityAsync(entity, cancellationToken).ConfigureAwait(false);
        }

        protected override async Task OnDeleteEntityAsync(Entity.RelyingPartyClaimMapping entity, CancellationToken cancellationToken)
        {
            var provider = await _session.LoadAsync<Entity.RelyingParty>($"{nameof(RelyingParty).ToLowerInvariant()}/{entity.RelyingPartyId}", cancellationToken).ConfigureAwait(false);
            if (provider == null)
            {
                return;
            }

            var collection = provider.ClaimMappings ?? new List<Entity.RelyingPartyClaimMapping>();
            var id = $"{nameof(Entity.RelyingPartyClaimMapping).ToLowerInvariant()}/{entity.Id}";
            var toDeleteList = collection.Where(t => t.Id == id).ToList();
            foreach (var item in toDeleteList)
            {
                collection.Remove(item);
            }
            await base.OnDeleteEntityAsync(entity, cancellationToken).ConfigureAwait(false);
        }

    }
}
