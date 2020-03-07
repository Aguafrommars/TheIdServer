using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.Blazor.Oidc
{
    public class OidcAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly NavigationManager _navigationManager;
        private readonly IJSRuntime _jsRuntime;
        private readonly IUserStore _userStore;
        private readonly Task<AuthorizationOptions> _getOptionsTask;
        private readonly ILogger<OidcAuthenticationStateProvider> _logger;

        public OidcAuthenticationStateProvider(HttpClient httpClient,
            NavigationManager navigationManager,
            IJSRuntime JsRuntime,
            IUserStore userStore,
            Task<AuthorizationOptions> getOptionsTask,
            ILogger<OidcAuthenticationStateProvider> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _navigationManager = navigationManager ?? throw new ArgumentNullException(nameof(httpClient));
            _jsRuntime = JsRuntime ?? throw new ArgumentNullException(nameof(JsRuntime));
            _userStore = userStore ?? throw new ArgumentNullException(nameof(userStore));
            _getOptionsTask = getOptionsTask ?? throw new ArgumentNullException(nameof(getOptionsTask));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task LogoffAsync()
        {
            var options = await _getOptionsTask.ConfigureAwait(false);
            var endpoint = await GetItemAsync<string>(options.RevocationEndpointStorageKey);
            var token = await GetItemAsync<string>(options.TokensStorageKey);
            await ClearStorageAsync(options);

            await _httpClient.RevokeTokenAsync(new TokenRevocationRequest
            {
                Address = endpoint,
                ClientId = options.ClientId,
                Token = token
            }).ConfigureAwait(false);

            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

            _navigationManager.NavigateTo(options.LogoutUri);
        }

        public void Login()
        {
            NotifyAuthenticationStateChanged(LoginAsync());
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            AuthorizationOptions options;
            try
            {
                options = await _getOptionsTask.ConfigureAwait(false);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, e.Message);
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("Oidc.NotConnected", "") })));
            }
            catch (TaskCanceledException tce)
            {
                _logger.LogError(tce, tce.Message);
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("Oidc.NotConnected", "") })));
            }

            var uri = new Uri(_navigationManager.Uri);

            if (_userStore.User == null)
            {
                await GetUserFromSessionStorage(options)
                    .ConfigureAwait(false);
            }

            var queryParams = QueryHelpers.ParseQuery(uri.Query);
            if (_userStore.User == null && queryParams.ContainsKey("code"))
            {
                var code = queryParams["code"];
                await GetTokensAsync(code, options)
                    .ConfigureAwait(false);
            }
            if (_userStore.User != null)
            {
                _logger.LogInformation("User found with name {UserName}", _userStore.User.Identity.Name);
                return new AuthenticationState(_userStore.User);
            }
            _logger.LogInformation("No user, returning not authenticate identity");
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim("Oidc.NotConnected", "") })));
        }

        private async Task<AuthenticationState> LoginAsync()
        {
            var options = await _getOptionsTask.ConfigureAwait(false);
            await ClearStorageAsync(options).ConfigureAwait(false);

            var discoveryResponse = await GetDicoveryDocumentAsync(options)
                .ConfigureAwait(false);

            var nonce = ToUrlBase64String(CryptoRandom.CreateRandomKey(64));

            var verifier = ToUrlBase64String(CryptoRandom.CreateRandomKey(64));
            await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", options.VerifierStorageKey, verifier);
            var challenge = GetChallenge(Encoding.ASCII.GetBytes(verifier));

            var authorizationUri = QueryHelpers.AddQueryString(discoveryResponse.AuthorizeEndpoint, new Dictionary<string, string>
            {
                ["client_id"] = options.ClientId,
                ["redirect_uri"] = options.RedirectUri,
                ["scope"] = options.Scope,
                ["response_type"] = "code",
                ["nonce"] = nonce,
                ["code_challenge"] = challenge,
                ["code_challenge_method"] = "S256"
            });

            await SetItemAsync(options.BackUriStorageKey, _navigationManager.Uri);
            _navigationManager.NavigateTo(authorizationUri);

            return await GetAuthenticationStateAsync().ConfigureAwait(false);
        }

        private async Task ClearStorageAsync(AuthorizationOptions options)
        {
            _userStore.User = null;
            _userStore.AccessToken = null;
            _userStore.AuthenticationScheme = null;

            await DeleteItemAsync(options.RevocationEndpointStorageKey);
            await DeleteItemAsync(options.TokenEndpointStorageKey);
            await DeleteItemAsync(options.TokensStorageKey);
            await DeleteItemAsync(options.UserInfoEndpointStorageKey);
            await DeleteItemAsync(options.VerifierStorageKey);
            await DeleteItemAsync(options.BackUriStorageKey);
            await DeleteItemAsync(options.ClaimsStorageKey);
            await DeleteItemAsync(options.ExpireAtStorageKey);
        }

        private async Task GetUserFromSessionStorage(AuthorizationOptions options)
        {
            var expireAtString = await GetItemAsync<string>(options.ExpireAtStorageKey);
            var expireAt = DateTime.MinValue;
            if (!string.IsNullOrEmpty(expireAtString))
            {
                expireAt = DateTime.Parse(expireAtString);
            }
            if (DateTime.UtcNow < expireAt)
            {
                var tokensString = await GetItemAsync<string>(options.TokensStorageKey);
                var tokens = JsonSerializer.Deserialize<Tokens>(tokensString);
                var claimsString = await GetItemAsync<string>(options.ClaimsStorageKey);
                var claims = JsonSerializer.Deserialize<IEnumerable<SerializableClaim>>(claimsString);

                _userStore.User = CreateUser(options,
                    claims.Select(c => new Claim(c.Type, c.Value)),
                    tokens.AccessToken,
                    tokens.TokenType);
            }
        }

        private async Task<DiscoveryDocumentResponse> GetDicoveryDocumentAsync(AuthorizationOptions options)
        {
            var discoveryResponse = await _httpClient.GetDiscoveryDocumentAsync(options.Authority)
                .ConfigureAwait(false);

            if (discoveryResponse.Error != null)
            {
                _logger.LogError(discoveryResponse.Error);
                throw new InvalidOperationException(discoveryResponse.Error);
            }

            await SetItemAsync(options.TokenEndpointStorageKey, discoveryResponse.TokenEndpoint);
            await SetItemAsync(options.RevocationEndpointStorageKey, discoveryResponse.RevocationEndpoint);
            await SetItemAsync(options.UserInfoEndpointStorageKey, discoveryResponse.UserInfoEndpoint);

            return discoveryResponse;
        }

        private async Task GetTokensAsync(StringValues code, AuthorizationOptions options)
        {
            var authorizeEnpoint = await GetItemAsync<string>(options.TokenEndpointStorageKey);
            var codeVerifier = await GetItemAsync<string>(options.VerifierStorageKey);
            codeVerifier = codeVerifier.Replace("=", "")
                    .Replace('+', '-')
                    .Replace('/', '_');

            using var authorizationCodeRequest = new AuthorizationCodeTokenRequest
            {
                Address = authorizeEnpoint,
                ClientId = options.ClientId,
                RedirectUri = options.RedirectUri,
                Code = code,
                CodeVerifier = codeVerifier
            };
            try
            {
                var tokenResponse = await _httpClient.RequestAuthorizationCodeTokenAsync(authorizationCodeRequest)
                    .ConfigureAwait(false);

                if (tokenResponse.IsError)
                {
                    _logger.LogError("Token response error {Error} {Description}", tokenResponse.Error, tokenResponse.ErrorDescription);
                    return;
                }

                await SetItemAsync(options.TokensStorageKey, tokenResponse.Json.ToString());
                await SetItemAsync(options.ExpireAtStorageKey,
                    DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn));

                using var userInfoRequest = new UserInfoRequest
                {
                    Address = await GetItemAsync<string>(options.UserInfoEndpointStorageKey),
                    Token = tokenResponse.AccessToken
                };
                var userInfoResponse = await _httpClient.GetUserInfoAsync(userInfoRequest)
                    .ConfigureAwait(false);


                if (userInfoResponse.IsError)
                {
                    _logger.LogError("{Error}", userInfoResponse.Error);
                    return;
                }

                await SetItemAsync(options.ClaimsStorageKey,
                    JsonSerializer.Serialize(userInfoResponse
                        .Claims
                        .Select(c => new SerializableClaim { Type = c.Type, Value = c.Value })));

                _userStore.User = CreateUser(options,
                    userInfoResponse.Claims,
                    tokenResponse.AccessToken,
                    tokenResponse.TokenType);

            }
            catch (HttpRequestException e)
            {
                _logger.LogError("{Error}", e.Message);
                return;
            }

            var redirectTo = await GetItemAsync<string>(options.BackUriStorageKey);
            _navigationManager.NavigateTo(redirectTo);
        }

        private ClaimsPrincipal CreateUser(AuthorizationOptions options,
            IEnumerable<Claim> claims,
            string accesToken,
            string tokenType)
        {
            _userStore.AccessToken = accesToken;
            _userStore.AuthenticationScheme = tokenType;
            return new ClaimsPrincipal(new ClaimsIdentity[]
            {
                new ClaimsIdentity(
                    claims, tokenType, options.NameClaimType, options.RoleClaimType)
            });
        }

        private string GetChallenge(byte[] verifier)
        {
            using var sha256 = SHA256.Create();
            return ToUrlBase64String(sha256.ComputeHash(verifier));
        }

        private string ToUrlBase64String(byte[] value)
        {
            return Convert.ToBase64String(value)
                    .Replace("=", "")
                    .Replace('+', '-')
                    .Replace('/', '_');
        }

        private ValueTask<T> GetItemAsync<T>(string key)
        {
            return _jsRuntime.InvokeAsync<T>("sessionStorage.getItem", key);
        }

        private ValueTask SetItemAsync<T>(string key, T value)
        {
            return _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", key, value);
        }

        private ValueTask DeleteItemAsync(string key)
        {
            return _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", key);
        }
    }
}
