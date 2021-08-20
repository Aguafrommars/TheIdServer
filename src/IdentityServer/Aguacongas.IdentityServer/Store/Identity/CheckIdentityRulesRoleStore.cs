// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Store
{
    public class CheckIdentityRulesRoleStore<TStore> : IAdminStore<Role> where TStore : IAdminStore<Role>
    {
        private readonly TStore _parent;
        private readonly RoleManager<IdentityRole> _manager;

        public CheckIdentityRulesRoleStore(TStore parent, RoleManager<IdentityRole> manager)
        {
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
        }

        public async Task<Role> CreateAsync(Role entity, CancellationToken cancellationToken = default)
        {
            var role = CreateRole(entity);
            var result = await _manager.CreateAsync(role).ConfigureAwait(false);
            entity = CheckResult(entity, result);
            entity.Id = role.Id;
            return entity;
        }


        public async Task<object> CreateAsync(object entity, CancellationToken cancellationToken = default)
        => await CreateAsync(entity as Role, cancellationToken).ConfigureAwait(false);

        public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            var role = await _manager.FindByIdAsync(id).ConfigureAwait(false);
            if (role == null)
            {
                return;
            }
            CheckResult(null, await _manager.DeleteAsync(role).ConfigureAwait(false));
        }

        public Task<Role> GetAsync(string id, GetRequest request, CancellationToken cancellationToken = default)
        => _parent.GetAsync(id, request, cancellationToken);

        public Task<PageResponse<Role>> GetAsync(PageRequest request, CancellationToken cancellationToken = default)
        => _parent.GetAsync(request, cancellationToken);

        public async Task<Role> UpdateAsync(Role entity, CancellationToken cancellationToken = default)
        => CheckResult(entity, await _manager.UpdateAsync(CreateRole(entity)).ConfigureAwait(false));

        public async Task<object> UpdateAsync(object entity, CancellationToken cancellationToken = default)
        => await UpdateAsync(entity as Role, cancellationToken);

        private static Role CheckResult(Role entity, IdentityResult result)
        {
            if (result.Succeeded)
            {
                return entity;
            }

            throw new IdentityException
            {
                Errors = result.Errors
            };
        }

        private static IdentityRole CreateRole(Role entity)
        => new()
        {
            Id = entity.Id,
            ConcurrencyStamp = entity.ConcurrencyStamp,
            Name = entity.Name,
            NormalizedName = entity.Name
        };
    }
}
