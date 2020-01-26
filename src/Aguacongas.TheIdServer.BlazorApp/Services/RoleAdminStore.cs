using Aguacongas.IdentityServer.Store;
using Aguacongas.TheIdServer.BlazorApp.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Services
{
    public class RoleAdminStore : IAdminStore<Role>
    {
        private readonly IAdminStore<Entity.Role> _store;

        public RoleAdminStore(IAdminStore<Entity.Role> store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public async Task<Role> CreateAsync(Role entity, CancellationToken cancellationToken = default)
        {
            return Role.FromEntity(await _store.CreateAsync(entity,cancellationToken)
                .ConfigureAwait(false));
        }

        public async Task<object> CreateAsync(object entity, CancellationToken cancellationToken = default)
        {
            return (await _store.CreateAsync(entity, cancellationToken)
                .ConfigureAwait(false)) as Role;
        }

        public Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            return _store.DeleteAsync(id, cancellationToken);
        }

        public async Task<Role> GetAsync(string id, GetRequest request, CancellationToken cancellationToken = default)
        {
            return Role.FromEntity(await _store.GetAsync(id, request, cancellationToken)
                .ConfigureAwait(false));
        }

        public Task<PageResponse<Role>> GetAsync(PageRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<Role> UpdateAsync(Role entity, CancellationToken cancellationToken = default)
        {
            return Role.FromEntity(await _store.UpdateAsync(entity, cancellationToken)
                .ConfigureAwait(false));
        }

        public async Task<object> UpdateAsync(object entity, CancellationToken cancellationToken = default)
        {
            return Role.FromEntity((await _store.UpdateAsync(entity, cancellationToken)
                .ConfigureAwait(false)) as Role);
        }
    }
}
