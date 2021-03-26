// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.RavenDb.Store
{
    public class IdentityUserStore<TUser> : IAdminStore<Entity.User>
        where TUser : IdentityUser, new()
    {
        private readonly UserManager<TUser> _userManager;
        private readonly IAsyncDocumentSession _session;
        private readonly ILogger<IdentityUserStore<TUser>> _logger;

        public IdentityUserStore(UserManager<TUser> userManager,
            ScopedAsynDocumentcSession session,
            ILogger<IdentityUserStore<TUser>> logger)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _session = session?.Session ?? throw new ArgumentNullException(nameof(session));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Entity.User> CreateAsync(Entity.User entity, CancellationToken cancellationToken = default)
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
            return await CreateAsync(entity as Entity.User, cancellationToken)
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

        public async Task<Entity.User> GetAsync(string id, GetRequest request, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(id).ConfigureAwait(false);
            ICollection<Entity.UserClaim> claims = null;
            if (request.Expand.Contains(nameof(Entity.User.UserClaims)))
            {
                claims = await _session.Query<Entity.UserClaim>()
                    .Where(c => c.UserId == user.Id)
                    .ToListAsync()
                    .ConfigureAwait(false);
            }
            ICollection<Entity.UserRole> roles = null;
            if (request.Expand.Contains(nameof(Entity.User.UserRoles)))
            {
                roles = await _session.Query<Entity.UserRole>()
                    .Where(c => c.UserId == user.Id)
                    .ToListAsync()
                    .ConfigureAwait(false);
            }
            return user.ToUserEntity(claims, roles);
        }

        public async Task<PageResponse<Entity.User>> GetAsync(PageRequest request, CancellationToken cancellationToken = default)
        {
            request = request ?? throw new ArgumentNullException(nameof(request));
            
            var rql = request.ToRQL<TUser, string>(_session.Advanced.DocumentStore.Conventions.FindCollectionName(typeof(TUser)), i => i.Id);
            var pageQuery = _session.Advanced.AsyncRawQuery<TUser>(rql);
            if (request.Take.HasValue)
            {
                pageQuery = pageQuery.GetPage(request);
            }

            var items = await pageQuery.ToListAsync(cancellationToken).ConfigureAwait(false);

            var countQuery = _session.Advanced.AsyncRawQuery<TUser>(rql);
            var count = await countQuery.CountAsync(cancellationToken).ConfigureAwait(false);

            return new PageResponse<Entity.User>
            {
                Count = count,
                Items = items.Select(u => u.ToUserEntity(null, null))
            };
        }

        public async Task<Entity.User> UpdateAsync(Entity.User entity, CancellationToken cancellationToken = default)
        {
            var user = await GetUserAsync(entity.Id)
                .ConfigureAwait(false);
            user.Email = entity.Email;
            user.EmailConfirmed = entity.EmailConfirmed;
            user.TwoFactorEnabled = entity.TwoFactorEnabled;
            user.LockoutEnabled = entity.LockoutEnabled;

            if (entity.LockoutEnd.HasValue)
            {
                var lockoutEnd = entity.LockoutEnd.Value;
                user.LockoutEnd = lockoutEnd != DateTime.MinValue ? new DateTimeOffset(lockoutEnd) : DateTimeOffset.MinValue;
            }
            else
            {
                user.LockoutEnd = null;
            }
            user.PhoneNumber = entity.PhoneNumber;
            user.PhoneNumberConfirmed = entity.PhoneNumberConfirmed;
            user.SecurityStamp = entity.SecurityStamp;
            user.UserName = entity.UserName;
            user.NormalizedUserName = entity.NormalizedUserName;
            user.NormalizedEmail = entity.NormalizedEmail;
            user.ConcurrencyStamp = entity.ConcurrencyStamp;
            user.PasswordHash = entity.PasswordHash;
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
            return await UpdateAsync(entity as Entity.User, cancellationToken)
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
