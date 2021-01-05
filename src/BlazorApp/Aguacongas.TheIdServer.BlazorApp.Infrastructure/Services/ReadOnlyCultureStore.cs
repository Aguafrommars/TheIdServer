// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Services
{
    public class ReadOnlyCultureStore : IReadOnlyCultureStore
    {
        private readonly IAdminStore<Culture> _adminStore;

        public ReadOnlyCultureStore(IAdminStore<Culture> adminStore)
        {
            _adminStore = adminStore ?? throw new ArgumentNullException(nameof(adminStore));
        }

        public Task<PageResponse<Culture>> GetAsync(PageRequest pageRequest, CancellationToken cancellationToken = default)
        => _adminStore.GetAsync(pageRequest, cancellationToken);
    }
}
