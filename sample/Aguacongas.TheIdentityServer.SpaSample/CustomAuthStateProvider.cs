using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Aguacongas.TheIdentityServer.SpaSample
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly NavigationManager _navigationManager;
        private readonly IJSRuntime _JsRuntime;
        private readonly UserStore _userStore;

        public CustomAuthStateProvider(HttpClient httpClient, 
            NavigationManager navigationManager, 
            IJSRuntime JsRuntime,
            UserStore userStore)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _navigationManager = navigationManager ?? throw new ArgumentNullException(nameof(httpClient));
            _JsRuntime = JsRuntime ?? throw new ArgumentNullException(nameof(JsRuntime));
            _userStore = userStore ?? throw new ArgumentNullException(nameof(userStore));
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
                ["client_id"] = "spa",
                ["redirect_uri"] = "http://localhost:5002",
                ["scope"] = "openid profile api1",
                ["response_type"] = "code",
                ["nonce"] = nonce,
                ["code_challenge"] = challenge,
                ["code_challenge_method"] = "S256"
            });

            _navigationManager.NavigateTo(authorizationUri);
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
            var expireAtString = await _JsRuntime.InvokeAsync<string>("sessionStorage.getItem", "expireAt");
            DateTime expireAt = DateTime.MinValue;
            if (!string.IsNullOrEmpty(expireAtString))
            {
                Console.WriteLine($"Tokens expire at {expireAtString}");
                expireAt = DateTime.Parse(expireAtString);
            }
            if (DateTime.UtcNow < expireAt)
            {
                var tokensString = await _JsRuntime.InvokeAsync<string>("sessionStorage.getItem", "tokens");
                var tokens = JsonConvert.DeserializeObject<Tokens>(tokensString);
                var claimsString = await _JsRuntime.InvokeAsync<string>("sessionStorage.getItem", "claims");
                var claims = JsonConvert.DeserializeObject<IEnumerable<SerializableClaim>>(claimsString); 
                _userStore.User = CreateUser(claims.Select(c => new Claim(c.Type, c.Value)), tokens.access_token);
            }
        }

        private async Task<DiscoveryDocumentResponse> GetDicoveryDocumentAsync()
        {
            var discoveryResponse = await _httpClient.GetDiscoveryDocumentAsync("https://localhost:5443")
                .ConfigureAwait(false);

            if (discoveryResponse.Error != null)
            {
                throw new InvalidOperationException(discoveryResponse.Error);
            }

            return discoveryResponse;
        }

        private async Task GetTokensAsync(StringValues code)
        {
            var discoveryResponse = await GetDicoveryDocumentAsync();
            
            using (var authorizationCodeRequest = new AuthorizationCodeTokenRequest
            {
                Address = discoveryResponse.TokenEndpoint,
                ClientId = "spa",
                RedirectUri = "http://localhost:5002",
                Code = code,
                CodeVerifier = (await _JsRuntime.InvokeAsync<string>("sessionStorage.getItem", "verfier"))
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
                await _JsRuntime.InvokeVoidAsync("sessionStorage.setItem", "tokens", authorizationCodeResponse.Json.ToString());
                await _JsRuntime.InvokeVoidAsync("sessionStorage.setItem", "expireAt", DateTime.UtcNow.AddSeconds(authorizationCodeResponse.ExpiresIn));

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

                    await _JsRuntime.InvokeVoidAsync("sessionStorage.setItem",
                        "claims", 
                        System.Text.Json.JsonSerializer.Serialize(userInfoResponse
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
    }
}
