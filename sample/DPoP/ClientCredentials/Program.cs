using ClientCredentialsDPoPClient;
using Duende.AccessTokenManagement;
using Duende.AccessTokenManagement.DPoP;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Security.Cryptography;
using System.Text.Json;

var host = Host.CreateDefaultBuilder(args)
    .UseSerilog((hostingContext, configuration) => configuration.MinimumLevel.Debug()
        .WriteTo.Console(theme: AnsiConsoleTheme.Code))
    .ConfigureServices((services) =>
    {
        services.AddDistributedMemoryCache();

        services.AddClientCredentialsTokenManagement()
            .AddClient("dpop", client =>
            {
                client.TokenEndpoint = new Uri("https://localhost:5443/connect/token");

                client.ClientId = ClientId.Parse("dpop");
                //client.ClientId = "dpop.nonce";
                client.ClientSecret = ClientSecret.Parse("905e4892-7610-44cb-a122-6209b38c882f");

                client.Scope = Scope.Parse("dpopscope");
                client.DPoPJsonWebKey = DPoPProofKey.Parse(CreateDPoPKey());
            });

        services.AddClientCredentialsHttpClient("client", ClientCredentialsClientName.Parse("dpop"), client =>
        {
            client.BaseAddress = new Uri("https://localhost:5005/");
        });

        services.AddHostedService<DPoPClient>();
    });

await host.Build().RunAsync().ConfigureAwait(false);
static string CreateDPoPKey()
{
    var key = new RsaSecurityKey(RSA.Create(2048));
    var jwk = JsonWebKeyConverter.ConvertFromRSASecurityKey(key);
    jwk.Alg = "PS256";
    var jwkJson = JsonSerializer.Serialize(jwk);
    return jwkJson;
}
