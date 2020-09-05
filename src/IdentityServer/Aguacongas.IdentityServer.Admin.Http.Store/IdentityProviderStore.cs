// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Admin.Http.Store
{
    public class IdentityProviderStore : HttpStoreBase<IdentityProvider>, IIdentityProviderStore
    {
        public IdentityProviderStore(Task<HttpClient> httpClientFactory, 
            ILogger<IdentityProviderStore> logger) : base(httpClientFactory, logger)
        {
        }

        public Task<IdentityProvider> GetAsync(string id)
            => GetAsync(id, null);

        public Task<PageResponse<IdentityProvider>> GetAsync(PageRequest request)
            => base.GetAsync(request);
    }
}
