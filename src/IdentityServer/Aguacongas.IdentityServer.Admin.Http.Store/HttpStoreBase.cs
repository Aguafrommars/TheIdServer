using Aguacongas.IdentityServer.Store;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Admin.Http.Store
{
    public class HttpStoreBase<T>
    {
        protected JsonSerializerOptions JsonSerializerOptions { get; } = new JsonSerializerOptions
        {
            IgnoreNullValues = true,
            IgnoreReadOnlyProperties = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
        protected ILogger<HttpStoreBase<T>> Logger { get; }
        protected Task<HttpClient> HttpClientFactory { get; }
        protected string BaseUri { get; }

        public HttpStoreBase(Task<HttpClient> httpClientFactory, ILogger<HttpStoreBase<T>> logger)
        {
            HttpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            BaseUri = $"/{typeof(T).Name}".ToLowerInvariant();
        }

        public async Task<T> GetAsync(string id, GetRequest request, CancellationToken cancellationToken = default)
        {
            var httpClient = await HttpClientFactory
                .ConfigureAwait(false);

            var expandParameter = request != null ? $"?expand={request.Expand}" : string.Empty;

            using (var response = await httpClient.GetAsync(GetUri(httpClient, $"{BaseUri}/{id}{expandParameter}"), cancellationToken)
                .ConfigureAwait(false))
            {
                await EnsureSuccess(response)
                    .ConfigureAwait(false);

                return await DeserializeResponse(response)
                    .ConfigureAwait(false);
            }
        }

        public async Task<PageResponse<T>> GetAsync(PageRequest request, CancellationToken cancellationToken = default)
        {
            request = request ?? new PageRequest();

            var dictionary = typeof(PageRequest)
                .GetProperties()
                .Where(p => p.GetValue(request) != null)
                .ToDictionary(p => p.Name.ToLowerInvariant(), p => p.GetValue(request).ToString());

            var httpClient = await HttpClientFactory
                .ConfigureAwait(false);

            using (var response = await httpClient.GetAsync(GetUri(httpClient, QueryHelpers.AddQueryString(BaseUri, dictionary)), cancellationToken)
                .ConfigureAwait(false))
            {

                await EnsureSuccess(response)
                    .ConfigureAwait(false);

                return await DeserializeResponse<PageResponse<T>>(response)
                    .ConfigureAwait(false);
            }
        }

        protected async Task EnsureSuccess(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                if (response.Content == null)
                {
                    response.EnsureSuccessStatusCode();
                }

                var content = await response.Content.ReadAsStringAsync()
                    .ConfigureAwait(false);
                Logger.LogError("Error response received {@Response}", content);

                if (content != null &&
                    (response.StatusCode == HttpStatusCode.BadRequest ||
                     response.StatusCode == HttpStatusCode.Conflict))
                {
                    var details = JsonSerializer.Deserialize<ProblemDetails>(content);
                    throw new ProblemException
                    {
                        Details = details
                    };
                }
                response.EnsureSuccessStatusCode();
            }
        }

        protected Task<T> DeserializeResponse(HttpResponseMessage response)
        {
            return DeserializeResponse<T>(response);
        }


        protected async Task<TResponse> DeserializeResponse<TResponse>(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync()
                .ConfigureAwait(false);
            Logger.LogDebug("Response received {@Response}", content);
            return JsonSerializer.Deserialize<TResponse>(content, JsonSerializerOptions);
        }

        protected Uri GetUri(HttpClient httpClient, string uri)
        {
            var baseAddress = httpClient.BaseAddress.ToString();
            if (baseAddress.EndsWith("/", StringComparison.Ordinal))
            {
                baseAddress = baseAddress.Substring(0, baseAddress.Length - 1);
            }

            var result = new Uri($"{baseAddress}{uri}");
            Logger.LogDebug($"Request {result}");
            return result;
        }
    }
}
