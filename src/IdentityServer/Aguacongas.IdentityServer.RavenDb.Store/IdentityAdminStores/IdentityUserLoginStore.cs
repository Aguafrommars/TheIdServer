// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Community.OData.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.OData.Edm;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.RavenDb.Store
{
    public class IdentityUserLoginStore<TUser> : IAdminStore<UserLogin>
        where TUser : IdentityUser, new()
    {
        private readonly UserManager<TUser> _userManager;
        private readonly IAsyncDocumentSession _session;
        private readonly ILogger<IdentityUserLoginStore<TUser>> _logger;
        [SuppressMessage("Major Code Smell", "S2743:Static fields should not be used in generic types", Justification = "We use only one type of TUser")]
        private static readonly IEdmModel _edmModel = GetEdmModel();

        public IdentityUserLoginStore(UserManager<TUser> userManager,
            IAsyncDocumentSession session,
            ILogger<IdentityUserLoginStore<TUser>> logger)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<UserLogin> CreateAsync(UserLogin entity, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(entity.UserId).ConfigureAwait(false);
            if (user ==  null)
            {
                throw new IdentityException($"User at id {entity.UserId} is not found.");
            }

            var result =  await _userManager.AddLoginAsync(user, new UserLoginInfo
            (
                entity.LoginProvider,
                entity.ProviderKey,
                entity.ProviderDisplayName
            )).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                throw new IdentityException
                {
                    Errors = result.Errors
                };
            }
            entity.Id = $"{entity.UserId}@{entity.LoginProvider}@{entity.ProviderKey}";
            return entity;
        }

        public async Task<object> CreateAsync(object entity, CancellationToken cancellationToken = default)
        {
            return await CreateAsync(entity as UserLogin, cancellationToken).ConfigureAwait(false);
        }

        public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            var login = await GetLoginAsync(id, cancellationToken).ConfigureAwait(false);
            var user = await GetUserAsync(login.UserId)
                .ConfigureAwait(false);
            var result = await _userManager.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey)
                .ConfigureAwait(false);
            if (!result.Succeeded)
            {
                throw new IdentityException
                {
                    Errors = result.Errors
                };
            }
            _logger.LogInformation("Entity {EntityId} deleted", id, login);
        }

        public Task<UserLogin> UpdateAsync(UserLogin entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<object> UpdateAsync(object entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
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

            var rql = request.ToRQL<IdentityUserLogin<string>, string>(_session.Advanced.DocumentStore.Conventions.FindCollectionName(typeof(IdentityUserLogin<string>)), i => i.UserId);
            var pageQuery = _session.Advanced.AsyncRawQuery<IdentityUserLogin<string>>(rql);
            if (request.Take.HasValue)
            {
                pageQuery = pageQuery.GetPage(request);
            }

            var items = await pageQuery.ToListAsync(cancellationToken).ConfigureAwait(false);

            var countQuery = _session.Advanced.AsyncRawQuery<IdentityUserLogin<string>>(rql);
            var count = await countQuery.CountAsync(cancellationToken).ConfigureAwait(false);

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

        private Task<IdentityUserLogin<string>> GetLoginAsync(string id, CancellationToken cancellationToken)
        {
            return _session.LoadAsync<IdentityUserLogin<string>>($"userlogin/{id}", cancellationToken);
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
