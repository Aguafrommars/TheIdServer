using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
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
    public class IdentityUserStore<TUser> : IIdentityUserStore<TUser> where TUser : IdentityUser
    {
        private readonly UserManager<TUser> _userManager;

        public IdentityUserStore(UserManager<TUser> userManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            if (!userManager.SupportsQueryableUsers)
            {
                throw new ArgumentException($"{nameof(userManager)} must support queryable users");
            }
            if (!userManager.SupportsUserClaim)
            {
                throw new ArgumentException($"{nameof(userManager)} must support user claim");
            }
            if (!userManager.SupportsUserLogin)
            {
                throw new ArgumentException($"{nameof(userManager)} must support user login");
            }
            if (!userManager.SupportsUserRole)
            {
                throw new ArgumentException($"{nameof(userManager)} must support user role");
            }
        }

        public async Task<EntityClaim> AddClaimAsync(string userId, EntityClaim claim, CancellationToken cancellationToken = default)
        {
            var user = await GetUserAsync(userId);
            var result = await _userManager.AddClaimAsync(user, new Claim(claim.Type, claim.Value))
                .ConfigureAwait(false);
            return ChechResult(result, claim);
        }

        public async Task<string> AddRoleAsync(string userId, string role, CancellationToken cancellationToken = default)
        {
            var user = await GetUserAsync(userId);
            var result = await _userManager.AddToRoleAsync(user, role);
            return ChechResult(result, role);
        }

        public async Task<TUser> CreateAsync(TUser entity, CancellationToken cancellationToken = default)
        {
            var result = string.IsNullOrEmpty(entity.PasswordHash)
                ? await _userManager.CreateAsync(entity)
                    .ConfigureAwait(false)
                : await _userManager.CreateAsync(entity, entity.PasswordHash)
                    .ConfigureAwait(false);
            return ChechResult(result, entity);
        }

        public Task<IEntityId> CreateAsync(IEntityId entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            var user = await GetUserAsync(id);
            await _userManager.DeleteAsync(user)
                .ConfigureAwait(false);
        }

        public Task<TUser> GetAsync(string id, GetRequest request, CancellationToken cancellationToken = default)
        {
            return _userManager.Users
                .Expand(request?.Expand)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<PageResponse<TUser>> GetAsync(PageRequest request, CancellationToken cancellationToken = default)
        {
            request = request ?? throw new ArgumentNullException(nameof(request));
            var odataQuery = _userManager.Users.GetODataQuery(request);

            var count = await odataQuery.CountAsync(cancellationToken).ConfigureAwait(false);

            var page = odataQuery.GetPage(request);

            var items = (await page.ToListAsync(cancellationToken).ConfigureAwait(false)) as IEnumerable<TUser>;

            return new PageResponse<TUser>
            {
                Count = count,
                Items = items
            };
        }

        public async Task<PageResponse<EntityClaim>> GetClaimsAsync(string userId, PageRequest request, CancellationToken cancellationToken = default)
        {
            var user = await GetUserAsync(userId);
            var claims = await _userManager.GetClaimsAsync(user)
                .ConfigureAwait(false);

            return GetPage(request, claims
                .Select(c => new EntityClaim
                {
                    Type = c.Type,
                    Value = c.Value
                }));
        }

        public async Task<PageResponse<Login>> GetLoginsAsync(string userId, PageRequest request, CancellationToken cancellationToken = default)
        {
            var user = await GetUserAsync(userId);
            var logins = await _userManager.GetLoginsAsync(user)
                .ConfigureAwait(false);
            
            return GetPage(request, logins.Select(l => new Login
            {
                LoginProvider = l.LoginProvider,
                ProviderDisplayName = l.ProviderDisplayName,
                ProviderKey = l.ProviderKey
            }));
        }

        public async Task<PageResponse<string>> GetRolesAsync(string userId, PageRequest request, CancellationToken cancellationToken = default)
        {
            var user = await GetUserAsync(userId);
            var roles = await _userManager.GetRolesAsync(user)
                .ConfigureAwait(false);
            return new PageResponse<string>
            {
                Count = roles.Count,
                Items = roles.Where(r => r.Contains(request.Filter))
                    .Skip(request.Skip ?? 0)
                    .Take(request.Take)
            };
        }

        public async Task RemoveClaimAsync(string userId, EntityClaim claim, CancellationToken cancellationToken = default)
        {
            var user = await GetUserAsync(userId);
            var result = await _userManager.RemoveClaimAsync(user, new Claim(claim.Type, claim.Value));
            ChechResult(result, claim);
        }

        public async Task RemoveLoginAsync(string userId, Login login, CancellationToken cancellationToken = default)
        {
            var user = await GetUserAsync(userId);
            var result = await _userManager.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey);
            ChechResult(result, login);
        }

        public async Task RemoveRoleAsync(string userId, string role, CancellationToken cancellationToken = default)
        {
            var user = await GetUserAsync(userId);
            var result = await _userManager.RemoveFromRoleAsync(user, role);
            ChechResult(result, role);
        }

        public async Task<TUser> UpdateAsync(TUser entity, CancellationToken cancellationToken = default)
        {
            var result = await _userManager.UpdateAsync(entity)
                .ConfigureAwait(false);
            return ChechResult(result, entity);
        }

        public Task<IEntityId> UpdateAsync(IEntityId entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        private async Task<TUser> GetUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId)
                .ConfigureAwait(false);
            if (user == null)
            {
                throw new IdentityException($"User {userId} not found");
            }

            return user;
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
        private static PageResponse<T> GetPage<T>(PageRequest request, IEnumerable<T> roles) where T : class
        {
            var odataQuery = roles.AsQueryable().GetODataQuery(request);

            var count = odataQuery.Count();

            var page = odataQuery.GetPage(request);

            return new PageResponse<T>
            {
                Count = count,
                Items = page
            };
        }
    }
}
