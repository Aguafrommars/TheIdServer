using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Community.OData.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.EntityFramework.Store
{
    public class IdentityRoleStore<TRole> : IIdentityRoleStore<TRole> where TRole : IdentityRole
    {
        private readonly RoleManager<TRole> _roleManager;

        public IdentityRoleStore(RoleManager<TRole> roleManager)
        {
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            if (!roleManager.SupportsQueryableRoles)
            {
                throw new ArgumentException($"{nameof(roleManager)} must support queryable users");
            }
            if (!roleManager.SupportsRoleClaims)
            {
                throw new ArgumentException($"{nameof(roleManager)} must support role claims");
            }
        }

        public async Task<EntityClaim> AddClaimAsync(string roleId, EntityClaim claim, CancellationToken cancellationToken = default)
        {
            var role = await GetRoleAsync(roleId);
            var result = await _roleManager.AddClaimAsync(role, new Claim(claim.Type, claim.Value))
                .ConfigureAwait(false);
            return ChechResult(result, claim);
        }

        public async Task<TRole> CreateAsync(TRole entity, CancellationToken cancellationToken = default)
        {
            var result = await _roleManager.CreateAsync(entity)
                .ConfigureAwait(false);
            if (result.Succeeded)
            {
                return entity;
            }
            throw new IdentityException
            {
                Errors = result.Errors
            };
        }

        public Task<IEntityId> CreateAsync(IEntityId entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            var role = await GetRoleAsync(id);
            await _roleManager.DeleteAsync(role)
                .ConfigureAwait(false);
        }

        public Task<TRole> GetAsync(string id, GetRequest request, CancellationToken cancellationToken = default)
        {
            return _roleManager.Roles
                .Expand(request?.Expand)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<PageResponse<TRole>> GetAsync(PageRequest request, CancellationToken cancellationToken = default)
        {
            request = request ?? throw new ArgumentNullException(nameof(request));
            var query = _roleManager.Roles;
            var odataQuery = query.GetODataQuery(request);

            var count = await odataQuery.CountAsync(cancellationToken).ConfigureAwait(false);

            var page = odataQuery.GetPage(request);

            var items = (await page.ToListAsync(cancellationToken).ConfigureAwait(false)) as IEnumerable<TRole>;

            return new PageResponse<TRole>
            {
                Count = count,
                Items = items
            };
        }

        public async Task<PageResponse<EntityClaim>> GetClaimsAsync(string roleId, PageRequest request, CancellationToken cancellationToken = default)
        {
            var role = await GetRoleAsync(roleId);
            var claims = await _roleManager.GetClaimsAsync(role)
                .ConfigureAwait(false);

            var odataQuery = claims.AsQueryable().GetODataQuery(request);

            var count = odataQuery.Count();

            var page = odataQuery.GetPage(request);

            return new PageResponse<EntityClaim>
            {
                Count = count,
                Items = page.Select(c => new EntityClaim { Type = c.Type, Value = c.Value})
            };
        }

        public async Task RemoveClaimAsync(string roleId, EntityClaim claim, CancellationToken cancellationToken = default)
        {
            var role = await GetRoleAsync(roleId);
            var result = await _roleManager.RemoveClaimAsync(role, new Claim(claim.Type, claim.Value));
            ChechResult(result, claim);
        }

        public async Task<TRole> UpdateAsync(TRole entity, CancellationToken cancellationToken = default)
        {
            var result = await _roleManager.UpdateAsync(entity)
                .ConfigureAwait(false);
            return ChechResult(result, entity);
        }

        public Task<IEntityId> UpdateAsync(IEntityId entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
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
