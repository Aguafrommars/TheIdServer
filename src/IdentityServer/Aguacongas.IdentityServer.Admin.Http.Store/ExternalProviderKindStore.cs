// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Admin.Http.Store
{
    public class ExternalProviderKindStore : HttpStoreBase<ExternalProviderKind>, IExternalProviderKindStore
    {
        public ExternalProviderKindStore(Task<HttpClient> httpClientFactory,
            ILogger<ExternalProviderKindStore> logger) : base(httpClientFactory, logger)
        {
        }

        public Task<PageResponse<ExternalProviderKind>> GetAsync(PageRequest request)
            => base.GetAsync(request);
    }
}
