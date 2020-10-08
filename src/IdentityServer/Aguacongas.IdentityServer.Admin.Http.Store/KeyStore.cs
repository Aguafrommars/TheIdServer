using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Admin.Http.Store
{
    /// <summary>
    /// Generic key store
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="HttpStoreBase{T}" />
    /// <seealso cref="IKeyStore{T}" />
    public class KeyStore<T> : HttpStoreBase<T>, IKeyStore<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyStore{T}"/> class.
        /// </summary>
        /// <param name="httpClientFactory">The HTTP client factory.</param>
        /// <param name="logger">The logger.</param>
        public KeyStore(Task<HttpClient> httpClientFactory, ILogger<HttpStoreBase<T>> logger) : base(httpClientFactory, logger)
        {
        }

        public override Task<PageResponse<T>> GetAsync(PageRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override Task<T> GetAsync(string id, GetRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets keys.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<PageResponse<Key>> GetAllKeysAsync(CancellationToken cancellationToken = default)
        {
            var httpClient = await HttpClientFactory
                .ConfigureAwait(false);

            using (var response = await httpClient.GetAsync(GetUri(httpClient, BaseUri), cancellationToken)
                .ConfigureAwait(false))
            {
                await EnsureSuccess(response)
                    .ConfigureAwait(false);

                return await DeserializeResponse<PageResponse<Key>>(response)
                    .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Revokes all keys.
        /// </summary>
        /// <param name="revocationDate">The revocation date.</param>
        /// <param name="reason">The reason.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task RevokeAllKeysAsync(DateTimeOffset revocationDate, string reason, CancellationToken cancellationToken = default)
        {
            var httpClient = await HttpClientFactory
                .ConfigureAwait(false);

            using (var response = await httpClient.DeleteAsync(GetUri(httpClient, $"{BaseUri}?revocationDate{revocationDate}&reason={reason}"), cancellationToken)
                .ConfigureAwait(false))
            {

                await EnsureSuccess(response)
                    .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Revokes a key.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="reason">The reason.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="ArgumentNullException">id</exception>
        public async Task RevokeKeyAsync(string id, string reason, CancellationToken cancellationToken = default)
        {
            id = id ?? throw new ArgumentNullException(nameof(id));
            var httpClient = await HttpClientFactory
                .ConfigureAwait(false);

            using (var response = await httpClient.DeleteAsync(GetUri(httpClient, $"{BaseUri}/{id}?reason={reason}"), cancellationToken)
                .ConfigureAwait(false))
            {

                await EnsureSuccess(response)
                    .ConfigureAwait(false);
            }
        }
    }
}
