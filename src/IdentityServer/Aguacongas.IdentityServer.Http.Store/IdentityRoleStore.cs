using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Admin.Http.Store
{
    public class IdentityRoleStore : AdminStore<IdentityRole>, IIdentityRoleStore<IdentityRole>
    {
        public IdentityRoleStore(Task<HttpClient> httpClientFactory, ILogger<IdentityRoleStore> logger) : base(httpClientFactory, logger)
        {
        }

        public async Task<EntityClaim> AddClaimAsync(string roleId, EntityClaim claim, CancellationToken cancellationToken = default)
        {
            var httpClient = await HttpClientFactory
                .ConfigureAwait(false);
            using (var content = new StringContent(SerializeEntity(claim), Encoding.UTF8, "application/json"))
            {
                using (var response = await httpClient.PostAsync(GetUri(httpClient, $"{BaseUri}/claim/add"), content, cancellationToken)
                    .ConfigureAwait(false))
                {
                    await EnsureSuccess(response).ConfigureAwait(false);
                    return await DeserializeResponse<EntityClaim>(response)
                        .ConfigureAwait(false);
                }
            }
        }

        public Task<IEntityId> CreateAsync(IEntityId entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<PageResponse<EntityClaim>> GetClaimsAsync(string roleId, PageRequest request, CancellationToken cancellationToken = default)
        {
            var httpClient = await HttpClientFactory
                .ConfigureAwait(false);
            using (var response = await httpClient.GetAsync(GetUri(httpClient, $"{BaseUri}/claim"), cancellationToken)
                .ConfigureAwait(false))
            {
                await EnsureSuccess(response).ConfigureAwait(false);
                return await DeserializeResponse<PageResponse<EntityClaim>>(response)
                    .ConfigureAwait(false);
            }
        }

        public async Task RemoveClaimAsync(string roleId, EntityClaim claim, CancellationToken cancellationToken = default)
        {
            var httpClient = await HttpClientFactory
                .ConfigureAwait(false);
            using (var content = new StringContent(SerializeEntity(claim), Encoding.UTF8, "application/json"))
            {
                using (var response = await httpClient.PostAsync(GetUri(httpClient, $"{BaseUri}/claim/remove"), content, cancellationToken)
                    .ConfigureAwait(false))
                {
                    await EnsureSuccess(response).ConfigureAwait(false);
                }
            }
        }

        public Task<IEntityId> UpdateAsync(IEntityId entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
