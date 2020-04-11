using Aguacongas.AspNetCore.Authentication;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Community.OData.Linq;
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
#pragma warning disable S2743 // Static fields should not be used in generic types
        private static readonly IEdmModel _edmModel = GetEdmModel();
#pragma warning restore S2743 // Static fields should not be used in generic types

        public ExternalProviderStore(PersistentDynamicManager<SchemeDefinition> manager, 
            IAuthenticationSchemeOptionsSerializer serializer,
            ConfigurationDbContext context)
        {
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<ExternalProvider> CreateAsync(ExternalProvider entity, CancellationToken cancellationToken = default)
        {           
            var handlerType = _serializer.DeserializeType(entity.SerializedHandlerType);
            await _manager.AddAsync(new SchemeDefinition
            {
                DisplayName = entity.DisplayName,
                HandlerType = handlerType,
                Options = _serializer.DeserializeOptions(entity.SerializedOptions, handlerType.GetAuthenticationSchemeOptionsType()),
                Scheme = entity.Id
            }, cancellationToken).ConfigureAwait(false);
            return entity;
        }

        public async Task<object> CreateAsync(object entity, CancellationToken cancellationToken = default)
        {
            return await CreateAsync(entity as ExternalProvider, cancellationToken).ConfigureAwait(false);
        }

        public Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            return _manager.RemoveAsync(id, cancellationToken);
        }

        public async Task<ExternalProvider> GetAsync(string id, GetRequest request, CancellationToken cancellationToken = default)
        {
            var definition = await GetEntity(id).ConfigureAwait(false);
            return CreateEntity(definition);
        }

        public async Task<PageResponse<ExternalProvider>> GetAsync(PageRequest request, CancellationToken cancellationToken = default)
        {
            request = request ?? throw new ArgumentNullException(nameof(request));
            request.Filter = request.Filter?.Replace(nameof(ExternalProvider.Id), nameof(SchemeDefinition.Scheme))
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
            definition.HandlerType = handlerType;
            definition.Options = _serializer.DeserializeOptions(entity.SerializedOptions, handlerType.GetAuthenticationSchemeOptionsType());
            await _manager.UpdateAsync(definition, cancellationToken).ConfigureAwait(false);
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
                KindName = optionsType.Name.Replace("Options", ""),
                SerializedHandlerType = definition.SerializedHandlerType ?? _serializer.SerializeType(hanlderType),
                SerializedOptions = definition.SerializedOptions ?? _serializer.SerializeOptions(definition.Options, optionsType)
            };
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
