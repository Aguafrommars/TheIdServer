# Data protection

To be able to run behind a load balancer the key crypting cookies and anti-CSRF mechanism must be share by all servers or the load balancer must use a session affinity mechanism.  
The *DataProtectionOptions* section configure the [ASP.NET Core Data Protection](https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/introduction?view=aspnetcore-3.1) : 

**minimum configuration sample**
```js
"DataProtectionOptions": {
  "StorageKind": "EntityFramework"
}
```

**full configuration sample**
```js
"DataProtectionOptions": {
  "StorageKind": "FileSytem",
  "StorageConnectionString": "C:\\data-protection-keys",
  "KeyProtectionOptions": {
    "KeyProtectionKind": "X509",
    "X509CertificatePath": "C:\\certifiactes\\data-protection.pfx",
    "X509CertificatePassword": "P@ssw0rd"
  },
  "KeyManagementOptions": {
    "AutoGenerateKeys": true,
    "NewKeyLifetime": "90.00:00:00"
  },
  "AuthenticatedEncryptorConfiguration" : {
    "EncryptionAlgorithm": "AES_256_CBC",
    "ValidationAlgorithm": "HMACSHA256"
  }
}
```

## Storages

* **StorageKind** defines the storage kind to use.
* **StorageConnectionString** defines how to access the storage.

The configuration support all [Key storage providers](https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/implementation/key-storage-providers?view=aspnetcore-3.1&tabs=visual-studio)

### File system

```js
  "StorageKind": "FileSytem",
  "StorageConnectionString": "C:\\data-protection-keys",
```

For *FileSytem* storage kind, the **StorageConnectionString** defines the path where to store keys.

### Azure Storage

```js
  "StorageKind": "AzureStorage",
  "StorageConnectionString": "<blob URI including SAS token>",
```

For *AzureStorage* storage kind, the **StorageConnectionString** defines the blog URI including SAS token where to store keys.

### Redis

```js
  "StorageKind": "Redis",
  "StorageConnectionString": "localhost:6379",
  "RedisKey": "DataProtection-Keys"
```

For *Redis* storage kind, the **StorageConnectionString** defines the redis connection string.  
(optional) **RedisKey** defines the redis list key where to store generated keys.

### Entity Framework Core

```js
  "StorageKind": "EntityFramework"
```

For *EntityFramework* storage king, keys are store in the **DataProtectionKeys** table of TheIdServer database

### RavenDb

```js
  "StorageKind": "RavenDb"
```

For *RavenDb* storage king, keys are store in the **DataProtectionKeys** document of RavenDb database

### Registry
Only applies to Windows deployments.

```js
  "StorageKind": "Registry",
  "StorageConnectionString": "SOFTWARE\\Sample\\keys"
```

For *Registry* storage kind, the **StorageConnectionString** defines the registry path where to store keys. 

> Keys cannot be shared across several instances of a web app using Registry store

## Key protection

*KeyProtectionOptions* controls [Key encryption at rest](https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/implementation/key-encryption-at-rest?view=aspnetcore-3.1) configuration.  
The configuartion support all kind of encryption systems.

* **KeyProtectionKind** defines the kind of key protection to use.

### Azure Key Vault

```js
  "KeyProtectionOptions": {
    "KeyProtectionKind": "AzureKeyVault",
    "AzureKeyVaultKeyId": "<keyIdentifier>",
    "AzureKeyVaultClientId": "<clientId>",
    "AzureKeyVaultClientSecret": "<clientSecret>"
  }
```

### Windows DPAPI

Only applies to Windows deployments.

```js
  "KeyProtectionOptions": {
    "KeyProtectionKind": "WindowsDpApi",
    "WindowsDPAPILocalMachine": false
  }
```

### X.509 certificate

From certificate file :

```js
  "KeyProtectionOptions": {
    "KeyProtectionKind": "X509",  
    "X509CertificatePath": "C:\\certificates\\theidserver.pfx",
    "X509CertificatePassword": "P@ssw0rd"
  }
```

> If the certificate is loaded from a file, it can be selfsigned/seflencrypted and expired. 

From certificate thumbprint :

```js
  "KeyProtectionOptions": {
    "KeyProtectionKind": "X509",
    "X509CertificatePath": "3BCE558E2AD3E0E34A7743EAB5AEA2A9BD2575A0"
  }
```

> Using the thumbprint, the certificate must be valid.

### Windows DPAPI-NG

This mechanism is available only on Windows 8/Windows Server 2012 or later.

```js
  "KeyProtectionOptions": {
    "KeyProtectionKind": "WindowsDpApiNg"
  }
```

Using a SID

```js
  "KeyProtectionOptions": {
    "KeyProtectionKind": "WindowsDpApiNg"
    "WindowsDpApiNgSid": "S-1-5-21-..."
  }
```

Using a certificate thumbprint

```js
  "KeyProtectionOptions": {
    "KeyProtectionKind": "WindowsDpApiNg"
    "WindowsDpApiNgCerticate": "3BCE558E2...B5AEA2A9BD2575A0"
  }
```

## Key management

The section *KeyManagementOptions* congrols the [Key management](https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/implementation/key-management?view=aspnetcore-3.1) configuration

```js
  "KeyManagementOptions": {
    "AutoGenerateKeys": true,
    "NewKeyLifetime": "90.00:00:00"
  }
```

* **AutoGenerateKeys** by default keys are auto generated, you can disable auto generation with `"AutoGenerateKeys": false`.
* **NewKeyLifetime** by default the key lifetime is 90 days. You can set your lifetime with **NewKeyLifetime** but it cannot be less than 1 week.

## Algorithms

You can change alrorithms with the section *AuthenticatedEncryptorConfiguration*. It's binded to a [`AuthenticatedEncryptorConfiguration`](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.dataprotection.authenticatedencryption.configurationmodel.authenticatedencryptorconfiguration?view=aspnetcore-3.1) object.

```js
  "AuthenticatedEncryptorConfiguration" : {
    "EncryptionAlgorithm": "AES_256_CBC",
    "ValidationAlgorithm": "HMACSHA256"
  }
```

## Additionals resources

* [ASP.NET Core Data Protection](https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/introduction?view=aspnetcore-3.1)
* [Configure ASP.NET Core Data Protection](https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/configuration/overview?view=aspnetcore-3.1)
* [Key management in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/implementation/key-management?view=aspnetcore-3.1)
* [Key storage providers in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/implementation/key-storage-providers?view=aspnetcore-3.1&tabs=visual-studio)
* [Key encryption at rest in Windows and Azure using ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/implementation/key-encryption-at-rest?view=aspnetcore-3.1)