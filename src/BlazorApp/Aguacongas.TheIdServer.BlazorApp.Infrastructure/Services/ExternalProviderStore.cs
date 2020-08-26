// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Aguacongas.TheIdServer.BlazorApp.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Services
{
    public class ExternalProviderStore : IAdminStore<ExternalProvider>
    {
        private readonly IAdminStore<Entity.ExternalProvider> _store;

        public ExternalProviderStore(IAdminStore<Entity.ExternalProvider> store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public async Task<ExternalProvider> CreateAsync(ExternalProvider entity, CancellationToken cancellationToken = default)
        {
            return ExternalProvider.FromEntity(await _store.CreateAsync(entity, cancellationToken)
                .ConfigureAwait(false));
        }

        public async Task<object> CreateAsync(object entity, CancellationToken cancellationToken = default)
        {
            return await CreateAsync(entity as ExternalProvider, cancellationToken)
                .ConfigureAwait(false);
        }

        public Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            return _store.DeleteAsync(id, cancellationToken);
        }

        public async Task<ExternalProvider> GetAsync(string id, GetRequest request, CancellationToken cancellationToken = default)
        {
            return ExternalProvider.FromEntity(await _store.GetAsync(id, request, cancellationToken)
                .ConfigureAwait(false));
        }

        public Task<PageResponse<ExternalProvider>> GetAsync(PageRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<ExternalProvider> UpdateAsync(ExternalProvider entity, CancellationToken cancellationToken = default)
        {
            return ExternalProvider.FromEntity(await _store.UpdateAsync(entity, cancellationToken)
                .ConfigureAwait(false));
        }

        public async Task<object> UpdateAsync(object entity, CancellationToken cancellationToken = default)
        {
            return await UpdateAsync(entity as ExternalProvider, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
