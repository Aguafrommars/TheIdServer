using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Community.OData.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.OData.Edm;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.EntityFramework.Store
{
    public class IdentityUserLoginStore<TUser> : IAdminStore<UserLogin>
        where TUser : IdentityUser, new()
    {
        private readonly UserManager<TUser> _userManager;
        private readonly IdentityDbContext<TUser> _context;
        private readonly ILogger<IdentityUserLoginStore<TUser>> _logger;
        [SuppressMessage("Major Code Smell", "S2743:Static fields should not be used in generic types", Justification = "We use only one type of TUser")]
        private static readonly IEdmModel _edmModel = GetEdmModel();

        public IdentityUserLoginStore(UserManager<TUser> userManager, 
            IdentityDbContext<TUser> context,
            ILogger<IdentityUserLoginStore<TUser>> logger)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<UserLogin> CreateAsync(UserLogin entity, CancellationToken cancellationToken = default)
        {
            var user = await GetUserAsync(entity.UserId);
            var login = entity.ToUserLoginInfo();
            var result = await _userManager.AddLoginAsync(user, login)
                .ConfigureAwait(false);
            if (result.Succeeded)
            {
                entity.Id = $"{entity.UserId}@{entity.LoginProvider}@{entity.ProviderKey}";
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
            return await CreateAsync(entity as UserLogin, cancellationToken)
                .ConfigureAwait(false);
        }


        public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            var login = await GetLoginAsync(id, cancellationToken).ConfigureAwait(false);
            var user = await GetUserAsync(login.UserId);
            var result = await _userManager.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey);
            if (!result.Succeeded)
            {
                throw new IdentityException
                {
                    Errors = result.Errors
                };
            }
            _logger.LogInformation("Entity {EntityId} deleted", id, login);
        }

        public async Task<UserLogin> UpdateAsync(UserLogin entity, CancellationToken cancellationToken = default)
        {
            var login = await GetLoginAsync(entity.Id, cancellationToken).ConfigureAwait(false);
            var user = await GetUserAsync(entity.Id);
            var result = await _userManager.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey)
                .ConfigureAwait(false);
            ChechResult(result);
            result = await _userManager.AddLoginAsync(user, entity.ToUserLoginInfo())
                .ConfigureAwait(false);
            ChechResult(result);
            _logger.LogInformation("Entity {EntityId} updated", entity.Id, entity);
            return entity;
        }

        public async Task<object> UpdateAsync(object entity, CancellationToken cancellationToken = default)
        {
            return await UpdateAsync(entity as UserLogin, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<UserLogin> GetAsync(string id, GetRequest request, CancellationToken cancellationToken = default)
        {
            var claim = await GetLoginAsync(id, cancellationToken).ConfigureAwait(false);
            if (claim == null)
            {
                return null;
            }
            return claim.ToEntity();
        }

        public async Task<PageResponse<UserLogin>> GetAsync(PageRequest request, CancellationToken cancellationToken = default)
        {
            request = request ?? throw new ArgumentNullException(nameof(request));
            var odataQuery = _context.UserLogins.GetODataQuery(request, _edmModel);

            var count = await odataQuery.CountAsync(cancellationToken).ConfigureAwait(false);

            var page = odataQuery.GetPage(request);

            var items = await page.ToListAsync(cancellationToken).ConfigureAwait(false);

            return new PageResponse<UserLogin>
            {
                Count = count,
                Items = items.Select(l => l.ToEntity())
            };
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

        private async Task<IdentityUserLogin<string>> GetLoginAsync(string id, CancellationToken cancellationToken)
        {
            var info = id.Split('@');
            var login = await _context.UserLogins.FirstOrDefaultAsync(l => l.UserId == info[0] &&
                l.LoginProvider == info[1] &&
                l.ProviderKey == info[2], cancellationToken)
                            .ConfigureAwait(false);
            if (login == null)
            {
                throw new DbUpdateException($"Entity type {typeof(UserLogin).Name} at id {id} is not found");
            }

            return login;
        }

        private void ChechResult(IdentityResult result)
        {
            if (!result.Succeeded)
            {
                throw new IdentityException
                {
                    Errors = result.Errors
                };
            }
        }

        private static IEdmModel GetEdmModel()
        {
            var builder = new ODataConventionModelBuilder();
            var entitySet = builder.EntitySet<IdentityUserLogin<string>>(typeof(IdentityUserLogin<string>).Name);
            entitySet.EntityType.HasKey(e => e.UserId);
            entitySet.EntityType.HasKey(e => e.LoginProvider);
            entitySet.EntityType.HasKey(e => e.ProviderKey);
            return builder.GetEdmModel();
        }
    }
}
