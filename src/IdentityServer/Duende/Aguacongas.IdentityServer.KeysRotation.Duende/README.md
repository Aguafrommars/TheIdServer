# Aguacongas.IdentityServer.KeysRotation

> This package depends on [Duende.IdentityServer](https://www.nuget.org/packages/Duende.IdentityServer). You'll need to acquire a license for a commercial use.

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
    .AddRsaEncryptorConfiguration(RsaSigningAlgorithm.RS256, options => 
    {
        options.EncryptionAlgorithmKeySize = 2048;
        options.KeyIdSize = 256;
        options.KeyRetirement = TimeSpan.FromDays(120);
    });
```

`AddRsaEncryptorConfiguration` takes a `RsaSigningAlgorithm` and an `Action<RsaEncryptorConfiguration>` to configure the Rsa key generation.  

#### RsaEncryptorConfiguration properties

* **EncryptionAlgorithmKeySize** Controls the length of the generated key. The default value is 2048 bytes.
* **KeyIdSize** Controls the length of the key id. The default value is 128 bits.
* **KeyRetirement** Controls the time an expired key is present in the jwks document. The default value is 180 days.

### Configure ECDsa key generation

```cs
services.AddIdentityServer()
    .AddKeysRotation()
    .AddECDsaEncryptorConfiguration(ECDsaSigningAlgorithm.ES256, options => 
    {
        options.EncryptionAlgorithmKeySize = 256;
        options.KeyIdSize = 128;
        options.KeyRetirement = TimeSpan.FromDays(120);
    });
```

## Persistence

### File system

To configure a file system-based key repository, call the `PersistKeysToFileSystem` configuration routine as shown below. Provide a `DirectoryInfo` pointing to the repository where keys should be stored:

```cs
services.AddIdentityServer()
    .AddKeysRotation()
    .PersistKeysToFileSystem(new DirectoryInfo(@"c:\temp-keys\"));
```

### Azure Storage

To configure the Azure Blob Storage provider, call one of the `PersistKeysToAzureBlobStorage` overloads.

```cs
services.AddIdentityServer()
    .AddKeysRotation()
    .PersistKeysToAzureBlobStorage(new Uri("<blob URI including SAS token>"));
```

Or with connection string:

```cs
services.AddIdentityServer()
    .AddKeysRotation()
    .PersistKeysToAzureBlobStorage(
        connectionString: "<connection string>",
        containerName: "keys",
        blobName: "keys.xml");
```

### Redis

To configure on Redis, call one of the `PersistKeysToStackExchangeRedis` overloads:

```cs
var redis = ConnectionMultiplexer.Connect("<URI>");

services.AddIdentityServer()
    .AddKeysRotation()
    .PersistKeysToStackExchangeRedis(redis, "KeyRotation-Keys");
```

### Entity Framework Core

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

### RavenDB

To configure the RavenDB provider:

```cs
services.AddIdentityServer()
    .AddKeysRotation()
    .PersistKeysToRavenDb();
```

### MongoDB

To configure the MongoDB provider:

```cs
services.AddIdentityServer()
    .AddKeysRotation()
    .PersistKeysToMongoDb();
```

## Key encryption at rest

### Azure Key Vault

> **⚠️ Migration Notice**: This package now uses the modern `Azure.Security.KeyVault.Keys` SDK. The old `Microsoft.Azure.KeyVault` SDK is obsolete.

#### Option 1: DefaultAzureCredential (Recommended)

The simplest and most flexible approach. Works with Managed Identity in production and Azure CLI in development:

```cs
services.AddIdentityServer()
    .AddKeysRotation()
    .PersistKeysToAzureBlobStorage(new Uri("<blob URI>"))
    .ProtectKeysWithAzureKeyVault(
        keyVaultUri: "https://your-vault.vault.azure.net/",
        keyName: "key-rotation-key");
```

This automatically uses:
- **Managed Identity** when running on Azure (App Service, VM, Container Apps, etc.)
- **Azure CLI** credentials when developing locally (after `az login`)
- **Visual Studio** credentials when debugging
- **Environment variables** for CI/CD pipelines

#### Option 2: Service Principal with Client Secret

For environments where you need explicit authentication:

```cs
services.AddIdentityServer()
    .AddKeysRotation()
    .PersistKeysToAzureBlobStorage(new Uri("<blob URI>"))
    .ProtectKeysWithAzureKeyVault(
        keyVaultUri: "https://your-vault.vault.azure.net/",
        keyName: "key-rotation-key",
        tenantId: "<tenant-id>",
        clientId: "<client-id>",
        clientSecret: "<client-secret>");
```

#### Option 3: Service Principal with Certificate

More secure than client secrets:

```cs
var certificate = X509CertificateLoader.LoadPkcs12FromFile("auth-cert.pfx", "password");

services.AddIdentityServer()
    .AddKeysRotation()
    .PersistKeysToAzureBlobStorage(new Uri("<blob URI>"))
    .ProtectKeysWithAzureKeyVault(
        keyVaultUri: "https://your-vault.vault.azure.net/",
        keyName: "key-rotation-key",
        tenantId: "<tenant-id>",
        clientId: "<client-id>",
        certificate: certificate);
```

#### Option 4: Managed Identity (Explicit)

For Azure resources with Managed Identity:

```cs
services.AddIdentityServer()
    .AddKeysRotation()
    .PersistKeysToAzureBlobStorage(new Uri("<blob URI>"))
    .ProtectKeysWithAzureKeyVaultManagedIdentity(
        keyVaultUri: "https://your-vault.vault.azure.net/",
        keyName: "key-rotation-key");
```

Or with user-assigned Managed Identity:

```cs
services.AddIdentityServer()
    .AddKeysRotation()
    .PersistKeysToAzureBlobStorage(new Uri("<blob URI>"))
    .ProtectKeysWithAzureKeyVaultManagedIdentity(
        keyVaultUri: "https://your-vault.vault.azure.net/",
        keyName: "key-rotation-key",
        managedIdentityClientId: "<managed-identity-client-id>");
```

#### Option 5: Custom TokenCredential

For advanced scenarios:

```cs
using Azure.Identity;

var credential = new ChainedTokenCredential(
    new ManagedIdentityCredential(),
    new AzureCliCredential());

services.AddIdentityServer()
    .AddKeysRotation()
    .PersistKeysToAzureBlobStorage(new Uri("<blob URI>"))
    .ProtectKeysWithAzureKeyVault(
        keyVaultUri: new Uri("https://your-vault.vault.azure.net/"),
        keyName: "key-rotation-key",
        credential: credential);
```

#### Azure Key Vault Permissions

Your identity (Service Principal or Managed Identity) needs the following permissions in Key Vault:

- **Get** (keys)
- **Wrap Key**
- **Unwrap Key**

Configure these in Azure Portal under **Key Vault** > **Access policies**.

#### Configuration by Environment

Recommended approach for multiple environments:

```cs
var keyVaultUri = Configuration["AzureKeyVault:VaultUri"];
var keyName = Configuration["AzureKeyVault:KeyName"];

var builder = services.AddIdentityServer()
    .AddKeysRotation()
    .PersistKeysToAzureBlobStorage(new Uri("<blob URI>"));

if (Environment.IsDevelopment())
{
    // Development: use DefaultAzureCredential (Azure CLI)
    builder.ProtectKeysWithAzureKeyVault(keyVaultUri, keyName);
}
else
{
    // Production: use Managed Identity
    builder.ProtectKeysWithAzureKeyVaultManagedIdentity(keyVaultUri, keyName);
}
```

**appsettings.json:**
```json
{
  "AzureKeyVault": {
    "VaultUri": "https://your-vault.vault.azure.net/",
    "KeyName": "key-rotation-key"
  }
}
```

### X.509 certificate

If Duende IdentityServer is spread across multiple machines, it may be convenient to distribute a shared X.509 certificate across the machines and configure the hosted Duende IdentityServer to use the certificate for encryption of keys at rest:

```cs
var certificate = X509CertificateLoader.LoadPkcs12FromFile("TestCert1.pfx", "password");

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

## Migration from old Azure Key Vault SDK

If you're upgrading from a version using `Microsoft.Azure.KeyVault`, your existing code will continue to work but will show obsolete warnings. Here's how to migrate:

### Before (obsolete)
```cs
.ProtectKeysWithAzureKeyVault(
    "https://vault.vault.azure.net/keys/key-name",
    "client-id",
    "client-secret")
```

### After (recommended)
```cs
.ProtectKeysWithAzureKeyVault(
    keyVaultUri: "https://vault.vault.azure.net/",
    keyName: "key-name",
    tenantId: "tenant-id",  // Now required
    clientId: "client-id",
    clientSecret: "client-secret")
```

Or better yet, use `DefaultAzureCredential`:

```cs
.ProtectKeysWithAzureKeyVault(
    "https://vault.vault.azure.net/",
    "key-name")
```

**Important changes:**
- `keyIdentifier` is now split into `keyVaultUri` and `keyName`
- `tenantId` is now required (was optional with "common" tenant)
- New authentication options available (DefaultAzureCredential, Managed Identity)
- Data encrypted with the old SDK is 100% compatible with the new SDK

## Complete Example

```cs
public void ConfigureServices(IServiceCollection services)
{
    services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

    services.AddIdentityServer()
        .AddAspNetIdentity<ApplicationUser>()
        .AddConfigurationStore(options =>
        {
            options.ConfigureDbContext = b => b.UseSqlServer(
                Configuration.GetConnectionString("ConfigurationConnection"));
        })
        .AddOperationalStore(options =>
        {
            options.ConfigureDbContext = b => b.UseSqlServer(
                Configuration.GetConnectionString("OperationalConnection"));
        })
        .AddKeysRotation(options =>
        {
            options.NewKeyLifetime = TimeSpan.FromDays(30);
            options.KeyPropagationWindow = TimeSpan.FromDays(7);
        })
        .AddRsaEncryptorConfiguration(RsaSigningAlgorithm.RS256, options =>
        {
            options.EncryptionAlgorithmKeySize = 2048;
            options.KeyRetirement = TimeSpan.FromDays(180);
        })
        .PersistKeysToAzureBlobStorage(
            Configuration["AzureStorage:ConnectionString"],
            "keys",
            "signing-keys.xml")
        .ProtectKeysWithAzureKeyVault(
            Configuration["AzureKeyVault:VaultUri"],
            Configuration["AzureKeyVault:KeyName"]);
}
```

## Additional resources

* [IdentityServer and Signing Key Rotation](https://brockallen.com/2019/08/09/identityserver-and-signing-key-rotation/)
* [ASP.NET Core Data Protection](https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/introduction?view=aspnetcore-3.1)
* [Azure Key Vault Keys SDK](https://docs.microsoft.com/en-us/dotnet/api/overview/azure/security.keyvault.keys-readme)
* [Azure Identity SDK](https://docs.microsoft.com/en-us/dotnet/api/overview/azure/identity-readme)