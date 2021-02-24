// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store
{
    public class IdentityUserClaimStore<TUser> : IAdminStore<Entity.UserClaim>
        where TUser : IdentityUser, new()
    {
        private readonly UserManager<TUser> _userManager;
        private readonly IAsyncDocumentSession _session;
        private readonly ILogger<IdentityUserClaimStore<TUser>> _logger;
        
        public IdentityUserClaimStore(UserManager<TUser> userManager,
            IAsyncDocumentSession session,
            ILogger<IdentityUserClaimStore<TUser>> logger)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Entity.UserClaim> CreateAsync(Entity.UserClaim entity, CancellationToken cancellationToken = default)
        {
            var user = await GetUserAsync(entity.UserId)
                .ConfigureAwait(false);
            var claim = entity.ToUserClaim().ToClaim();
            var result = await _userManager.AddClaimAsync(user, claim)
                .ConfigureAwait(false);
            if (result.Succeeded)
            {
                entity.Id = Guid.NewGuid().ToString();
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
            return await CreateAsync(entity as Entity.UserClaim, cancellationToken)
                .ConfigureAwait(false);
        }


        public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            var claim = await GetClaimAsync(id, cancellationToken).ConfigureAwait(false);
            if (claim == null)
            {
                return;
            }


            var user = await GetUserAsync(claim.UserId)
                .ConfigureAwait(false);
            var result = await _userManager.RemoveClaimAsync(user, claim.ToClaim())
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

        public async Task<Entity.UserClaim> UpdateAsync(Entity.UserClaim entity, CancellationToken cancellationToken = default)
        {
            var claim = await GetClaimAsync(entity.Id, cancellationToken).ConfigureAwait(false);
            if (claim == null)
            {
                throw new InvalidOperationException($"Entity type {typeof(UserClaim).Name} at id {entity.Id} is not found");
            }

            var user = await GetUserAsync(entity.UserId)
                .ConfigureAwait(false);
            var result = await _userManager.RemoveClaimAsync(user, claim.ToClaim())
                .ConfigureAwait(false);
            ChechResult(result);
            result = await _userManager.AddClaimAsync(user, entity.ToUserClaim().ToClaim())
                .ConfigureAwait(false);
            ChechResult(result);
            _logger.LogInformation("Entity {EntityId} updated", entity.Id, entity);
            return entity;
        }

        public async Task<object> UpdateAsync(object entity, CancellationToken cancellationToken = default)
        {
            return await UpdateAsync(entity as Entity.UserClaim, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<Entity.UserClaim> GetAsync(string id, GetRequest request, CancellationToken cancellationToken = default)
        {
            var claim = await GetClaimAsync(id, cancellationToken).ConfigureAwait(false);
            if (claim == null)
            {
                return null;
            }
            return claim.ToEntity();
        }

        public async Task<PageResponse<Entity.UserClaim>> GetAsync(PageRequest request, CancellationToken cancellationToken = default)
        {
            request = request ?? throw new ArgumentNullException(nameof(request));
            var odataQuery = _session.Query<UserClaim>().GetODataQuery(request);

            var count = await odataQuery.CountAsync(cancellationToken).ConfigureAwait(false);

            var page = odataQuery.GetPage(request);

            var items = await page.ToListAsync(cancellationToken).ConfigureAwait(false);

            return new PageResponse<Entity.UserClaim>
            {
                Count = count,
                Items = items.Select(r => r.ToEntity())
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

        private async Task<UserClaim> GetClaimAsync(string id, CancellationToken cancellationToken)
        {
            return await _session.LoadAsync<UserClaim>(id, cancellationToken)
                            .ConfigureAwait(false);
        }

        private static void ChechResult(IdentityResult result)
        {
            if (!result.Succeeded)
            {
                throw new IdentityException
                {
                    Errors = result.Errors
                };
            }
        }
    }
}
