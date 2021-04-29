# Aguacongas.IdentityServer.WsFederation

Add a WS-Federation controller to your IS4 server.

## Setup

```cs
services.AddIdentityServer()
    .AddKeysRotation(options => configuration.GetSection(nameof(KeyRotationOptions))?.Bind(options));

services.AddControllersWithViews()
    .AddIdentityServerWsFederation();
```

> WS-Fedration depends on a `ISigningCredentialStore`. You can register it using `AddSigningCredential` with a `X509Certificate2` in place of `AddKeysRotation` if you prefer.

## Usage

**`wsfederation/metadata`** returns the WS-Federation metadata document.

You can add a client to you configuration with **wsfed** as protocol type:

```cs
new Client
{
    ClientId = "urn:aspnetcorerp",
    ProtocolType = ProtocolTypes.WsFederation,

    RedirectUris = { "http://localhost:10314/" },
    FrontChannelLogoutUri = "http://localhost:10314/account/signoutcleanup",
    IdentityTokenLifetime = 36000,

    AllowedScopes = { "openid", "profile" }
}
```

And configure the client to use WS-Federation authentication:

```cs
services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = WsFederationDefaults.AuthenticationScheme;
})
    .AddCookie(options =>
    {
        options.Cookie.Name = "aspnetcorewsfed";
    })
    .AddWsFederation(options =>
    {
        options.MetadataAddress = "https://localhost:5443/wsfederation/metadata";
        options.RequireHttpsMetadata = false;

        options.Wtrealm = "urn:aspnetcorerp";

        options.SignOutWreply = "https://localhost:10315";
        options.SkipUnrecognizedRequests = true;
    });
```