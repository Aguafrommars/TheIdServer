using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Community.OData.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.EntityFramework.Store
{
    [SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "No localization")]
    public class AdminStore<T> : IAdminStore<T> where T: class, IEntityId
    {
        private readonly IdentityServerDbContext _context;
        private readonly ILogger<AdminStore<T>> _logger;

        public AdminStore(IdentityServerDbContext context, ILogger<AdminStore<T>> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task<T> GetAsync(string id)
        {
            return _context.Set<T>().FindAsync(id).AsTask();
        }

        public async Task<PageResponse<T>> GetAsync(PageRequest request)
        {
            request = request ?? throw new ArgumentNullException(nameof(request));
            var query = _context.Set<T>().OData();
            if (!string.IsNullOrEmpty(request.Filter))
            {
                query = query.Filter(request.Filter);
            }
            if (!string.IsNullOrEmpty(request.OrderBy))
            {
                query = query.OrderBy(request.OrderBy);
            }
            
            var page = query.Skip(request.Skip.HasValue ? request.Skip.Value : 0);
            if (request.Take.HasValue)
            {
                page = page.Take(request.Take.Value);
            }

            return new PageResponse<T>
            {
                Count = await page.CountAsync().ConfigureAwait(false),
                Items = await page.ToListAsync().ConfigureAwait(false)
            };
        }

        public async Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default)
        {
            entity = entity ?? throw new ArgumentNullException(nameof(entity));
            await _context.AddAsync(entity, cancellationToken).ConfigureAwait(false);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            _logger.LogInformation("Entity {EntityId} created", entity.Id, entity);
            return entity;
        }

        public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            var entity = await _context.Set<T>().FindAsync(id).ConfigureAwait(false);
            if (entity != null)
            {
                _context.Remove(entity);
                await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                _logger.LogInformation("Entity {EntityId} deleted", entity.Id, entity);
            }
        }

        public async Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            entity = entity ?? throw new ArgumentNullException(nameof(entity));
            _context.Update(entity);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            _logger.LogInformation("Entity {EntityId} updated", entity.Id, entity);
            return entity;
        }
    }
}
