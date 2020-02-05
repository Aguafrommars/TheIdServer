using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.EntityFramework.Store
{
    [SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "No localization")]
    public class AdminStore<TEntity, TContext> : IAdminStore<TEntity> 
        where TEntity: class, IEntityId, new()
        where TContext: DbContext
    {
        private readonly TContext _context;
        private readonly ILogger<AdminStore<TEntity, TContext>> _logger;

        public AdminStore(TContext context, ILogger<AdminStore<TEntity, TContext>> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task<TEntity> GetAsync(string id, GetRequest request, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<TEntity>().AsNoTracking();
            query = query.Expand(request?.Expand);
            return query.Where(e => e.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<PageResponse<TEntity>> GetAsync(PageRequest request, CancellationToken cancellationToken = default)
        {
            request = request ?? throw new ArgumentNullException(nameof(request));
            var query = _context.Set<TEntity>().AsNoTracking();
            var odataQuery = query.GetODataQuery(request);

            var count = await odataQuery.CountAsync(cancellationToken).ConfigureAwait(false);

            var page = odataQuery.GetPage(request);

            var items = (await page.ToListAsync(cancellationToken).ConfigureAwait(false)) as IEnumerable<TEntity>;

            return new PageResponse<TEntity>
            {
                Count = count,
                Items = items
            };
        }

        public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            var entity = await _context.Set<TEntity>().FindAsync(new[] { id }, cancellationToken).ConfigureAwait(false);
            if (entity == null)
            {
                throw new DbUpdateException($"Entity type {typeof(TEntity).Name} at id {id} is not found");
            }
            _context.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            _logger.LogInformation("Entity {EntityId} deleted", entity.Id, entity);
        }

        public async Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            entity = entity ?? throw new ArgumentNullException(nameof(entity));
            if (entity.Id == null)
            {
                entity.Id = Guid.NewGuid().ToString();
            }
            await _context.AddAsync(entity, cancellationToken).ConfigureAwait(false);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            _logger.LogInformation("Entity {EntityId} created", entity.Id, entity);
            return entity;
        }

        public async Task<object> CreateAsync(object entity, CancellationToken cancellationToken = default)
        {
            return await CreateAsync(entity as TEntity, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            entity = entity ?? throw new ArgumentNullException(nameof(entity));
            var storedEntity = await _context.Set<TEntity>()
                .AsNoTracking()
                .Where(e => e.Id == entity.Id)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);
            if (storedEntity == null)
            {
                throw new DbUpdateException($"Entity type {typeof(TEntity).Name} at id {entity.Id} is not found");
            }
            if (entity is IAuditable auditable)
            {
                var storedAuditable = storedEntity as IAuditable;
                auditable.CreatedAt = storedAuditable.CreatedAt;
                auditable.ModifiedAt = storedAuditable.ModifiedAt;
            }
            _context.Update(entity);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            _logger.LogInformation("Entity {EntityId} updated", entity.Id, entity);
            return entity;
        }

        public async Task<object> UpdateAsync(object entity, CancellationToken cancellationToken = default)
        {
            return await UpdateAsync(entity as TEntity, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
