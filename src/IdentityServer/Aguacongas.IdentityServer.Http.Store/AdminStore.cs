using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Admin.Http.Store
{
    [SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "No globalization")]
    public class AdminStore<T> : IAdminStore<T> where T : class, IEntityId
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
        {
            IgnoreNullValues = true,
            IgnoreReadOnlyProperties = true
        };
        private readonly HttpClient _httpClient;
        private readonly ILogger<AdminStore<T>> _logger;
        private readonly string _baseUri;

        public AdminStore(HttpClient httpClient, ILogger<AdminStore<T>> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _baseUri = $"/{typeof(T).Name}";
        }

        public async Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default)
        {
            using var content = new StringContent(SerializeEntity(entity), Encoding.ASCII, "application/json");
            using var response = await _httpClient.PostAsync(_baseUri, content, cancellationToken)
                .ConfigureAwait(false);
                
            await EnsureSuccess(response).ConfigureAwait(false);
            return await DeserializeResponse(response)
                .ConfigureAwait(false);
        }

        public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            using var response = await _httpClient.DeleteAsync($"_baseUri/{id}", cancellationToken)
                .ConfigureAwait(false);

            await EnsureSuccess(response)
                .ConfigureAwait(false);
        }

        public async Task<T> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            using var response = await _httpClient.GetAsync($"_baseUri/{id}", cancellationToken)
                .ConfigureAwait(false);

            await EnsureSuccess(response)
                .ConfigureAwait(false);

            return await DeserializeResponse(response)
                .ConfigureAwait(false);
        }

        public async Task<PageResponse<T>> GetAsync(PageRequest request, CancellationToken cancellationToken = default)
        {
            request ??= new PageRequest();

            var dictionary = typeof(PageRequest)
                .GetProperties()
                .ToDictionary(p => p.Name, p => p.GetValue(request).ToString());

            using var response = await _httpClient.GetAsync(QueryHelpers.AddQueryString(_baseUri, dictionary), cancellationToken)
                .ConfigureAwait(false);

            await EnsureSuccess(response)
                .ConfigureAwait(false);

            return await DeserializeResponse<PageResponse<T>>(response)
                .ConfigureAwait(false);
        }

        public async Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            entity = entity ?? throw new ArgumentNullException(nameof(entity));

            using var content = new StringContent(SerializeEntity(entity), Encoding.ASCII, "application/json");
            using var response = await _httpClient.PutAsync($"{_baseUri}/{entity.Id}", content, cancellationToken)
                .ConfigureAwait(false);

            await EnsureSuccess(response).ConfigureAwait(false);
            return await DeserializeResponse(response)
                .ConfigureAwait(false);
        }

        private string SerializeEntity(T entity)
        {
            return JsonSerializer.Serialize(entity, _jsonSerializerOptions);
        }

        private async Task EnsureSuccess(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error response received {@Response}", await response.Content.ReadAsStringAsync().ConfigureAwait(false));
                response.EnsureSuccessStatusCode();
            }
        }

        private Task<T> DeserializeResponse(HttpResponseMessage response)
        {
            return DeserializeResponse<T>(response);
        }

        private async Task<TResponse> DeserializeResponse<TResponse>(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync()
                .ConfigureAwait(false);
            _logger.LogDebug("Response received {@Response}", content);
            return JsonSerializer.Deserialize<TResponse>(content, _jsonSerializerOptions);
        }
    }
}
