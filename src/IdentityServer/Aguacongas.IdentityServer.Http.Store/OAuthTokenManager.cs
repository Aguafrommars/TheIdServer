using IdentityModel.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Http.Store
{
    public class OAuthTokenManager : IDisposable
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _httpClient;
        private readonly IOptions<AuthorizationOptions> _options;
        private readonly Mutex _mutex = new Mutex();
        private DiscoveryDocumentResponse _discoveryResponse;
        private DateTime _expiration;
        private string _accessToken;

        public OAuthTokenManager(IHttpContextAccessor httpContextAccessor,
            HttpClient httpClient,
            IOptions<AuthorizationOptions> options)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<string> GetTokenAsync()
        {
            var userToken = _httpContextAccessor.HttpContext?
                .Request?
                .Headers?
                .FirstOrDefault(h => h.Key == "Authorization").Value.FirstOrDefault();
            if (userToken != null)
            {
                return userToken;
            }

            if (_expiration > DateTime.Now)
            {
                return _accessToken;
            }

            try
            {
                _mutex.WaitOne();
                await SetAccessTokenAsync().ConfigureAwait(false);
            }
            finally
            {
                _mutex.ReleaseMutex();
            }

            return _accessToken;                    
        }

        private async Task SetAccessTokenAsync()
        {
            if (_expiration > DateTime.Now)
            {
                return;
            }

            var discoveryResponse = await GetDiscoveryResponseAsync().ConfigureAwait(false);

            var options = _options.Value;
            using var request = new ClientCredentialsTokenRequest
            {
                Address = discoveryResponse.TokenEndpoint,
                ClientId = options.ClientId,
                ClientSecret = options.ClientSecret,
                Scope = options.Scope
            };

            var tokenResponse = await _httpClient.RequestClientCredentialsTokenAsync(request)
                .ConfigureAwait(false);
            Console.WriteLine($"Access token: {tokenResponse.AccessToken}\n");
            if (tokenResponse.Error != null)
            {
                throw new InvalidOperationException($"{tokenResponse.Error} {tokenResponse.ErrorDescription}");
            }

            _expiration = DateTime.Now.AddSeconds(tokenResponse.ExpiresIn - options.RefreshBefore);
            _accessToken = tokenResponse.AccessToken;
        }


        private async Task<DiscoveryDocumentResponse> GetDiscoveryResponseAsync()
        {
            if (_discoveryResponse != null)
            {
                return _discoveryResponse;
            }
            var discoveryResponse = await _httpClient.GetDiscoveryDocumentAsync(_options.Value.Authority)
                            .ConfigureAwait(false);

            Console.WriteLine($"Token enpoint: {discoveryResponse.TokenEndpoint}\n");
            if (discoveryResponse.Error != null)
            {
                throw new InvalidOperationException(discoveryResponse.Error);
            }

            _discoveryResponse = discoveryResponse;
            return _discoveryResponse;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _mutex.Dispose();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
