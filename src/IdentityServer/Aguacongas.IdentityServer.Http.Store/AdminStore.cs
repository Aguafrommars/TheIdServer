using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Admin.Http.Store
{
    public class AdminStore<T> : HttpStoreBase<T>  where T : class
    {
        private readonly PropertyInfo _idProperty;
        public AdminStore(Task<HttpClient> httpClientFactory, ILogger<AdminStore<T>> logger)
            : base(httpClientFactory, logger)
        {
            _idProperty = typeof(T).GetProperty("Id") ?? throw new ArgumentException($"The type parameter {typeof(T)} cannot be used because it doesn't contain an Id property.");
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
                var id = _idProperty.GetValue(entity);
                using (var response = await httpClient.PutAsync(GetUri(httpClient, $"{BaseUri}/{id}"), content, cancellationToken)
                    .ConfigureAwait(false))
                {

                    await EnsureSuccess(response).ConfigureAwait(false);
                    return await DeserializeResponse(response)
                        .ConfigureAwait(false);
                }
            }
        }

        protected string SerializeEntity<TEntity>(TEntity entity)
        {
            return JsonSerializer.Serialize(entity, JsonSerializerOptions);
        }
    }

    public class EntityAdminStore<T> : AdminStore<T>, IAdminStore<T> where T : class, IEntityId
    {
        public EntityAdminStore(Task<HttpClient> httpClientFactory, ILogger<AdminStore<T>> logger)
            : base(httpClientFactory, logger)
        {
        }

        public async Task<IEntityId> CreateAsync(IEntityId entity, CancellationToken cancellationToken = default)
        {
            return await base.CreateAsync(entity as T, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<IEntityId> UpdateAsync(IEntityId entity, CancellationToken cancellationToken = default)
        {
            return await base.UpdateAsync(entity as T, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
