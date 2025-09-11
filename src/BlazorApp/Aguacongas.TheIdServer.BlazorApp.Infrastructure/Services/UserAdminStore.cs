// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Aguacongas.TheIdServer.BlazorApp.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Services
{
    public class UserAdminStore : IAdminStore<User>
    {
        private readonly IAdminStore<Entity.User> _store;

        public UserAdminStore(IAdminStore<Entity.User> store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public async Task<User> CreateAsync(User entity, CancellationToken cancellationToken = default)
        {
            return User.FromEntity(await _store.CreateAsync(entity,cancellationToken)
                .ConfigureAwait(false));
        }

        public async Task<object> CreateAsync(object entity, CancellationToken cancellationToken = default)
        {
            return await CreateAsync(entity as User, cancellationToken)
                .ConfigureAwait(false);
        }

        public Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            return _store.DeleteAsync(id, cancellationToken);
        }

        public async Task<User> GetAsync(string id, GetRequest request, CancellationToken cancellationToken = default)
        {
            return User.FromEntity(await _store.GetAsync(id, request, cancellationToken)
                .ConfigureAwait(false));
        }

        public Task<PageResponse<User>> GetAsync(PageRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<User> UpdateAsync(User entity, CancellationToken cancellationToken = default)
        {
            return User.FromEntity(await _store.UpdateAsync(entity, cancellationToken)
                .ConfigureAwait(false));
        }

        public async Task<object> UpdateAsync(object entity, CancellationToken cancellationToken = default)
        {
            return await UpdateAsync(entity as User, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
