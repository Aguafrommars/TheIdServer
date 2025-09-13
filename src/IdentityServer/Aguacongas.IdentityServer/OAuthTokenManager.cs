// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using IdentityModel.Client;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer
{
    public class OAuthTokenManager: OAuthTokenManager<IdentityServerOptions>
    {
        public OAuthTokenManager(HttpClient httpClient, IOptions<IdentityServerOptions> options)
            : base(httpClient, options)
        { }
    }

    public class OAuthTokenManager<T> : IDisposable where T: IdentityServerOptions, new()
    {
        private readonly HttpClient _httpClient;
        private readonly IOptions<T> _options;
        private readonly object _syncObject = new();
        private readonly ManualResetEvent _resetEvent = new(true);
        private DiscoveryDocumentResponse _discoveryResponse;
        private DateTime _expiration;
        private AuthenticationHeaderValue _accessToken;

        public OAuthTokenManager(HttpClient httpClient,
            IOptions<T> options)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<AuthenticationHeaderValue> GetTokenAsync()
        {
            if (_expiration > DateTime.Now)
            {
                return _accessToken;
            }

            lock(_syncObject)
            {
                _resetEvent.WaitOne();
                if (_expiration > DateTime.Now)
                {
                    return _accessToken;
                }

                _resetEvent.Reset();
            }

            try
            {
                await SetAccessTokenAsync().ConfigureAwait(true);
            }
            finally
            {
                _resetEvent.Set();
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

            if (tokenResponse.Error != null)
            {
                throw new InvalidOperationException($"{tokenResponse.Error} {tokenResponse.ErrorDescription}");
            }

            _expiration = DateTime.Now.AddSeconds(tokenResponse.ExpiresIn - options.RefreshBefore);
            _accessToken = new AuthenticationHeaderValue(tokenResponse.TokenType, tokenResponse.AccessToken);
        }


        private async Task<DiscoveryDocumentResponse> GetDiscoveryResponseAsync()
        {
            if (_discoveryResponse != null)
            {
                return _discoveryResponse;
            }
            var discoveryResponse = await _httpClient.GetDiscoveryDocumentAsync(_options.Value.Authority)
                            .ConfigureAwait(false);

            if (discoveryResponse.Error != null)
            {
                if (discoveryResponse.Exception != null)
                {
                    throw discoveryResponse.Exception;
                }
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
                    _resetEvent.Dispose();
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
