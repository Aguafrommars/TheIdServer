# Keys rotation

The *IdentityServer:Key* section configure the [signing key](https://identityserver4.readthedocs.io/en/latest/topics/crypto.html?highlight=siging%20key#token-signing-and-validation/) using [Aguacongas.IdentityServer.KeysRotation](../src/IdentityServer/Aguacongas.IdentityServer.KeysRotation#aguacongasidentityserverkeysrotation).  
To use the keys rotation mechanism, the *Type* must be **KeysRotation**.

**minimum configuration sample**
```js
"IdentityServer": {
  "Key": {
    "Type": "KeysRotation",
    "StorageKind": "EntityFramework"
  }
}
```

**full configuration sample**
```js
"IdentityServer": {
  "Key": {
    "Type": "KeysRotation",
    "StorageKind": "Redis",
    "StorageConnectionString": "localhost:6379",
    "KeyProtectionOptions": {
      "KeyProtectionKind": "X509",
      "X509CertificatePath": "C:\\certificates\\theidserver.pfx",
      "X509CertificatePassword": "P@ssw0rd"
    },
    "KeyRotationOptions": {
      "AutoGenerateKeys": true,
      "NewKeyLifetime": "90.00:00:00",
      "KeyPropagationWindow": "14.00:00:00",
      "MaxServerClockSkew": "00:05:00",
      "KeyRingRefreshPeriod": "24:00:00"
    },
    "RsaEncryptorConfiguration": {
      "EncryptionAlgorithmKeySize": 2048,
      "SigningAlgorithm": "RS256"
      "KeyIdSize": 128,
      "KeyRetirement": "180.00:00:00"
    },
    "AdditionalSigningKeyType": {
      "RS384": {
        "EncryptionAlgorithmKeySize": 2048,
        "SigningAlgorithm": "RS384"
        "KeyIdSize": 128,
        "KeyRetirement": "180.00:00:00"
      },
      "RS512": {
        "EncryptionAlgorithmKeySize": 2048,
        "SigningAlgorithm": "RS512"
        "KeyIdSize": 128,
        "KeyRetirement": "180.00:00:00"
      },
      "PS256": {
        "EncryptionAlgorithmKeySize": 2048,
        "SigningAlgorithm": "PS256"
        "KeyIdSize": 128,
        "KeyRetirement": "180.00:00:00"
      },
      "PS384": {
        "EncryptionAlgorithmKeySize": 2048,
        "SigningAlgorithm": "PS384"
        "KeyIdSize": 128,
        "KeyRetirement": "180.00:00:00"
      },
      "PS512": {
        "EncryptionAlgorithmKeySize": 2048,
        "SigningAlgorithm": "PS512"
        "KeyIdSize": 128,
        "KeyRetirement": "180.00:00:00"
      },
      "ES256": {
        "EncryptionAlgorithmKeySize": 521,
        "SigningAlgorithm": "ES256"
        "KeyIdSize": 128,
        "KeyRetirement": "180.00:00:00"
      },
      "ES384": {
        "EncryptionAlgorithmKeySize": 521,
        "SigningAlgorithm": "ES384"
        "KeyIdSize": 128,
        "KeyRetirement": "180.00:00:00"
      },
      "ES512": {
        "EncryptionAlgorithmKeySize": 521,
        "SigningAlgorithm": "ES512"
        "KeyIdSize": 128,
        "KeyRetirement": "180.00:00:00"
      }
    }
  }
}
```

## Storages

* **StorageKind** defines the storage kind to use.
* **StorageConnectionString** defines how to access the storage.

The configuration support all [Key storage providers](https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/implementation/key-storage-providers?view=aspnetcore-3.1&tabs=visual-studio) except **Registry** because it's a Windows only store.

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
  "RedisKey": "KeysRotation-Keys"
```

For *Redis* storage kind, the **StorageConnectionString** defines the redis connection string.  
(optional) **RedisKey** defines the redis list key where to store generated keys.

### Entity Framework Core

```js
  "StorageKind": "EntityFramework"
```

For *EntityFramework* storage king, keys are store in the **KeyRotationKeys** table of TheIdServer database

### RavenDb

```js
  "StorageKind": "RavenDb"
```

For *RavenDb* storage king, keys are store in the **KeyRotationKeys** documents of RavenDb database

### MongoDb

```js
  "StorageKind": "MongoDb"
```

For *MongoDb* storage king, keys are store in the **KeyRotationKeys** collection of MongoDb database

## Key protection

*KeyProtectionOptions* controls [Key encryption at rest](https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/implementation/key-encryption-at-rest?view=aspnetcore-3.1) configuration.  

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

## Key rotation options

The section *KeyRotationOptions* congrols key rotation options. It's binded to [`KeyRotationOptions`](../src/IdentityServer/Aguacongas.IdentityServer.KeysRotation#keyrotationoptions-properties) 

```js
    "KeyRotationOptions": {
      "AutoGenerateKeys": true,
      "NewKeyLifetime": "90.00:00:00",
      "KeyPropagationWindow": "14.00:00:00",
      "MaxServerClockSkew": "00:05:00",
      "KeyRingRefreshPeriod": "24:00:00"
    }
```

## Default RSA key generation options

The section *RsaEncryptorConfiguration* congrols the default RSA key generation options. It's binded to [`RsaEncryptorConfiguration`](../src/IdentityServer/Duende/Aguacongas.IdentityServer.KeysRotation.Duende#rsaencryptorconfiguration-properties) 

```js
    "RsaEncryptorConfiguration": {
      "EncryptionAlgorithmKeySize": 2048,
      "SigningAlgorithm": "RS256",
      "KeyIdSize": 128,
      "KeyRetirement": "180.00:00:00"
    }
```
## Additional key type genration options

The section *AdditionalSigningKeyType* controls additional key type generation options. It's a dictionary of ['SigningAlgorithmConfiguration`](../src/IdentityServer/Aguacongas.IdentityServer.KeysRotation#signingalgorithmconfiguration-properties) indexed by signing algorithm.  

> When the key start with a **E** the encryption algorithm is ECDsa else the encryption algorithm is Rsa.

For exemple if you want to support **ES512** and **PS384** in addition of the default **RS256** algorithm your configuration can look like that:

```js
"IdentityServer": {
  "Key": {
    "Type": "KeysRotation",
    "StorageKind": "EntityFramework",
    "RsaEncryptorConfiguration": {
      "SigningAlgorithm": "RS256",
    },
    "AdditionalSigningKeyType": {
      "PS384": {
        "SigningAlgorithm": "PS384"
      },
      "ES512": {
        "SigningAlgorithm": "ES512"
      }
    }
  }
}
```

## Additional resources

* [IdentityServer and Signing Key Rotation](https://brockallen.com/2019/08/09/identityserver-and-signing-key-rotation/)
* [Aguacongas.IdentityServer.KeysRotation](../src/IdentityServer/Aguacongas.IdentityServer.KeysRotation#aguacongasidentityserverkeysrotation)