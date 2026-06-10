using Duende.IdentityModel;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Aguacongas.TheIdServer.BlazorApp.BFF.Services;

public class AssertionService(IConfiguration configuration)
{
    private static readonly string SECRET_KEY = "Secrets:Key";
    private static readonly string PS256_ALGORITHM = "PS256";

    public string CreateClientToken()
    {
        var now = DateTimeOffset.UtcNow;
        var options = new OpenIdConnectOptions();
        configuration.Bind("ProviderOptions", options);
        var clientId = options.ClientId!;

        // in production, you should load that key from some secure location
        var key = configuration.GetValue<string>(SECRET_KEY);

        var token = new JwtSecurityToken(
            options.ClientId,
            options.Authority,
            [
                new Claim(JwtClaimTypes.JwtId, Guid.NewGuid().ToString()),
                new Claim(JwtClaimTypes.Subject, clientId),
                new Claim(JwtClaimTypes.IssuedAt, now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            ],
            now.UtcDateTime,
            now.UtcDateTime.AddMinutes(1),
            new SigningCredentials(new JsonWebKey(key), PS256_ALGORITHM)
        );

        token.Header[JwtClaimTypes.TokenType] = "client-authentication+jwt";

        var tokenHandler = new JwtSecurityTokenHandler();
        tokenHandler.OutboundClaimTypeMap.Clear();

        return tokenHandler.WriteToken(token);
    }

    public string SignAuthorizationRequest(OpenIdConnectMessage message)
    {
        var now = DateTime.UtcNow;
        var options = new OpenIdConnectOptions();
        configuration.Bind("ProviderOptions", options);
        var clientId = options.ClientId!;

        // in production you should load that key from some secure location
        var key = configuration.GetValue<string>(SECRET_KEY);

        var claims = new List<Claim>();
        foreach (var parameter in message.Parameters)
        {
            claims.Add(new Claim(parameter.Key, parameter.Value));
        }

        var token = new JwtSecurityToken(
            clientId,
            options.Authority,
            claims,
            now,
            now.AddMinutes(1),
            new SigningCredentials(new JsonWebKey(key), PS256_ALGORITHM)
        );

        var tokenHandler = new JwtSecurityTokenHandler();
        tokenHandler.OutboundClaimTypeMap.Clear();

        return tokenHandler.WriteToken(token);
    }
}