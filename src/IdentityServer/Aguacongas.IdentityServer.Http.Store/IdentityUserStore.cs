using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Admin.Http.Store
{
    public class IdentityUserStore : AdminStore<IdentityUser>, IIdentityUserStore<IdentityUser>
    {
        public IdentityUserStore(Task<HttpClient> httpClientFactory, ILogger<IdentityUserStore> logger) : base(httpClientFactory, logger)
        {
        }

        public async Task<EntityClaim> AddClaimAsync(string userId, EntityClaim claim, CancellationToken cancellationToken = default)
        {
            var httpClient = await HttpClientFactory
                .ConfigureAwait(false);
            using (var content = new StringContent(SerializeEntity(claim), Encoding.UTF8, "application/json"))
            {
                using (var response = await httpClient.PostAsync(GetUri(httpClient, $"{BaseUri}/{userId}/claim/add"), content, cancellationToken)
                    .ConfigureAwait(false))
                {
                    await EnsureSuccess(response).ConfigureAwait(false);
                    return await DeserializeResponse<EntityClaim>(response)
                        .ConfigureAwait(false);
                }
            }
        }

        public async Task<string> AddRoleAsync(string userId, string role, CancellationToken cancellationToken = default)
        {
            var httpClient = await HttpClientFactory
                .ConfigureAwait(false);
            using (var content = new StringContent(SerializeEntity(role), Encoding.UTF8, "application/json"))
            {
                using (var response = await httpClient.PostAsync(GetUri(httpClient, $"{BaseUri}/{userId}/role"), content, cancellationToken)
                    .ConfigureAwait(false))
                {
                    await EnsureSuccess(response).ConfigureAwait(false);
                    return await DeserializeResponse<string>(response)
                        .ConfigureAwait(false);
                }
            }
        }

        public async Task<PageResponse<EntityClaim>> GetClaimsAsync(string userId, PageRequest request, CancellationToken cancellationToken = default)
        {
            var httpClient = await HttpClientFactory
                .ConfigureAwait(false);
            using (var response = await httpClient.GetAsync(GetUri(httpClient, $"{BaseUri}/{userId}/claim"), cancellationToken)
                .ConfigureAwait(false))
            {
                await EnsureSuccess(response).ConfigureAwait(false);
                return await DeserializeResponse<PageResponse<EntityClaim>>(response)
                    .ConfigureAwait(false);
            }
        }

        public async Task<PageResponse<Login>> GetLoginsAsync(string userId, PageRequest request, CancellationToken cancellationToken = default)
        {
            var httpClient = await HttpClientFactory
                .ConfigureAwait(false);
            using (var response = await httpClient.GetAsync(GetUri(httpClient, $"{BaseUri}/{userId}/login"), cancellationToken)
                .ConfigureAwait(false))
            {
                await EnsureSuccess(response).ConfigureAwait(false);
                return await DeserializeResponse<PageResponse<Login>>(response)
                    .ConfigureAwait(false);
            }
        }

        public async Task<PageResponse<string>> GetRolesAsync(string userId, PageRequest request, CancellationToken cancellationToken = default)
        {
            var httpClient = await HttpClientFactory
                .ConfigureAwait(false);
            using (var response = await httpClient.GetAsync(GetUri(httpClient, $"{BaseUri}/{userId}/login"), cancellationToken)
                .ConfigureAwait(false))
            {
                await EnsureSuccess(response).ConfigureAwait(false);
                return await DeserializeResponse<PageResponse<string>>(response)
                    .ConfigureAwait(false);
            }
        }

        public async Task RemoveClaimAsync(string userId, EntityClaim claim, CancellationToken cancellationToken = default)
        {
            var httpClient = await HttpClientFactory
                .ConfigureAwait(false);
            using (var content = new StringContent(SerializeEntity(claim), Encoding.UTF8, "application/json"))
            {
                using (var response = await httpClient.PostAsync(GetUri(httpClient, $"{BaseUri}/{userId}/claim/remove"), content, cancellationToken)
                    .ConfigureAwait(false))
                {
                    await EnsureSuccess(response).ConfigureAwait(false);
                }
            }
        }

        public async Task RemoveLoginAsync(string userId, Login login, CancellationToken cancellationToken = default)
        {
            var httpClient = await HttpClientFactory
                .ConfigureAwait(false);
            using (var content = new StringContent(SerializeEntity(login), Encoding.UTF8, "application/json"))
            {
                using (var response = await httpClient.PostAsync(GetUri(httpClient, $"{BaseUri}/{userId}/login/remove"), content, cancellationToken)
                    .ConfigureAwait(false))
                {
                    await EnsureSuccess(response).ConfigureAwait(false);
                }
            }
        }

        public async Task RemoveRoleAsync(string userId, string role, CancellationToken cancellationToken = default)
        {
            var httpClient = await HttpClientFactory
                .ConfigureAwait(false);
            using (var response = await httpClient.DeleteAsync(GetUri(httpClient, $"{BaseUri}/{userId}/role/{role}"), cancellationToken)
                .ConfigureAwait(false))
            {
                await EnsureSuccess(response).ConfigureAwait(false);
            }
        }
    }
}
