// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Services
{
    public class ReadOnlyLocalizedResourceStore : IReadOnlyLocalizedResourceStore
    {
        private readonly IAdminStore<LocalizedResource> _adminStore;

        public ReadOnlyLocalizedResourceStore(IAdminStore<LocalizedResource> adminStore)
        {
            _adminStore = adminStore ?? throw new ArgumentNullException(nameof(adminStore));
        }

        public Task<PageResponse<LocalizedResource>> GetAsync(PageRequest pageRequest, CancellationToken cancellationToken = default)
        => _adminStore.GetAsync(pageRequest, cancellationToken);
    }
}
