// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.IdentityServer.Abstractions;
using Duende.IdentityServer.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Configuration = Duende.IdentityServer.Configuration;

namespace Aguacongas.IdentityServer.Store
{
    public class CacheAdminStore<TStore, TEntity> : IAdminStore<TEntity> 
        where TStore: IAdminStore<TEntity>
        where TEntity : class
    {
        private readonly TStore _parent;
        private readonly IFlushableCache<TEntity> _entityCache;
        private readonly IFlushableCache<PageResponse<TEntity>> _responseCache;
        private readonly ILogger<CacheAdminStore<TStore, TEntity>> _logger;
        private readonly Configuration.IdentityServerOptions _options;

        public CacheAdminStore(TStore parent,
            IFlushableCache<TEntity> entityCache,
            IFlushableCache<PageResponse<TEntity>> responseCache,
            ILogger<CacheAdminStore<TStore, TEntity>> logger, 
            Configuration.IdentityServerOptions options)
        {
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));
            _entityCache = entityCache ?? throw new ArgumentNullException(nameof(entityCache));
            _responseCache = responseCache ?? throw new ArgumentNullException(nameof(responseCache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            var result = await _parent.CreateAsync(entity, cancellationToken).ConfigureAwait(false);
            FlushCaches();
            return result;
        }

        public async Task<object> CreateAsync(object entity, CancellationToken cancellationToken = default)
        => await CreateAsync(entity as TEntity, cancellationToken).ConfigureAwait(false);

        public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            await _parent.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
            FlushCaches();
        }

        public Task<TEntity> GetAsync(string id, GetRequest request, CancellationToken cancellationToken = default)
        => _entityCache.GetAsync($"{id}_{request?.Expand}", _options.Caching.ClientStoreExpiration, () => _parent.GetAsync(id, request, cancellationToken), _logger);

        public Task<PageResponse<TEntity>> GetAsync(PageRequest request, CancellationToken cancellationToken = default)
        => _responseCache.GetAsync($"{request.Filter}_{request.Skip}_{request.Take}_{request.OrderBy}_{request.Expand}", _options.Caching.ClientStoreExpiration, () => _parent.GetAsync(request, cancellationToken), _logger);

        public async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            var result = await _parent.UpdateAsync(entity, cancellationToken).ConfigureAwait(false);
            FlushCaches();
            return result;
        }
        public async Task<object> UpdateAsync(object entity, CancellationToken cancellationToken = default)
        => await UpdateAsync(entity as TEntity, cancellationToken).ConfigureAwait(false);

        private void FlushCaches()
        {
            _entityCache.Flush();
            _responseCache.Flush();
        }
    }
}
