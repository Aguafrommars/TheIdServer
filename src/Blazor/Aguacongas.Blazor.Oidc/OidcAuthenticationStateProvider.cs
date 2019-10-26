using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.WebUtilities;
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

namespace Aguacongas.Blazor.Oidc
{
    public class OidcAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly NavigationManager _navigationManager;
        private readonly IJSRuntime _jsRuntime;
        private readonly IUserStore _userStore;
        private readonly Task<AuthorizationOptions> _getOptionsTask;
        private DiscoveryDocumentResponse _discoveryDocumentResponse;
        public OidcAuthenticationStateProvider(HttpClient httpClient, 
            NavigationManager navigationManager, 
            IJSRuntime JsRuntime,
            IUserStore userStore,
            Task<AuthorizationOptions> getOptionsTask)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _navigationManager = navigationManager ?? throw new ArgumentNullException(nameof(httpClient));
            _jsRuntime = JsRuntime ?? throw new ArgumentNullException(nameof(JsRuntime));
            _userStore = userStore ?? throw new ArgumentNullException(nameof(userStore));
            _getOptionsTask = getOptionsTask ?? throw new ArgumentNullException(nameof(getOptionsTask));
        }
        public async Task LoginAsync()
        {
            var options = await _getOptionsTask.ConfigureAwait(false);
            var discoveryResponse = await GetDicoveryDocumentAsync(options);

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

            _navigationManager.NavigateTo(authorizationUri, true);
            
            using (var response = await _httpClient.GetAsync(authorizationUri))
            {
                var content = await response.Content.ReadAsStringAsync();
            }
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var options = await _getOptionsTask.ConfigureAwait(false);
            var uri = new Uri(_navigationManager.Uri);

            var queryParams = QueryHelpers.ParseQuery(uri.Query);
            if (_userStore.User == null)
            {
                await GetUserFromSessionStorage(options);
            }
            if (_userStore.User == null && queryParams.ContainsKey("code"))
            {
                var code = queryParams["code"];
                await GetTokensAsync(code, options);
            }
            if (_userStore.User != null)
            {
                return new AuthenticationState(_userStore.User);
            }

            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
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
            if (_discoveryDocumentResponse != null)
            {
                return _discoveryDocumentResponse;
            }

            var discoveryResponse = await _httpClient.GetDiscoveryDocumentAsync(options.Authority)
                .ConfigureAwait(false);

            if (discoveryResponse.Error != null)
            {
                throw new InvalidOperationException(discoveryResponse.Error);
            }

            _discoveryDocumentResponse = discoveryResponse;
            return _discoveryDocumentResponse;
        }

        private async Task GetTokensAsync(StringValues code, AuthorizationOptions options)
        {
            var discoveryResponse = await GetDicoveryDocumentAsync(options);
            
            using (var authorizationCodeRequest = new AuthorizationCodeTokenRequest
            {
                Address = discoveryResponse.TokenEndpoint,
                ClientId = options.ClientId,
                RedirectUri = options.RedirectUri,
                Code = code,
                CodeVerifier = (await GetItemAsync<string>(options.VerifierStorageKey))
                    .Replace("=", "")
                    .Replace('+', '-')
                    .Replace('/', '_')
            })
            {
                var authorizationCodeResponse = await _httpClient.RequestAuthorizationCodeTokenAsync(authorizationCodeRequest)
                    .ConfigureAwait(false);
                if (authorizationCodeResponse.IsError)
                {
                    throw new InvalidOperationException($"{authorizationCodeResponse.Error} {authorizationCodeResponse.ErrorDescription}");
                }
                await SetItemAsync(options.TokensStorageKey, authorizationCodeResponse.Json.ToString());
                await SetItemAsync(options.ExpireAtStorageKey, 
                    DateTime.UtcNow.AddSeconds(authorizationCodeResponse.ExpiresIn));

                using (var userInfoRequest = new UserInfoRequest
                {
                    Address = discoveryResponse.UserInfoEndpoint,
                    Token = authorizationCodeResponse.AccessToken
                })
                {
                    var userInfoResponse = await _httpClient.GetUserInfoAsync(userInfoRequest)
                        .ConfigureAwait(false);

                    if (userInfoResponse.IsError)
                    {
                        throw new InvalidOperationException($"{userInfoResponse.Error}");
                    }

                    await SetItemAsync(options.ClaimsStorageKey, 
                        JsonSerializer.Serialize(userInfoResponse
                            .Claims
                            .Select(c => new SerializableClaim { Type = c.Type, Value = c.Value })));
                    
                    _userStore.User = CreateUser(options, 
                        userInfoResponse.Claims, 
                        authorizationCodeResponse.AccessToken, 
                        authorizationCodeResponse.TokenType);
                }
            }
        }

        private ClaimsPrincipal CreateUser(AuthorizationOptions options, 
            IEnumerable<Claim> claims, 
            string accesToken, 
            string tokenType)
        {
            var claimList = claims.ToList();
            _userStore.AccessToken = accesToken;
            _userStore.AuthenticationScheme = tokenType;
            return new ClaimsPrincipal(new ClaimsIdentity[] 
            {
                new ClaimsIdentity(
                    claimList, tokenType, options.NameClaimType, options.RoleClaimType)
            });
        }

        private string GetChallenge(byte[] verifier)
        {
            using(var sha256 = SHA256.Create())
            {
                return ToUrlBase64String(sha256.ComputeHash(verifier));                
            }
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
    }
}
