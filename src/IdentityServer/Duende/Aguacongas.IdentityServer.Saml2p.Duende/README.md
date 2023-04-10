# Aguacongas.IdentityServer.Saml2p.Duende

Add a SAML2P controller to your Duende Identity server.  
This lib uses [`ITfoxtec.Identity.Saml2.MvcCore`](https://www.itfoxtec.com/IdentitySaml2) internally to implements the protocol.

## Setup

```cs
services.AddIdentityServer()
    .AddKeysRotation(options => configuration.GetSection(nameof(KeyRotationOptions))?.Bind(options));

services.AddControllersWithViews()
    .AddIdentityServerSaml2P();
```

> Saml2P depends on a `ISigningCredentialStore`. You can register it using `AddSigningCredential` with a `X509Certificate2` in place of `AddKeysRotation` if you prefer.

## Usage

**`saml2p/metadata`** returns the Saml2P metadata document.

You can add a client to you configuration with **saml2p** as protocol type:

```cs
new Client
{
    ClientId = "itfoxtec-testwebappcore",
    ProtocolType = ProtocolTypes.Saml2p,

    RedirectUris = { "http://localhost:10314/Auth/AssertionConsumerService" },
    PostLogoutRedirectUris = "http://localhost:10314/Auth/SingleLogout",

    ClientSecrets = [
      new Secret {
        Type = SecretTypes.X509CertificateBase64,
        Value = Convert.ToBase64String(certificate.Export(X509ContentType.Cert))
      }
    ]
}
```

> The client must have only one redirect uri pointing to the *Assertion Consumer Service* route and one X509Certificate secret mathing its signing certificate.  
 
And configure the client to use Saml2P authentication using `ITfoxtec.Identity.Saml2`:

```csharp
services.BindConfig<Saml2Configuration>(Configuration, "Saml2", (serviceProvider, saml2Configuration) =>
{
    // The certificate must be the one configured as secret for the client;
    saml2Configuration.SigningCertificate = CertificateUtil.Load(AppEnvironment.MapToPhysicalFilePath(Configuration["Saml2:SigningCertificateFile"]), Configuration["Saml2:SigningCertificatePassword"], X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
    saml2Configuration.AllowedAudienceUris.Add(saml2Configuration.Issuer);

    var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
    var entityDescriptor = new EntityDescriptor();
    entityDescriptor.ReadIdPSsoDescriptorFromUrlAsync(httpClientFactory, new Uri(Configuration["Saml2:IdPMetadata"])).GetAwaiter().GetResult();
    if (entityDescriptor.IdPSsoDescriptor != null)
    {
        saml2Configuration.AllowedIssuer = entityDescriptor.EntityId;
        saml2Configuration.SingleSignOnDestination = entityDescriptor.IdPSsoDescriptor.SingleSignOnServices.First().Location;
        saml2Configuration.SingleLogoutDestination = entityDescriptor.IdPSsoDescriptor.SingleLogoutServices.First().Location;
        foreach (var signingCertificate in entityDescriptor.IdPSsoDescriptor.SigningCertificates)
        {
            if (signingCertificate.IsValidLocalTime())
            {
                saml2Configuration.SignatureValidationCertificates.Add(signingCertificate);
            }
        }
        if (saml2Configuration.SignatureValidationCertificates.Count <= 0)
        {
            throw new Exception("The IdP signing certificates has expired.");
        }
        if (entityDescriptor.IdPSsoDescriptor.WantAuthnRequestsSigned.HasValue)
        {
            saml2Configuration.SignAuthnRequest = entityDescriptor.IdPSsoDescriptor.WantAuthnRequestsSigned.Value;
        }
    }
    else
    {
        throw new Exception("IdPSsoDescriptor not loaded from metadata.");
    }

    return saml2Configuration;
});            

services.AddSaml2(slidingExpiration: true);
services.AddHttpClient();
```

**appsettings.json**

```json
{
  "Saml2": {
    "IdPMetadata": "https://localhost:5443/saml2p/metadata", // Your Duende Identity server metadata uri
    "Issuer": "itfoxtec-testwebappcore", // Client Id
    "SignatureAlgorithm": "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256",
    "SigningCertificateFile": "itfoxtec.identity.saml2.testwebappcore_Certificate.pfx",
    "SigningCertificatePassword": "!QAZ2wsx",
    "CertificateValidationMode": "None", // "ChainTrust"
    "RevocationMode": "NoCheck"
  }
}
```

> Review samples in [ITfoxtec/ITfoxtec.Identity.Saml2 repo](https://raw.githubusercontent.com/ITfoxtec/ITfoxtec.Identity.Saml2)  

## Metadata configuration

`AddIdentityServerSaml2P` extension accept a `IConfiguration` or a `Saml2POptions` parameter to configure the metadata document génération with claims lists.

``` csharp
mvcBuilder.AddIdentityServerSaml2P(configurationManager.GetSection(nameof(Saml2POptions)));
```

```json
"Saml2POptions": {
    "X509CertificateValidationMode": "None",
    "ContactPersons": [
      {
        "ContactKind": "Technical",
        "Company": "Aguafrommars",
        "GivenName": "Olivier Lefebvre",
        "SurName": "Aguacongas",
        "EmailAddress": "aguacongas@gmail.com"
      }
    ],
    "RevocationMode": "NoCheck",
    "SignatureAlgorithm": "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256",
    "ValidUntil": 365
  }
```

## Implement your store

To access data the [`ISaml2PService`](Services/ISaml2PService.cs) use a [`IRelyingPartyStore`](Services/IRelyingPartyStore.cs). You can implement this interface and provide your implementation to the DI to ovveride the default [`IRelyingPartyStore`](Services/RelyingPartyStore.cs) implementation.

```cs
/// <summary>
/// Custom IRelyingPartyStore implementation
/// </summary>
/// <seealso cref="IRelyingPartyStore" />
public class MyRelyingPartyStore : IRelyingPartyStore
{
    public async Task<RelyingParty> FindRelyingPartyByRealm(string realm)
    {
        // TODO: Implement me
        throw new NotImplementedException();
    }
}
```

The DI configuration become:

```cs
services.AddIdentityServer()
    .AddKeysRotation(options => configuration.GetSection(nameof(KeyRotationOptions))?.Bind(options));

services.AddControllersWithViews()
    .AddIdentityServerSaml2P();

services.AddTransient<IRelyingPartyStore, MyRelyingPartyStore>();
```
