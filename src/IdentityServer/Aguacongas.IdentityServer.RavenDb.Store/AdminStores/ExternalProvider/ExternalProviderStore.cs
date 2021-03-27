// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.AspNetCore.Authentication;
using Aguacongas.IdentityServer.Abstractions;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Community.OData.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.OData.Edm;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store
{
    public class ExternalProviderStore : IAdminStore<ExternalProvider>
    {
        private readonly PersistentDynamicManager<SchemeDefinition> _manager;
        private readonly IAuthenticationSchemeOptionsSerializer _serializer;
        private readonly IAsyncDocumentSession _session;
        private readonly IProviderClient _providerClient;

        public ExternalProviderStore(PersistentDynamicManager<SchemeDefinition> manager, 
            IAuthenticationSchemeOptionsSerializer serializer,
            ScopedAsynDocumentcSession session,
            IProviderClient providerClient = null)
        {
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _session = session?.Session ?? throw new ArgumentNullException(nameof(session));
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
            var definition = await _session.LoadAsync<SchemeDefinition>($"{nameof(SchemeDefinition).ToLowerInvariant()}/{id}", b => b.Expand(request?.Expand), cancellationToken)
                .ConfigureAwait(false);
            if(request?.Expand == nameof(SchemeDefinition.ClaimTransformations))
            {
                definition.ClaimTransformations ??= new List<ExternalClaimTransformation>(0);
                var transformationList = new List<ExternalClaimTransformation>(definition.ClaimTransformations.Count);
                foreach(var transformationId in definition.ClaimTransformations)
                {
                    transformationList.Add(await _session.LoadAsync<ExternalClaimTransformation>(transformationId.Id, cancellationToken).ConfigureAwait(false));
                }
                definition.ClaimTransformations = transformationList;
            }
            return CreateEntity(definition);
        }

        public async Task<PageResponse<ExternalProvider>> GetAsync(PageRequest request, CancellationToken cancellationToken = default)
        {
            request = request ?? throw new ArgumentNullException(nameof(request));
            
            var rql = request.ToRQL<SchemeDefinition>(_session.Advanced.DocumentStore.Conventions.FindCollectionName(typeof(SchemeDefinition)), GetEdmModel());
            var pageQuery = _session.Advanced.AsyncRawQuery<SchemeDefinition>(rql);
            if (request.Take.HasValue)
            {
                pageQuery = pageQuery.GetPage(request);
            }

            var items = await pageQuery.ToListAsync(cancellationToken).ConfigureAwait(false);

            var countQuery = _session.Advanced.AsyncRawQuery<SchemeDefinition>(rql);
            var count = await countQuery.CountAsync(cancellationToken).ConfigureAwait(false);
            return new PageResponse<ExternalProvider>
            {
                Count = count,
                Items = items.Select(p => CreateEntity(p))
            };
        }

        public async Task<ExternalProvider> UpdateAsync(ExternalProvider entity, CancellationToken cancellationToken = default)
        {
            var definition = await GetEntity(entity.Id).ConfigureAwait(false);

            definition.DisplayName = entity.DisplayName;
            definition.StoreClaims = entity.StoreClaims;
            definition.MapDefaultOutboundClaimType = entity.MapDefaultOutboundClaimType;
            definition.SerializedHandlerType = entity.SerializedHandlerType;
            definition.SerializedOptions = entity.SerializedOptions;
            
            SanetizeCallbackPath(entity, definition.Options);

            definition.HandlerType = null;
            definition.Options = null;

            await _session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

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

            var hanlderType = _serializer.DeserializeType(definition.SerializedHandlerType);
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
