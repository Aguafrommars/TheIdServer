using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.EntityFramework.Store
{
    public class IdentityRoleStore<TUser, TRole> : IAdminStore<Role>
        where TRole : IdentityRole, new()
        where TUser : IdentityUser
    {
        private readonly RoleManager<TRole> _roleManager;
        private readonly IdentityDbContext<TUser, TRole> _context;
        private readonly ILogger<IdentityRoleStore<TUser, TRole>> _logger;
        public IdentityRoleStore(RoleManager<TRole> roleManager, 
            IdentityDbContext<TUser, TRole> context,
            ILogger<IdentityRoleStore<TUser, TRole>> logger)
        {
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Role> CreateAsync(Role entity, CancellationToken cancellationToken = default)
        {
            var role = entity.ToRole<TRole>();
            role.Id = Guid.NewGuid().ToString();
            var result = await _roleManager.CreateAsync(role)
                .ConfigureAwait(false);
            if (result.Succeeded)
            {
                entity.Id = role.Id;
                _logger.LogInformation("Entity {EntityId} created", entity.Id, entity);
                return entity;
            }
            throw new IdentityException
            {
                Errors = result.Errors
            };
        }

        public async Task<object> CreateAsync(object entity, CancellationToken cancellationToken = default)
        {
            return await CreateAsync(entity as Role, cancellationToken)
                .ConfigureAwait(false);
        }


        public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            var role = await GetRoleAsync(id)
                .ConfigureAwait(false);
            await _roleManager.DeleteAsync(role)
                .ConfigureAwait(false);
            _logger.LogInformation("Entity {EntityId} deleted", role.Id, role);
        }

        public async Task<Role> UpdateAsync(Role entity, CancellationToken cancellationToken = default)
        {
            var role = await GetRoleAsync(entity.Id)
                .ConfigureAwait(false);
            role.Name = entity.Name;
            role.ConcurrencyStamp = entity.ConcurrencyStamp;
            role.NormalizedName = entity.NormalizedName;
            var result = await _roleManager.UpdateAsync(role)
                .ConfigureAwait(false);
            if (result.Succeeded)
            {
                _logger.LogInformation("Entity {EntityId} updated", entity.Id, entity);
                return entity;
            }
            throw new IdentityException
            {
                Errors = result.Errors
            };
        }

        public async Task<object> UpdateAsync(object entity, CancellationToken cancellationToken = default)
        {
            return await UpdateAsync(entity as Role, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<Role> GetAsync(string id, GetRequest request, CancellationToken cancellationToken = default)
        {
            var role = await _context.Roles.FindAsync(new object[] { id }, cancellationToken)
                .ConfigureAwait(false);
            return role.ToEntity();
        }

        public async Task<PageResponse<Role>> GetAsync(PageRequest request, CancellationToken cancellationToken = default)
        {
            request = request ?? throw new ArgumentNullException(nameof(request));
            var odataQuery = _context.Roles.AsNoTracking().GetODataQuery(request);

            var count = await odataQuery.CountAsync(cancellationToken).ConfigureAwait(false);

            var page = odataQuery.GetPage(request);

            var items = await page.ToListAsync(cancellationToken).ConfigureAwait(false);

            return new PageResponse<Role>
            {
                Count = count,
                Items = items.Select(r => r.ToEntity())
            };
        }

        private async Task<TRole> GetRoleAsync(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId)
                .ConfigureAwait(false);
            if (role == null)
            {
                throw new IdentityException($"Role {roleId} not found");
            }

            return role;
        }
    }
}
