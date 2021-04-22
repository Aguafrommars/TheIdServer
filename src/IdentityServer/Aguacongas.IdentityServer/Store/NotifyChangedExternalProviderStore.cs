// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Abstractions;
using Aguacongas.IdentityServer.Store.Entity;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Store
{
    public class NotifyChangedExternalProviderStore : IAdminStore<ExternalProvider>
    {
        private readonly IAdminStore<ExternalProvider> _parent;
        private readonly IProviderClient _providerClient;

        public NotifyChangedExternalProviderStore(IAdminStore<ExternalProvider> parent, IProviderClient providerClient)
        {
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));
            _providerClient = providerClient ?? throw new ArgumentNullException(nameof(providerClient));
        }

        public async Task<ExternalProvider> CreateAsync(ExternalProvider entity, CancellationToken cancellationToken = default)
        {
            var result = await _parent.CreateAsync(entity, cancellationToken).ConfigureAwait(false);
            await _providerClient.ProviderAddedAsync(entity.Id).ConfigureAwait(false);
            return result;
        }

        public async Task<object> CreateAsync(object entity, CancellationToken cancellationToken = default)
        {
            return await CreateAsync(entity as ExternalProvider).ConfigureAwait(false);
        }

        public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            await _parent.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
            await _providerClient.ProviderRemovedAsync(id).ConfigureAwait(false);
        }

        public Task<ExternalProvider> GetAsync(string id, GetRequest request, CancellationToken cancellationToken = default)
        => _parent.GetAsync(id, request, cancellationToken);

        public Task<PageResponse<ExternalProvider>> GetAsync(PageRequest request, CancellationToken cancellationToken = default)
        => _parent.GetAsync(request, cancellationToken);

        public async Task<ExternalProvider> UpdateAsync(ExternalProvider entity, CancellationToken cancellationToken = default)
        {
            var result = await _parent.UpdateAsync(entity, cancellationToken).ConfigureAwait(false);
            await _providerClient.ProviderUpdatedAsync(entity.Id).ConfigureAwait(false);
            return result;
        }

        public async Task<object> UpdateAsync(object entity, CancellationToken cancellationToken = default)
        {
            return await UpdateAsync(entity as ExternalProvider).ConfigureAwait(false);
        }
    }
}
