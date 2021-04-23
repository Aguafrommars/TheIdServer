// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using IdentityServer4.Extensions;
using IdentityServer4.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Store
{
    public class CacheAdminStore<TStore, TEntity> : IAdminStore<TEntity> 
        where TStore: IAdminStore<TEntity>
        where TEntity : class
    {
        private readonly TStore _parent;
        private readonly ICache<TEntity> _entityCache;
        private readonly ICache<PageResponse<TEntity>> _responseCache;
        private readonly ILogger<CacheAdminStore<TStore, TEntity>> _logger;
        private readonly IdentityServer4.Configuration.IdentityServerOptions _options;

        public CacheAdminStore(TStore parent,
            ICache<TEntity> entityCache,
            ICache<PageResponse<TEntity>> responseCache,
            ILogger<CacheAdminStore<TStore, TEntity>> logger, 
            IdentityServer4.Configuration.IdentityServerOptions options)
        {
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));
            _entityCache = entityCache ?? throw new ArgumentNullException(nameof(entityCache));
            _responseCache = responseCache ?? throw new ArgumentNullException(nameof(responseCache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
        => _parent.CreateAsync(entity, cancellationToken);

        public Task<object> CreateAsync(object entity, CancellationToken cancellationToken = default)
        => _parent.CreateAsync(entity, cancellationToken);

        public Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        => _parent.DeleteAsync(id, cancellationToken);

        public Task<TEntity> GetAsync(string id, GetRequest request, CancellationToken cancellationToken = default)
        => _entityCache.GetAsync($"{id}_{request?.Expand}", _options.Caching.ClientStoreExpiration, () => _parent.GetAsync(id, request, cancellationToken), _logger);

        public Task<PageResponse<TEntity>> GetAsync(PageRequest request, CancellationToken cancellationToken = default)
        => _responseCache.GetAsync($"{request.Filter}_{request.Skip}_{request.Take}_{request.OrderBy}_{request.Expand}", _options.Caching.ClientStoreExpiration, () => _parent.GetAsync(request, cancellationToken), _logger);

        public Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        => _parent.UpdateAsync(entity, cancellationToken);

        public Task<object> UpdateAsync(object entity, CancellationToken cancellationToken = default)
        => _parent.UpdateAsync(entity, cancellationToken);        
    }
}
