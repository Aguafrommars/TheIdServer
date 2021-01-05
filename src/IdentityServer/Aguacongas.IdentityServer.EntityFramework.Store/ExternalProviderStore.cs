// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.AspNetCore.Authentication;
using Aguacongas.IdentityServer.Abstractions;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Community.OData.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.Edm;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.EntityFramework.Store
{
    public class ExternalProviderStore : IAdminStore<ExternalProvider>
    {
        private readonly PersistentDynamicManager<SchemeDefinition> _manager;
        private readonly IAuthenticationSchemeOptionsSerializer _serializer;
        private readonly ConfigurationDbContext _context;
        private readonly IProviderClient _providerClient;
#pragma warning disable S2743 // Static fields should not be used in generic types
        private static readonly IEdmModel _edmModel = GetEdmModel();
#pragma warning restore S2743 // Static fields should not be used in generic types

        public ExternalProviderStore(PersistentDynamicManager<SchemeDefinition> manager, 
            IAuthenticationSchemeOptionsSerializer serializer,
            ConfigurationDbContext context,
            IProviderClient providerClient = null)
        {
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _providerClient = providerClient;
        }

        public async Task<ExternalProvider> CreateAsync(ExternalProvider entity, CancellationToken cancellationToken = default)
        {
            var handlerType = _serializer.DeserializeType(entity.SerializedHandlerType);
            var options = _serializer.DeserializeOptions(entity.SerializedOptions, handlerType.GetAuthenticationSchemeOptionsType());
            SanetizeCallbackPath(entity, options);

            await _manager.AddAsync(new SchemeDefinition
            {
                DisplayName = entity.DisplayName,
                StoreClaims = entity.StoreClaims,
                MapDefaultOutboundClaimType = entity.MapDefaultOutboundClaimType,
                HandlerType = handlerType,
                Options = options,
                Scheme = entity.Id
            }, cancellationToken).ConfigureAwait(false);
            if (_providerClient != null)
            {
                await _providerClient.ProviderAddedAsync(entity.Id, cancellationToken).ConfigureAwait(false);
            }
            return entity;
        }

        public async Task<object> CreateAsync(object entity, CancellationToken cancellationToken = default)
        {
            return await CreateAsync(entity as ExternalProvider, cancellationToken).ConfigureAwait(false);
        }

        public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            await _manager.RemoveAsync(id, cancellationToken).ConfigureAwait(false);
            if (_providerClient != null)
            {
                await _providerClient.ProviderRemovedAsync(id, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task<ExternalProvider> GetAsync(string id, GetRequest request, CancellationToken cancellationToken = default)
        {
            var query = _context.Providers.AsNoTracking();
            query = query.Expand(request?.Expand);
            var definition = await query.FirstOrDefaultAsync(e => e.Scheme == id, cancellationToken: cancellationToken).ConfigureAwait(false);
            return CreateEntity(definition);
        }

        public async Task<PageResponse<ExternalProvider>> GetAsync(PageRequest request, CancellationToken cancellationToken = default)
        {
            request = request ?? throw new ArgumentNullException(nameof(request));
            request.Filter = request.Filter?.Replace(nameof(ExternalProvider.Id), nameof(SchemeDefinition.Scheme))
                .Replace(nameof(ExternalProvider.KindName), nameof(SchemeDefinition.SerializedHandlerType));
            request.OrderBy = request.OrderBy?.Replace(nameof(ExternalProvider.Id), nameof(SchemeDefinition.Scheme))
                .Replace(nameof(ExternalProvider.KindName), nameof(SchemeDefinition.SerializedHandlerType));

            var odataQuery = _context.Providers.AsNoTracking().GetODataQuery(request, _edmModel);

            var count = await odataQuery.CountAsync(cancellationToken).ConfigureAwait(false);

            var page = odataQuery.GetPage(request);

            var items = await page.ToListAsync(cancellationToken).ConfigureAwait(false);

            return new PageResponse<ExternalProvider>
            {
                Count = count,
                Items = items.Select(p => CreateEntity(p))
            };
        }

        public async Task<ExternalProvider> UpdateAsync(ExternalProvider entity, CancellationToken cancellationToken = default)
        {
            var definition = await GetEntity(entity.Id).ConfigureAwait(false);
            var handlerType = _serializer.DeserializeType(entity.SerializedHandlerType);

            definition.DisplayName = entity.DisplayName;
            definition.StoreClaims = entity.StoreClaims;
            definition.MapDefaultOutboundClaimType = entity.MapDefaultOutboundClaimType;

            definition.HandlerType = handlerType;
            definition.Options = _serializer.DeserializeOptions(entity.SerializedOptions, handlerType.GetAuthenticationSchemeOptionsType());

            SanetizeCallbackPath(entity, definition.Options);

            await _manager.UpdateAsync(definition, cancellationToken).ConfigureAwait(false);
            if (_providerClient != null)
            {
                await _providerClient.ProviderUpdatedAsync(entity.Id, cancellationToken).ConfigureAwait(false);
            }
            return entity;
        }

        public async Task<object> UpdateAsync(object entity, CancellationToken cancellationToken = default)
        {
            return await UpdateAsync(entity as ExternalProvider, cancellationToken).ConfigureAwait(false);
        }

        private async Task<SchemeDefinition> GetEntity(string id)
        {
            var definition = await _manager.FindBySchemeAsync(id).ConfigureAwait(false);
            if (definition == null)
            {
                throw new IdentityException($"SchemeDefinition {id} not found");
            }
            return definition;
        }

        private ExternalProvider CreateEntity(SchemeDefinition definition)
        {
            if (definition == null)
            {
                return null;
            }

            var hanlderType = definition.HandlerType ?? _serializer.DeserializeType(definition.SerializedHandlerType);
            var optionsType = hanlderType.GetAuthenticationSchemeOptionsType();
            return new ExternalProvider
            {
                DisplayName = definition.DisplayName,
                Id = definition.Scheme,
                StoreClaims = definition.StoreClaims,
                MapDefaultOutboundClaimType = definition.MapDefaultOutboundClaimType,
                KindName = optionsType.Name.Replace("Options", ""),
                SerializedHandlerType = definition.SerializedHandlerType ?? _serializer.SerializeType(hanlderType),
                SerializedOptions = definition.SerializedOptions ?? _serializer.SerializeOptions(definition.Options, optionsType),
                ClaimTransformations = definition.ClaimTransformations
            };
        }

        private static void SanetizeCallbackPath(ExternalProvider entity, AuthenticationSchemeOptions options)
        {
            if (options is RemoteAuthenticationOptions remoteAuthenticationOptions)
            {
                remoteAuthenticationOptions.CallbackPath = $"/signin-{entity.Id}";
            }
        }

        private static IEdmModel GetEdmModel()
        {
            var builder = new ODataConventionModelBuilder();
            var entitySet = builder.EntitySet<SchemeDefinition>(typeof(SchemeDefinition).Name);
            var entityType = entitySet.EntityType;
            entityType.HasKey(e => e.Scheme);
            entityType.Ignore(e => e.HandlerType);
            entityType.Ignore(e => e.Options); 
            return builder.GetEdmModel();
        }
    }
}
