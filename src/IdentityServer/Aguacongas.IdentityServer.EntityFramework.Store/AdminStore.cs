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
    public class AdminStore<T> : IAdminStore<T> where T: class, IEntityId, new()
    {
        private readonly DbContext _context;
        private readonly ILogger<AdminStore<T>> _logger;

        public AdminStore(DbContext context, ILogger<AdminStore<T>> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task<T> GetAsync(string id, GetRequest request, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<T>().AsNoTracking();
            query = query.Expand(request?.Expand);
            return query.Where(e => e.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<PageResponse<T>> GetAsync(PageRequest request, CancellationToken cancellationToken = default)
        {
            request = request ?? throw new ArgumentNullException(nameof(request));
            var query = _context.Set<T>().AsNoTracking();
            var odataQuery = query.GetODataQuery(request);

            var count = await odataQuery.CountAsync(cancellationToken).ConfigureAwait(false);

            var page = odataQuery.GetPage(request);

            var items = (await page.ToListAsync(cancellationToken).ConfigureAwait(false)) as IEnumerable<T>;

            return new PageResponse<T>
            {
                Count = count,
                Items = items
            };
        }

        public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            var entity = await _context.Set<T>().FindAsync(new[] { id }, cancellationToken).ConfigureAwait(false);
            if (entity == null)
            {
                throw new DbUpdateException($"Entity type {typeof(T).Name} at id {id} is not found");
            }
            _context.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            _logger.LogInformation("Entity {EntityId} deleted", entity.Id, entity);
        }

        public async Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default)
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
            return await CreateAsync(entity as T, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            entity = entity ?? throw new ArgumentNullException(nameof(entity));
            var storedEntity = await _context.Set<T>()
                .AsNoTracking()
                .Where(e => e.Id == entity.Id)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);
            if (storedEntity == null)
            {
                throw new DbUpdateException($"Entity type {typeof(T).Name} at id {entity.Id} is not found");
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
            return await UpdateAsync(entity as T, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
