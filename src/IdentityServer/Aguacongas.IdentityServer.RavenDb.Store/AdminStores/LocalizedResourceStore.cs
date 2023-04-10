// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Session;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.RavenDb.Store
{
    public class LocalizedResourceStore : AdminStore<LocalizedResource>
    {
        private readonly IAsyncDocumentSession _session;
        public LocalizedResourceStore(ScopedAsynDocumentcSession session, ILogger<AdminStore<LocalizedResource>> logger) : base(session, logger)
        {
            _session = session.Session;
        }

        protected override async Task OnCreateEntityAsync(LocalizedResource entity, CancellationToken cancellationToken)
        {
            var culture = await _session.LoadAsync<Culture>($"{nameof(Culture).ToLowerInvariant()}/{entity.CultureId}", cancellationToken).ConfigureAwait(false);
            if (culture == null)
            {
                throw new InvalidOperationException($"Culture '{entity.CultureId}' doesn't exist.");
            }

            var collection = culture.Resources ?? new Collection<LocalizedResource>();
            collection.Add(new LocalizedResource
            {
                Id = $"{nameof(LocalizedResource).ToLowerInvariant()}/{entity.Id}"
            });

            culture.Resources = collection;
            await base.OnCreateEntityAsync(entity, cancellationToken).ConfigureAwait(false);
        }

        protected override async Task OnDeleteEntityAsync(LocalizedResource entity, CancellationToken cancellationToken)
        {
            var culture = await _session.LoadAsync<Culture>($"{nameof(Culture).ToLowerInvariant()}/{entity.CultureId}", cancellationToken).ConfigureAwait(false);
            if (culture == null)
            {
                return;
            }

            var collection = culture.Resources ?? new Collection<LocalizedResource>();
            var id = $"{nameof(LocalizedResource).ToLowerInvariant()}/{entity.Id}";
            var toDelete = collection.Where(l => l.Id == id).ToList();
            foreach(var item in toDelete)
            {
                collection.Remove(item);
            }

            await base.OnDeleteEntityAsync(entity, cancellationToken).ConfigureAwait(false);
        }
    }
}
