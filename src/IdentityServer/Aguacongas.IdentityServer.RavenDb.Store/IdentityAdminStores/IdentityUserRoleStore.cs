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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.RavenDb.Store
{
    public class IdentityUserRoleStore<TUser> : IAdminStore<UserRole>
        where TUser : IdentityUser, new()
    {
        private readonly UserManager<TUser> _userManager;
        private readonly IAsyncDocumentSession _session;
        private readonly ILogger<IdentityUserRoleStore<TUser>> _logger;
        [SuppressMessage("Major Code Smell", "S2743:Static fields should not be used in generic types", Justification = "We use only one type of TUser")]
        private static readonly IEdmModel _edmModel = GetEdmModel();
        public IdentityUserRoleStore(UserManager<TUser> userManager,
            IAsyncDocumentSession session,
            ILogger<IdentityUserRoleStore<TUser>> logger)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<UserRole> CreateAsync(UserRole entity, CancellationToken cancellationToken = default)
        {
            var user = await GetUserAsync(entity.UserId)
                .ConfigureAwait(false);
            var role = await GetRoleAsync(entity.RoleId, cancellationToken)
                .ConfigureAwait(false);


            var result = await _userManager.AddToRoleAsync(user, role.Name)
                .ConfigureAwait(false);                
            if (result.Succeeded)
            {
                entity.Id = $"{role.Name}@{user.Id}";
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
            return await CreateAsync(entity as UserRole, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            var info = id.Split('@');
            var role = await GetRoleAsync(info[1], cancellationToken).ConfigureAwait(false);
            var user = await GetUserAsync(info[0])
                .ConfigureAwait(false);
            var result = await _userManager.RemoveFromRoleAsync(user, role.Name)
                .ConfigureAwait(false);
            if (!result.Succeeded)
            {
                throw new IdentityException
                {
                    Errors = result.Errors
                };
            }
            _logger.LogInformation("Entity {EntityId} deleted", id, role);
        }

        public async Task<UserRole> UpdateAsync(UserRole entity, CancellationToken cancellationToken = default)
        {
            await DeleteAsync(entity.Id, cancellationToken).ConfigureAwait(false);
            return await CreateAsync(entity, cancellationToken).ConfigureAwait(false);
        }

        public async Task<object> UpdateAsync(object entity, CancellationToken cancellationToken = default)
        {
            return await UpdateAsync(entity as UserRole, cancellationToken).ConfigureAwait(false);
        }

        public async Task<UserRole> GetAsync(string id, GetRequest request, CancellationToken cancellationToken = default)
        {
            var role = await _session.LoadAsync<IdentityUserRole<string>>($"userrole/{id}", builder => builder.IncludeDocuments(r => $"role/{r.RoleId}"), cancellationToken)
                            .ConfigureAwait(false);
            if (role == null)
            {
                return null;
            }

            return role.ToEntity(await GetRoleAsync(role.RoleId, cancellationToken).ConfigureAwait(false));
        }

        public async Task<PageResponse<UserRole>> GetAsync(PageRequest request, CancellationToken cancellationToken = default)
        {
            request = request ?? throw new ArgumentNullException(nameof(request));

            var rql = request.ToRQL<IdentityUserRole<string>, string>(_session.Advanced.DocumentStore.Conventions.FindCollectionName(typeof(IdentityUserRole<string>)), i => i.RoleId);
            var pageQuery = _session.Advanced.AsyncRawQuery<IdentityUserRole<string>>(rql);
            if (request.Take.HasValue)
            {
                pageQuery = pageQuery.GetPage(request);
            }

            var items = await pageQuery.ToListAsync(cancellationToken).ConfigureAwait(false);

            var countQuery = _session.Advanced.AsyncRawQuery<IdentityUserRole<string>>(rql);
            var count = await countQuery.CountAsync(cancellationToken).ConfigureAwait(false);

            var roles = new List<UserRole>(items.Count);
            foreach(var userRole in items)
            {
                roles.Add(userRole.ToEntity(await GetRoleAsync(userRole.RoleId, cancellationToken).ConfigureAwait(false)));
            }
            return new PageResponse<UserRole>
            {
                Count = count,
                Items = roles
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

        private async Task<IdentityRole> GetRoleAsync(string id, CancellationToken cancellationToken)
        {
            var role = await _session.LoadAsync<IdentityRole>($"role/{id}", cancellationToken)
                .ConfigureAwait(false);

            if (role == null)
            {
                throw new InvalidOperationException($"Entity type {typeof(UserRole).Name} at id {id} is not found");
            }

            return role;
        }

        private static IEdmModel GetEdmModel()
        {
            var builder = new ODataConventionModelBuilder();
            var entitySet = builder.EntitySet<IdentityUserRole<string>>(typeof(IdentityUserRole<string>).Name);
            entitySet.EntityType.HasKey(e => e.UserId);
            entitySet.EntityType.HasKey(e => e.RoleId);
            return builder.GetEdmModel();
        }
    }
}
