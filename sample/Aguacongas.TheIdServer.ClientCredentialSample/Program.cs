using IdentityModel.Client;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.ClientCredentialSample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var httpClient = new HttpClient();
            
            var discoveryResponse = await httpClient.GetDiscoveryDocumentAsync("https://localhost:5443")
                .ConfigureAwait(false);
            Console.WriteLine($"Token enpoint: {discoveryResponse.TokenEndpoint}\n");
            if (discoveryResponse.Error != null)
            {
                throw new InvalidOperationException(discoveryResponse.Error);
            }
            
            using var request = new ClientCredentialsTokenRequest
            {
                Address = discoveryResponse.TokenEndpoint,
                ClientId = "client",
                ClientSecret = "511536EF-F270-4058-80CA-1C89C192F69A"
            };
            var tokenResponse = await httpClient.RequestClientCredentialsTokenAsync(request)
                .ConfigureAwait(false);
            Console.WriteLine($"Access token: {tokenResponse.AccessToken}\n");
            if (tokenResponse.Error != null)
            {
                throw new InvalidOperationException($"{tokenResponse.Error} {tokenResponse.ErrorDescription}");
            }            
            httpClient.SetBearerToken(tokenResponse.AccessToken);
            
            using var apiResponse = await httpClient.GetAsync("https://localhost:5448/weatherforecast");
            var content = await apiResponse.Content.ReadAsStringAsync();
            Console.WriteLine($"Api response:\n{content}");
            apiResponse.EnsureSuccessStatusCode();
        }
    }
}
