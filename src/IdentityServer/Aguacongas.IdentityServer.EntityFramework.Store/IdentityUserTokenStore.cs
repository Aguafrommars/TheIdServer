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
    public class IdentityUserTokenStore<TUser> : IAdminStore<UserToken>
        where TUser : IdentityUser
    {
        private readonly IdentityDbContext<TUser> _context;
        private readonly ILogger<IdentityUserTokenStore<TUser>> _logger;
        [SuppressMessage("Major Code Smell", "S2743:Static fields should not be used in generic types", Justification = "We use only one type of TUser")]
        private static readonly IEdmModel _edmModel = GetEdmModel();

        public IdentityUserTokenStore(IdentityDbContext<TUser> context,
            ILogger<IdentityUserTokenStore<TUser>> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task<UserToken> CreateAsync(UserToken entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<object> CreateAsync(object entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }


        public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            var token = await GetTokenAsync(id, cancellationToken).ConfigureAwait(false);
            _context.UserTokens.Remove(token);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Entity {EntityId} deleted", id, token);
        }

        public Task<UserToken> UpdateAsync(UserToken entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<object> UpdateAsync(object entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<UserToken> GetAsync(string id, GetRequest request, CancellationToken cancellationToken = default)
        {
            var token = await GetTokenAsync(id, cancellationToken).ConfigureAwait(false);
            if (token == null)
            {
                return null;
            }
            return token.ToEntity();
        }

        public async Task<PageResponse<UserToken>> GetAsync(PageRequest request, CancellationToken cancellationToken = default)
        {
            request = request ?? throw new ArgumentNullException(nameof(request));
            var query = _context.UserTokens;
            var odataQuery = query.GetODataQuery(request, _edmModel);

            var count = await odataQuery.CountAsync(cancellationToken).ConfigureAwait(false);

            var page = odataQuery.GetPage(request);

            var items = await page.ToListAsync(cancellationToken).ConfigureAwait(false);

            return new PageResponse<UserToken>
            {
                Count = count,
                Items = items.Select(t => t.ToEntity())
            };
        }

        private async Task<IdentityUserToken<string>> GetTokenAsync(string id, CancellationToken cancellationToken)
        {
            var info = id.Split('@');
            var login = await _context.UserTokens.FirstOrDefaultAsync(l => l.UserId == info[0] &&
                l.LoginProvider == info[1] &&
                l.Name == info[2], cancellationToken)
                            .ConfigureAwait(false);
            if (login == null)
            {
                throw new DbUpdateException($"Entity type {typeof(UserLogin).Name} at id {id} is not found");
            }

            return login;
        }

        private static IEdmModel GetEdmModel()
        {
            var builder = new ODataConventionModelBuilder();
            var entitySet = builder.EntitySet<IdentityUserToken<string>>(typeof(IdentityUserToken<string>).Name);
            entitySet.EntityType.HasKey(e => e.UserId);
            entitySet.EntityType.HasKey(e => e.LoginProvider);
            entitySet.EntityType.HasKey(e => e.Name);
            return builder.GetEdmModel();
        }

    }
}
