// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Abstractions;
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.AspNetCore.Authentication;
using System;
using System.Threading;
using System.Threading.Tasks;
using Aguacongas.TheIdServer.Authentication;

namespace Aguacongas.IdentityServer.Store
{
    public class NotifyChangedExternalProviderStore<TStore> : IAdminStore<ExternalProvider> where TStore : IAdminStore<ExternalProvider>
    {
        private readonly TStore _parent;
        private readonly PersistentDynamicManager<SchemeDefinition> _manager;
        private readonly IAuthenticationSchemeOptionsSerializer _serializer;
        private readonly IProviderClient _providerClient;

        public NotifyChangedExternalProviderStore(TStore parent,
            IProviderClient providerClient, 
            PersistentDynamicManager<SchemeDefinition> manager,
            IAuthenticationSchemeOptionsSerializer serializer)
        {
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));
            _providerClient = providerClient ?? throw new ArgumentNullException(nameof(providerClient));
            _manager = manager ?? throw new ArgumentNullException(nameof(parent));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public async Task<ExternalProvider> CreateAsync(ExternalProvider entity, CancellationToken cancellationToken = default)
        {
            await _manager.AddAsync(CreateSchemeDefinition(entity), cancellationToken).ConfigureAwait(false);

            await _providerClient.ProviderAddedAsync(entity.Id, cancellationToken).ConfigureAwait(false);
            return entity;
        }

        public async Task<object> CreateAsync(object entity, CancellationToken cancellationToken = default)
        {
            return await CreateAsync(entity as ExternalProvider, cancellationToken).ConfigureAwait(false);
        }

        public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            await _manager.RemoveAsync(id, cancellationToken).ConfigureAwait(false);
            await _providerClient.ProviderRemovedAsync(id, cancellationToken).ConfigureAwait(false);
        }

        public Task<ExternalProvider> GetAsync(string id, GetRequest request, CancellationToken cancellationToken = default)
        => _parent.GetAsync(id, request, cancellationToken);

        public Task<PageResponse<ExternalProvider>> GetAsync(PageRequest request, CancellationToken cancellationToken = default)
        => _parent.GetAsync(request, cancellationToken);

        public async Task<ExternalProvider> UpdateAsync(ExternalProvider entity, CancellationToken cancellationToken = default)
        {
            await _manager.UpdateAsync(CreateSchemeDefinition(entity), cancellationToken).ConfigureAwait(false);
            await _providerClient.ProviderUpdatedAsync(entity.Id, cancellationToken).ConfigureAwait(false);
            return entity;
        }

        public async Task<object> UpdateAsync(object entity, CancellationToken cancellationToken = default)
        {
            return await UpdateAsync(entity as ExternalProvider, cancellationToken).ConfigureAwait(false);
        }

        private SchemeDefinition CreateSchemeDefinition(ExternalProvider entity)
        {
            var handlerType = _serializer.DeserializeType(entity.SerializedHandlerType);
            return new()
            {
                DisplayName = entity.DisplayName,
                HandlerType = handlerType,
                Options = _serializer.DeserializeOptions(entity.SerializedOptions, handlerType.GetAuthenticationSchemeOptionsType()),
                Scheme = entity.Id
            };
        }

    }
}
