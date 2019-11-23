using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Admin.Http.Store
{
    [SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "No globalization")]
    public class AdminStore<T> : HttpStoreBase<T>, IAdminStore<T> where T : class, IEntityId
    {

        [SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase", Justification = "Paths shoul be to lower")]
        public AdminStore(Task<HttpClient> httpClientFactory, ILogger<AdminStore<T>> logger)
            : base(httpClientFactory, logger)
        {
        }

        public async Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default)
        {
            var httpClient = await HttpClientFactory
                .ConfigureAwait(false);
            using (var content = new StringContent(SerializeEntity(entity), Encoding.UTF8, "application/json"))
            {
                using (var response = await httpClient.PostAsync(GetUri(httpClient, BaseUri), content, cancellationToken)
                    .ConfigureAwait(false))
                {
                    await EnsureSuccess(response).ConfigureAwait(false);
                    return await DeserializeResponse(response)
                        .ConfigureAwait(false);
                }
            }
        }

        public async Task<IEntityId> CreateAsync(IEntityId entity, CancellationToken cancellationToken = default)
        {
            return await CreateAsync(entity as T, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            var httpClient = await HttpClientFactory
                .ConfigureAwait(false);

            using (var response = await httpClient.DeleteAsync(GetUri(httpClient, $"{BaseUri}/{id}"), cancellationToken)
                .ConfigureAwait(false))
            {

                await EnsureSuccess(response)
                    .ConfigureAwait(false);
            }
        }

        public async Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            entity = entity ?? throw new ArgumentNullException(nameof(entity));

            var httpClient = await HttpClientFactory
                .ConfigureAwait(false);

            using (var content = new StringContent(SerializeEntity(entity), Encoding.UTF8, "application/json"))
            {
                using (var response = await httpClient.PutAsync(GetUri(httpClient, $"{BaseUri}/{entity.Id}"), content, cancellationToken)
                    .ConfigureAwait(false))
                {

                    await EnsureSuccess(response).ConfigureAwait(false);
                    return await DeserializeResponse(response)
                        .ConfigureAwait(false);
                }
            }
        }

        public async Task<IEntityId> UpdateAsync(IEntityId entity, CancellationToken cancellationToken = default)
        {
            return await UpdateAsync(entity as T, cancellationToken)
                .ConfigureAwait(false);
        }

        private string SerializeEntity(T entity)
        {
            return JsonSerializer.Serialize(entity, JsonSerializerOptions);
        }

    }
}
