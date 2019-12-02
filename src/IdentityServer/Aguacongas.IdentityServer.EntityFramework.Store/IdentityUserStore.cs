using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Community.OData.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.EntityFramework.Store
{
    public class IdentityUserStore<TUser> : IAdminStore<TUser> where TUser : IdentityUser
    {
        private readonly UserManager<TUser> _userManager;

        public IdentityUserStore(UserManager<TUser> userManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            if (!userManager.SupportsQueryableUsers)
            {
                throw new ArgumentException("userManager must support queryable users");
            }
        }
        public async Task<TUser> CreateAsync(TUser entity, CancellationToken cancellationToken = default)
        {
            var result = await _userManager.CreateAsync(entity);
            if (result.Succeeded)
            {
                return entity;
            }
            throw new AggregateException(result.Errors.Select(e => new IdentityUserException($"{e.Code} {e.Description}")));
        }

        public Task<IEntityId> CreateAsync(IEntityId entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
        }

        public Task<TUser> GetAsync(string id, GetRequest request, CancellationToken cancellationToken = default)
        {
            return _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<PageResponse<TUser>> GetAsync(PageRequest request, CancellationToken cancellationToken = default)
        {
            request = request ?? throw new ArgumentNullException(nameof(request));
            var query = _userManager.Users;
            query = query.Expand(request.Expand);

            var odataQuery = query.OData();

            if (!string.IsNullOrEmpty(request.Filter))
            {
                odataQuery = odataQuery.Filter(request.Filter);
            }
            if (!string.IsNullOrEmpty(request.OrderBy))
            {
                odataQuery = odataQuery.OrderBy(request.OrderBy);
            }

            var count = await odataQuery.CountAsync(cancellationToken).ConfigureAwait(false);

            var page = odataQuery.Skip(request.Skip ?? 0);
            if (request.Take.HasValue)
            {
                page = page.Take(request.Take.Value);
            }

            var items = (await page.ToListAsync(cancellationToken).ConfigureAwait(false)) as IEnumerable<TUser>;

            return new PageResponse<TUser>
            {
                Count = count,
                Items = items
            };
        }

        public async Task<TUser> UpdateAsync(TUser entity, CancellationToken cancellationToken = default)
        {
            var result = await _userManager.UpdateAsync(entity);
            if (result.Succeeded)
            {
                return entity;
            }
            throw new AggregateException(result.Errors.Select(e => new IdentityUserException($"{e.Code} {e.Description}")));
        }

        public Task<IEntityId> UpdateAsync(IEntityId entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
