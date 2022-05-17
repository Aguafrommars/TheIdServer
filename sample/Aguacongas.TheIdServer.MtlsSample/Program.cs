using Aguacongas.TheIdServer.MtlsSample;
using IdentityModel.Client;
using System.Security.Cryptography.X509Certificates;

Console.Title = "Console MTLS Client";

var tokenResponse = await RequestTokenAsync();

tokenResponse.Show();

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
        throw new Exception(response.Error);
    }

    return response;
}
