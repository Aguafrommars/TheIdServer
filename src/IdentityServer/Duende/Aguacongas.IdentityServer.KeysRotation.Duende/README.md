# Aguacongas.IdentityServer.KeysRotation

This library adapts [ASP.NET Core Data Protection](https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/introduction?view=aspnetcore-3.1) key management system to a signing keys rotation provider for [Duende IdentityServer](https://duendesoftware.com/products/identityserver).

## Setup

```cs
services.AddIdentityServer()
    .AddKeysRotation();
```

This code adds a [`IValidationKeysStore`](https://github.com/DuendeSoftware/IdentityServer/blob/main/src/IdentityServer/Stores/IValidationKeysStore.cs) and [`ISigningCredentialStore`](https://github.com/DuendeSoftware/IdentityServer/blob/main/src/IdentityServer/Stores/ISigningCredentialStore.cs) with the default key rotation configuration.

### Configure key management

```cs
services.AddIdentityServer()
    .AddKeysRotation(options => 
    {
        options.NewKeyLifetime = TimeSpan.FromDays(30);
        options.KeyPropagationWindow = TimeSpan.FromDays(7);
    });
```

`AddKeysRotation` takes an `Action<KeyRotationOptions>` as optional parameter to configure the key management.  

#### `KeyRotationOptions` properties

* **NewKeyLifetime** Controls the lifetime (number of days before expiration) for newly-generated key. The lifetime cannot be less than one week. The default value is 90 days.  
* **KeyPropagationWindow** Specifies the period before key expiration in which a new key should be generated so that it has time to propagate fully throughout the key ring. For example, if this period is 72 hours, then a new key will be created and persisted to storage approximately 72 hours before expiration. The default value is 2 weeks.
* **MaxServerClockSkew** Specifies the maximum clock skew allowed between servers when reading keys from the key ring. The key ring may use a key which has not yet been activated or which has expired if the key's valid lifetime is within the allowed clock skew window. This value can be set to `TimeSpan.Zero` if key activation and expiration times should be strictly honored by this server. The default value is 5 minutes.
* **KeyRingRefreshPeriod** Controls the auto-refresh period where the key ring provider will flush its collection of cached keys and reread the collection from backing storage. The default value is 24 hours.

### Configure Rsa key generation

```cs
services.AddIdentityServer()
    .AddKeysRotation()
    .AddRsaEncryptorConfiguration(options => 
    {
        options.KeyIdSize = 256;
        options.KeyRetirement = TimeSpan.FromDays(120);
    })
```

`AddRsaEncryptorConfiguration` takes an `Action<RsaEncryptorConfiguration>` to configure the Rsa key generation.  

#### RsaEncryptorConfiguration properties

* **EncryptionAlgorithmKeySize** Controls the length of the generated key. The default value is 2048 bytes.
* **RsaSigningAlgorithm** Controls the Rsa Signing algorithm. The default value is `RS256`.
* **KeyIdSize** Controls the length of the key id. The default value is 128 bits.
* **KeyRetirement** Controls the time an expired key is present in the jwks document. The default value is 180 days.

### Persistence

#### File system

To configure a file system-based key repository, call the `PersistKeysToFileSystem` configuration routine as shown below. Provide a `DirectoryInfo` pointing to the repository where keys should be stored:

```cs
services.AddIdentityServer()
    .AddKeysRotation()
    .PersistKeysToFileSystem(new DirectoryInfo(@"c:\temp-keys\"));
```

#### Azure Storage

To configure the Azure Blob Storage provider, call one of the `PersistKeysToAzureBlobStorage` overloads.

```cs
services.AddIdentityServer()
    .AddKeysRotation()
    .PersistKeysToAzureBlobStorage(new Uri("<blob URI including SAS token>"));
```

#### Redis

To configure on Redis, call one of the `PersistKeysToStackExchangeRedis` overloads:

```cs
var redis = ConnectionMultiplexer.Connect("<URI>");

services.AddIdentityServer()
    .AddKeysRotation()
    .PersistKeysToStackExchangeRedis(redis, "KeysRotation-Keys");
```

#### Entity Framework Core

To configure the EF Core provider, call the `PersistKeysToDbContext<TContext>` method:

```cs
services.AddDbContext<MyKeysContext>(options =>
        options.UseSqlServer(
            Configuration.GetConnectionString("MyKeysConnection")));

services.AddIdentityServer()
    .AddKeysRotation()
    .PersistKeysToDbContext<MyKeysContext>();
```

The generic parameter, `TContext`, must inherit from `DbContext` and implement `IKeyRotationContext`:

```cs
using Aguacongas.IdentityServer.KeysRotation.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebApp1
{
    class MyKeysContext : DbContext, IKeyRotationContext
    {
        // A recommended constructor overload when using EF Core 
        // with dependency injection.
        public MyKeysContext(DbContextOptions<MyKeysContext> options) 
            : base(options) { }

        // This maps to the table that stores keys.
        public DbSet<KeyRotationKey> KeyRotationKeys { get; set; }
    }
}
```

### Key encryption at rest

#### Azure Key Vault

To store keys in Azure Key Vault, configure the system with `ProtectKeysWithAzureKeyVault` in the Startup class:

```cs
services.AddIdentityServer()
    .AddKeysRotation()
    .PersistKeysToAzureBlobStorage(new Uri("<blob URI including SAS token>"))
    .ProtectKeysWithAzureKeyVault("<keyIdentifier>", "<clientId>", "<clientSecret>");
```

#### X.509 certificate

If the Duende IdentityServer is spread across multiple machines, it may be convenient to distribute a shared X.509 certificate across the machines and configure the hosted Duende IdentityServer to use the certificate for encryption of keys at rest:

```cs
var certificate = new X509Certificate2("TestCert1.pfx", "password");

services.AddIdentityServer()
    .AddKeysRotation()
    .ProtectKeysWithCertificate(certificate);
```

or 

```cs
services.AddIdentityServer()
    .AddKeysRotation()
    .ProtectKeysWithCertificate("3BCE558E2AD3E0E34A7743EAB5AEA2A9BD2575A0");
```

Due to .NET Framework limitations, only certificates with CAPI private keys are supported.

## Additional resources

* [IdentityServer and Signing Key Rotation](https://brockallen.com/2019/08/09/identityserver-and-signing-key-rotation/)
* [ASP.NET Core Data Protection](https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/introduction?view=aspnetcore-3.1)