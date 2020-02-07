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
    public class IdentityUserStore<TUser> : IAdminStore<User>
        where TUser : IdentityUser, new()
    {
        private readonly UserManager<TUser> _userManager;
        private readonly IdentityDbContext<TUser> _context;
        private readonly ILogger<IdentityUserStore<TUser>> _logger;

        public IdentityUserStore(UserManager<TUser> userManager, 
            IdentityDbContext<TUser> context,
            ILogger<IdentityUserStore<TUser>> logger)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<User> CreateAsync(User entity, CancellationToken cancellationToken = default)
        {
            var user = entity.ToUser<TUser>();
            user.Id = Guid.NewGuid().ToString();
            var result = string.IsNullOrEmpty(entity.Password)
                ? await _userManager.CreateAsync(user)
                    .ConfigureAwait(false)
                : await _userManager.CreateAsync(user, entity.Password)
                    .ConfigureAwait(false);
            if (result.Succeeded)
            {
                entity.Id = user.Id;
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
            return await CreateAsync(entity as User, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            var user = await GetUserAsync(id)
                .ConfigureAwait(false);
            var result = await _userManager.DeleteAsync(user)
                .ConfigureAwait(false);
            if (result.Succeeded)
            {
                _logger.LogInformation("Entity {EntityId} deleted", user.Id, user);
                return;
            }
            throw new IdentityException
            {
                Errors = result.Errors
            };
        }

        public async Task<User> GetAsync(string id, GetRequest request, CancellationToken cancellationToken = default)
        {
            var user = await _context.Users.FindAsync(new object[] { id }, cancellationToken)
                .ConfigureAwait(false);
            return user.ToUserEntity();
        }

        public async Task<PageResponse<User>> GetAsync(PageRequest request, CancellationToken cancellationToken = default)
        {
            request = request ?? throw new ArgumentNullException(nameof(request));
            var odataQuery = _userManager.Users.AsNoTracking().GetODataQuery(request);

            var count = await odataQuery.CountAsync(cancellationToken).ConfigureAwait(false);

            var page = odataQuery.GetPage(request);

            var items = await page.ToListAsync(cancellationToken).ConfigureAwait(false);

            return new PageResponse<User>
            {
                Count = count,
                Items = items.Select(u => u.ToUserEntity())
            };
        }

        public async Task<User> UpdateAsync(User entity, CancellationToken cancellationToken = default)
        {
            var user = await GetUserAsync(entity.Id)
                .ConfigureAwait(false);
            user.Email = entity.Email;
            user.EmailConfirmed = entity.EmailConfirmed;
            user.TwoFactorEnabled = entity.TwoFactorEnabled;
            user.LockoutEnabled = entity.LockoutEnabled;
            user.LockoutEnd = entity.LockoutEnd;
            user.PhoneNumber = entity.PhoneNumber;
            user.PhoneNumberConfirmed = entity.PhoneNumberConfirmed;
            var result = await _userManager.UpdateAsync(user)
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
            return await UpdateAsync(entity as User, cancellationToken)
                .ConfigureAwait(false);
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
    }
}
