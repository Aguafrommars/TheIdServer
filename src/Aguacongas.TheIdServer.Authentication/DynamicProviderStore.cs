// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.AspNetCore.Authentication;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.Authentication
{
    public class DynamicProviderStore<TSchemeDefinition> : IDynamicProviderStore<TSchemeDefinition>
        where TSchemeDefinition : SchemeDefinitionBase, new()
    {
        private readonly IAdminStore<ExternalProvider> _store;
        private readonly IAuthenticationSchemeOptionsSerializer _serializer;

        public DynamicProviderStore(IAdminStore<ExternalProvider> store, IAuthenticationSchemeOptionsSerializer serializer)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public IQueryable<TSchemeDefinition> SchemeDefinitions => _store.GetAsync(new PageRequest())
            .GetAwaiter()
            .GetResult()
            .Items
            .Select(FromEntity)
            .AsQueryable();

        public Task AddAsync(TSchemeDefinition definition, CancellationToken cancellationToken = default)
        {
            return _store.CreateAsync(ToEntity(definition), cancellationToken);
        }

        public async Task<TSchemeDefinition> FindBySchemeAsync(string scheme, CancellationToken cancellationToken = default)
        {
            var response = await _store.GetAsync(new PageRequest { Filter = $"{nameof(ExternalProvider.Id)} eq '{scheme}'" }, cancellationToken)
                .ConfigureAwait(false);

            return response.Items.Select(FromEntity).FirstOrDefault();
        }

        public Task RemoveAsync(TSchemeDefinition definition, CancellationToken cancellationToken = default)
        {
            return _store.DeleteAsync(definition.Scheme, cancellationToken);
        }

        public Task UpdateAsync(TSchemeDefinition definition, CancellationToken cancellationToken = default)
        {
            return _store.UpdateAsync(ToEntity(definition), cancellationToken);
        }

        private TSchemeDefinition FromEntity(ExternalProvider entity)
        {
            var handlerType = _serializer.DeserializeType(entity.SerializedHandlerType);
            return new TSchemeDefinition
            {
                Scheme = entity.Id,
                DisplayName = entity.DisplayName,
                HandlerType = handlerType,
                Options = _serializer.DeserializeOptions(entity.SerializedOptions, handlerType.GetAuthenticationSchemeOptionsType())
            };
        }

        private ExternalProvider ToEntity(TSchemeDefinition definition)
        {
            return new ExternalProvider
            {
                Id = definition.Scheme,
                DisplayName = definition.DisplayName,
                SerializedHandlerType = _serializer.SerializeType(definition.HandlerType),
                SerializedOptions = _serializer.SerializeOptions(definition.Options, definition.HandlerType.GetAuthenticationSchemeOptionsType())
            };
        }
    }
}
