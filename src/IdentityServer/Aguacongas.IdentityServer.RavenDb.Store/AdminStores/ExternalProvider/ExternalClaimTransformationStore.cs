// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.RavenDb.Store
{
    public class ExternalClaimTransformationStore : AdminStore<ExternalClaimTransformation>
    {
        private readonly IAsyncDocumentSession _session;
        public ExternalClaimTransformationStore(ScopedAsynDocumentcSession session, ILogger<AdminStore<ExternalClaimTransformation>> logger) : base(session, logger)
        {
            _session = session.Session;
        }

        protected override async Task OnCreateEntityAsync(ExternalClaimTransformation entity, CancellationToken cancellationToken)
        {
            var provider = await _session.LoadAsync<ExternalProvider>($"{nameof(ExternalProvider).ToLowerInvariant()}/{entity.Scheme}", cancellationToken).ConfigureAwait(false);
            if (provider == null)
            {
                throw new InvalidOperationException($"Scheme '{entity.Scheme}' doesn't exist.");
            }

            var collection = provider.ClaimTransformations ?? new List<ExternalClaimTransformation>();
            collection.Add(new ExternalClaimTransformation
            {
                Id = $"{nameof(ExternalClaimTransformation).ToLowerInvariant()}/{entity.Id}"
            });
            provider.ClaimTransformations = collection;

            await base.OnCreateEntityAsync(entity, cancellationToken).ConfigureAwait(false);
        }

        protected override async Task OnDeleteEntityAsync(ExternalClaimTransformation entity, CancellationToken cancellationToken)
        {
            var provider = await _session.LoadAsync<ExternalProvider>($"{nameof(ExternalProvider).ToLowerInvariant()}/{entity.Scheme}", cancellationToken).ConfigureAwait(false);
            if (provider == null)
            {
                return;
            }

            var collection = provider.ClaimTransformations ?? new List<ExternalClaimTransformation>();
            var id = $"{nameof(ExternalClaimTransformation).ToLowerInvariant()}/{entity.Id}";
            var toDeleteList = collection.Where(t => t.Id == id).ToList();
            foreach(var item in toDeleteList)
            {
                collection.Remove(item);
            }
            await base.OnDeleteEntityAsync(entity, cancellationToken).ConfigureAwait(false);
        }
    }
}
