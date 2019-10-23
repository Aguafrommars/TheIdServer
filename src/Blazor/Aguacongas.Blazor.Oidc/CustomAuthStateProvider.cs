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
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly NavigationManager _navigationManager;
        private readonly IJSRuntime _JsRuntime;
        private readonly UserStore _userStore;
        private readonly AuthorizationOptions _options;
        private DiscoveryDocumentResponse _discoveryDocumentResponse;
        public CustomAuthStateProvider(HttpClient httpClient, 
            NavigationManager navigationManager, 
            IJSRuntime JsRuntime,
            UserStore userStore,
            AuthorizationOptions options)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _navigationManager = navigationManager ?? throw new ArgumentNullException(nameof(httpClient));
            _JsRuntime = JsRuntime ?? throw new ArgumentNullException(nameof(JsRuntime));
            _userStore = userStore ?? throw new ArgumentNullException(nameof(userStore));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }
        public async Task LoginAsync()
        {
            var discoveryResponse = await GetDicoveryDocumentAsync();

            var nonce = ToUrlBase64String(CryptoRandom.CreateRandomKey(64));
            Console.WriteLine($"Nonce: {nonce}");

            var verifier = ToUrlBase64String(CryptoRandom.CreateRandomKey(64));
            await _JsRuntime.InvokeVoidAsync("sessionStorage.setItem", "verfier", verifier);
            var challenge = GetChallenge(Encoding.ASCII.GetBytes(verifier));
            Console.WriteLine($"Challenge: {challenge}");

            var authorizationUri = QueryHelpers.AddQueryString(discoveryResponse.AuthorizeEndpoint, new Dictionary<string, string>
            {
                ["client_id"] = _options.ClientId,
                ["redirect_uri"] = _options.RedirectUri,
                ["scope"] = _options.Scopes,
                ["response_type"] = "code",
                ["nonce"] = nonce,
                ["code_challenge"] = challenge,
                ["code_challenge_method"] = "S256"
            });

            _navigationManager.NavigateTo(authorizationUri);
            
            using (var response = await _httpClient.GetAsync(authorizationUri))
            {
                var content = await response.Content.ReadAsStringAsync();
            }
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var uri = new Uri(_navigationManager.Uri);
            Console.WriteLine(uri.LocalPath);
            var queryParams = QueryHelpers.ParseQuery(uri.Query);
            if (_userStore.User == null)
            {
                Console.WriteLine("Get user from session storage");
                await GetUserFromSessionStorage();
            }
            if (_userStore.User == null && queryParams.ContainsKey("code"))
            {
                Console.WriteLine("Get user from code");
                var code = queryParams["code"];
                await GetTokensAsync(code);
            }
            if (_userStore.User != null)
            {
                return new AuthenticationState(_userStore.User);
            }

            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        private async Task GetUserFromSessionStorage()
        {
            var expireAtString = await GetItemAsync<string>(_options.ExpireAtStorageKey);
            DateTime expireAt = DateTime.MinValue;
            if (!string.IsNullOrEmpty(expireAtString))
            {
                Console.WriteLine($"Tokens expire at {expireAtString}");
                expireAt = DateTime.Parse(expireAtString);
            }
            if (DateTime.UtcNow < expireAt)
            {
                var tokensString = await GetItemAsync<string>(_options.TokensStorageKey);
                var tokens = JsonSerializer.Deserialize<Tokens>(tokensString);
                var claimsString = await GetItemAsync<string>(_options.ClaimsStorageKey);
                var claims = JsonSerializer.Deserialize<IEnumerable<SerializableClaim>>(claimsString); 
                _userStore.User = CreateUser(claims.Select(c => new Claim(c.Type, c.Value)), tokens.access_token);
            }
        }

        private async Task<DiscoveryDocumentResponse> GetDicoveryDocumentAsync()
        {
            if (_discoveryDocumentResponse != null)
            {
                return _discoveryDocumentResponse;
            }

            var discoveryResponse = await _httpClient.GetDiscoveryDocumentAsync(_options.Authority)
                .ConfigureAwait(false);

            if (discoveryResponse.Error != null)
            {
                throw new InvalidOperationException(discoveryResponse.Error);
            }

            _discoveryDocumentResponse = discoveryResponse;
            return _discoveryDocumentResponse;
        }

        private async Task GetTokensAsync(StringValues code)
        {
            var discoveryResponse = await GetDicoveryDocumentAsync();
            
            using (var authorizationCodeRequest = new AuthorizationCodeTokenRequest
            {
                Address = discoveryResponse.TokenEndpoint,
                ClientId = _options.ClientId,
                RedirectUri = _options.RedirectUri,
                Code = code,
                CodeVerifier = (await GetItemAsync<string>(_options.VerifierStorageKey))
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
                await SetItemAsync(_options.TokensStorageKey, authorizationCodeResponse.Json.ToString());
                await SetItemAsync(_options.ExpireAtStorageKey, 
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

                    await SetItemAsync(_options.ClaimsStorageKey, 
                        JsonSerializer.Serialize(userInfoResponse
                            .Claims
                            .Select(c => new SerializableClaim { Type = c.Type, Value = c.Value })));                   
                    _userStore.User = CreateUser(userInfoResponse.Claims, authorizationCodeResponse.AccessToken);
                }
            }
        }

        private ClaimsPrincipal CreateUser(IEnumerable<Claim> claims, string accesToken)
        {
            var claimList = claims.ToList();
            claimList.Add(new Claim("access_token", accesToken));
            return new ClaimsPrincipal(new ClaimsIdentity[] 
            {
                new ClaimsIdentity(
                    claimList, "Bearer", "name", "role")
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
            return _JsRuntime.InvokeAsync<T>("sessionStorage.setItem", key);
        }

        private ValueTask SetItemAsync<T>(string key, T value)
        {
            return _JsRuntime.InvokeVoidAsync("sessionStorage.setItem", key, value);
        }
    }
}
