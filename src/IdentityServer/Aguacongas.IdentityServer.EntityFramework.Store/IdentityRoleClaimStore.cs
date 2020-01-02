using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.EntityFramework.Store
{
    public class IdentityRoleClaimStore<TUser, TRole> : IAdminStore<RoleClaim>
        where TRole : IdentityRole, new()
        where TUser : IdentityUser
    {
        private readonly RoleManager<TRole> _roleManager;
        private readonly IdentityDbContext<TUser, TRole, string> _context;
        private readonly ILogger<IdentityRoleClaimStore<TUser, TRole>> _logger;
        public IdentityRoleClaimStore(RoleManager<TRole> roleManager, 
            IdentityDbContext<TUser, TRole, string> context,
            ILogger<IdentityRoleClaimStore<TUser, TRole>> logger)
        {
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<RoleClaim> CreateAsync(RoleClaim entity, CancellationToken cancellationToken = default)
        {
            var role = await GetRoleAsync(entity.RoleId)
                .ConfigureAwait(false);
            var claim = entity.ToRoleClaim().ToClaim();
            var result = await _roleManager.AddClaimAsync(role, claim)
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
            return await CreateAsync(entity as RoleClaim, cancellationToken)
                .ConfigureAwait(false);
        }


        public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            var role = await GetRoleAsync(id)
                .ConfigureAwait(false);
            var claim = await GetClaimAsync(id, cancellationToken).ConfigureAwait(false);
            var result = await _roleManager.RemoveClaimAsync(role, claim.ToClaim())
                .ConfigureAwait(false);
            if (!result.Succeeded)
            {
                throw new IdentityException
                {
                    Errors = result.Errors
                };
            }
            _logger.LogInformation("Entity {EntityId} deleted", claim.Id, claim);
        }

        public async Task<RoleClaim> UpdateAsync(RoleClaim entity, CancellationToken cancellationToken = default)
        {
            var claim = await GetClaimAsync(entity.Id, cancellationToken).ConfigureAwait(false);
            var role = await GetRoleAsync(entity.Id)
                .ConfigureAwait(false);
            var result = await _roleManager.RemoveClaimAsync(role, claim.ToClaim())
                .ConfigureAwait(false);
            ChechResult(result, entity);
            result = await _roleManager.AddClaimAsync(role, entity.ToRoleClaim().ToClaim())
                .ConfigureAwait(false);
            _logger.LogInformation("Entity {EntityId} updated", entity.Id, entity);
            return ChechResult(result, entity);
        }

        public async Task<object> UpdateAsync(object entity, CancellationToken cancellationToken = default)
        {
            return await UpdateAsync(entity as RoleClaim, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<RoleClaim> GetAsync(string id, GetRequest request, CancellationToken cancellationToken = default)
        {
            var claim = await GetClaimAsync(id, cancellationToken).ConfigureAwait(false);
            if (claim == null)
            {
                return null;
            }
            return claim.ToEntity();
        }

        public async Task<PageResponse<RoleClaim>> GetAsync(PageRequest request, CancellationToken cancellationToken = default)
        {
            request = request ?? throw new ArgumentNullException(nameof(request));
            var odataQuery = _context.RoleClaims.GetODataQuery(request);

            var count = await odataQuery.CountAsync(cancellationToken).ConfigureAwait(false);

            var page = odataQuery.GetPage(request);

            var items = await page.ToListAsync(cancellationToken).ConfigureAwait(false);

            return new PageResponse<RoleClaim>
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

        private async Task<IdentityRoleClaim<string>> GetClaimAsync(string id, CancellationToken cancellationToken)
        {
            var claim = await _context.RoleClaims.FindAsync(new object[] { int.Parse(id) }, cancellationToken)
                            .ConfigureAwait(false);
            if (claim == null)
            {
                throw new DbUpdateException($"Entity type {typeof(RoleClaim).Name} at id {id} is not found");
            }

            return claim;
        }

        private TValue ChechResult<TValue>(IdentityResult result, TValue value)
        {
            if (result.Succeeded)
            {
                return value;
            }
            throw new IdentityException
            {
                Errors = result.Errors
            };
        }

    }
}
