using Aguacongas.TheIdServer.MtlsSample;
using IdentityModel.Client;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;

Console.Title = "Console MTLS Client";

var tokenResponse = await RequestTokenAsync();

tokenResponse.Show();

using var client = new HttpClient();
client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);

var response = await client.GetAsync("https://localhost:5448/WeatherForecast");

Console.ReadLine();

async Task<TokenResponse> RequestTokenAsync()
{
    var handler = new SocketsHttpHandler();
    var cert = new X509Certificate2("client.pks", "1234");
    handler.SslOptions.ClientCertificates = new X509CertificateCollection { cert };

    using var client = new HttpClient(handler);

    var response = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
    {
        Address = $"{Constants.Authority}connect/mtls/token",
        ClientId = "mtls.client",
        Scope = "api1"
    });

    if (response.IsError)
    {
        throw response.Exception ?? new InvalidOperationException(response.Error);
    }

    return response;
}
