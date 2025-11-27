// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.IdentityServer.Admin.Configuration;
using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.IdentityServer.KeysRotation;
using Aguacongas.IdentityServer.KeysRotation.Extensions;
using Aguacongas.TheIdServer.Models;
using Duende.IdentityServer.Configuration;
using StackExchange.Redis;
using System.Security.Cryptography.X509Certificates;
using static Duende.IdentityServer.IdentityServerConstants;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdentityServerBuilderExtensions
    {
        public static IIdentityServerBuilder ConfigureDiscovey(this IIdentityServerBuilder identityServerBuilder, IConfiguration configuration)
        {
            identityServerBuilder.Services.Configure<IdentityServerOptions>(options =>
            {
                var discovery = options.Discovery;
                var customEntriesOfStringArray = configuration.GetSection("CustomEntriesOfStringArray").Get<Dictionary<string, string[]>>() ?? [];
                foreach (var entry in customEntriesOfStringArray)
                {
                    discovery.CustomEntries.Add(entry.Key, entry.Value);
                }
                var customEntriesOfString = configuration.GetSection("CustomEntriesOfString").Get<Dictionary<string, string>>() ?? [];
                foreach (var entry in customEntriesOfString)
                {
                    discovery.CustomEntries.Add(entry.Key, entry.Value);
                }
                var customEntriesOfBool = configuration.GetSection("CustomEntriesOfBool").Get<Dictionary<string, bool>>() ?? [];
                foreach (var entry in customEntriesOfBool)
                {
                    discovery.CustomEntries.Add(entry.Key, entry.Value);
                }
            });
            return identityServerBuilder;
        }

        public static IIdentityServerBuilder ConfigureKey(this IIdentityServerBuilder identityServerBuilder, IConfiguration configuration)
        {
            if (configuration.GetValue<KeyKinds>("Type") != KeyKinds.KeysRotation)
            {
                identityServerBuilder.AddSigningCredentials();
                return identityServerBuilder;
            }

            var keyRotationSection = configuration.GetSection(nameof(KeyRotationOptions));
            var defaultRsaEncryptionSection = configuration.GetSection(nameof(RsaEncryptorConfiguration));
            var defaultSigingAlgortithm = defaultRsaEncryptionSection?.GetValue<RsaSigningAlgorithm>(nameof(RsaEncryptorConfiguration.SigningAlgorithm)) ?? RsaSigningAlgorithm.RS256;
            var builder = identityServerBuilder.AddKeysRotation(defaultSigingAlgortithm,
                options => keyRotationSection?.Bind(options))
                .AddRsaEncryptorConfiguration(defaultSigingAlgortithm, options => configuration.GetSection(nameof(RsaEncryptorConfiguration))?.Bind(options));

            identityServerBuilder.Services.AddRsaAddValidationKeysStore(defaultSigingAlgortithm);

            var additionalKeyType = new Dictionary<string, SigningAlgorithmConfiguration>();
            var additionalSigningKeyTypeSection = configuration.GetSection("AdditionalSigningKeyType");
            additionalSigningKeyTypeSection.Bind(additionalKeyType);
            foreach (var key in additionalKeyType.Keys)
            {
                if (key.StartsWith('E'))
                {
                    var ecdsa = Enum.Parse<ECDsaSigningAlgorithm>(key);
                    builder.AddECDsaKeysRotation(ecdsa, options => keyRotationSection?.Bind(options))
                        .AddECDsaEncryptorConfiguration(ecdsa, options => additionalSigningKeyTypeSection?.GetSection(key).Bind(options));
                    builder.Services.AddECDsaAddValidationKeysStore(ecdsa);
                    continue;
                }

                var rsa = Enum.Parse<RsaSigningAlgorithm>(key);
                builder.AddRsaKeysRotation(rsa, options => keyRotationSection?.Bind(options))
                    .AddRsaEncryptorConfiguration(rsa, options => additionalSigningKeyTypeSection.GetSection(key).Bind(options));
                builder.Services.AddRsaAddValidationKeysStore(rsa);
            }

            var dataProtectionsOptions = configuration.Get<DataProtectionOptions>();
            switch (dataProtectionsOptions?.StorageKind)
            {
                case StorageKind.AzureStorage:
                    builder.PersistKeysToAzureBlobStorage(new Uri(dataProtectionsOptions.StorageConnectionString));
                    break;
                case StorageKind.EntityFramework:
                    builder.PersistKeysToDbContext<OperationalDbContext>();
                    break;
                case StorageKind.RavenDb:
                    builder.PersistKeysToRavenDb();
                    break;
                case StorageKind.MongoDb:
                    builder.PersistKeysToMongoDb();
                    break;
                case StorageKind.FileSystem:
                    builder.PersistKeysToFileSystem(new DirectoryInfo(dataProtectionsOptions.StorageConnectionString));
                    break;
                case StorageKind.Redis:
                    var redis = ConnectionMultiplexer.Connect(dataProtectionsOptions.StorageConnectionString);
                    if (string.IsNullOrEmpty(dataProtectionsOptions.RedisKey))
                    {
                        builder.PersistKeysToStackExchangeRedis(redis);
                        break;
                    }
                    builder.PersistKeysToStackExchangeRedis(redis, dataProtectionsOptions.RedisKey);
                    break;
            }

            var protectOptions = dataProtectionsOptions?.KeyProtectionOptions;
            if (protectOptions != null)
            {
                switch (protectOptions.KeyProtectionKind)
                {
                    case KeyProtectionKind.AzureKeyVault:
                        ConfigureAzureKeyVaultProtection(builder, protectOptions);
                        break;
                    case KeyProtectionKind.X509:
                        if (!string.IsNullOrEmpty(protectOptions.X509CertificatePath))
                        {
                            var certificate = SigningKeysLoader.LoadFromFile(protectOptions.X509CertificatePath, protectOptions.X509CertificatePassword, X509KeyStorageFlags.Exportable | X509KeyStorageFlags.UserKeySet);
                            builder.ProtectKeysWithCertificate(certificate);
                            break;
                        }
                        builder.ProtectKeysWithCertificate(protectOptions.X509CertificateThumbprint);
                        break;
                }
            }
            return identityServerBuilder;
        }

        private static void ConfigureAzureKeyVaultProtection(IKeyRotationBuilder builder, KeyProtectionOptions protectOptions)
        {
            // Parse the key identifier to extract vault URI and key name
            // Format: https://vault-name.vault.azure.net/keys/key-name or https://vault-name.vault.azure.net/keys/key-name/version
            if (!Uri.TryCreate(protectOptions.AzureKeyVaultKeyId, UriKind.Absolute, out var keyUri))
            {
                throw new InvalidOperationException($"Invalid Azure Key Vault key identifier: {protectOptions.AzureKeyVaultKeyId}");
            }

            var vaultUri = $"{keyUri.Scheme}://{keyUri.Host}";
            var pathParts = keyUri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);

            if (pathParts.Length < 2 || pathParts[0] != "keys")
            {
                throw new InvalidOperationException($"Invalid Azure Key Vault key identifier format: {protectOptions.AzureKeyVaultKeyId}. Expected format: https://vault.vault.azure.net/keys/key-name");
            }

            var keyName = pathParts[1];

            // Determine authentication method based on provided credentials
            if (!string.IsNullOrEmpty(protectOptions.AzureKeyVaultClientId) &&
                !string.IsNullOrEmpty(protectOptions.AzureKeyVaultClientSecret))
            {
                // Service Principal with Client Secret
                // Need tenant ID - if not provided, try to extract from config or use common
                var tenantId = protectOptions.AzureKeyVaultTenantId;

                if (string.IsNullOrEmpty(tenantId))
                {
                    throw new InvalidOperationException(
                        "Azure Key Vault TenantId is required when using ClientId and ClientSecret. " +
                        "Please add 'AzureKeyVaultTenantId' to your KeyProtectionOptions configuration.");
                }

                builder.ProtectKeysWithAzureKeyVault(
                    vaultUri,
                    keyName,
                    tenantId,
                    protectOptions.AzureKeyVaultClientId,
                    protectOptions.AzureKeyVaultClientSecret);
            }
            else if (!string.IsNullOrEmpty(protectOptions.AzureKeyVaultClientId) &&
                     !string.IsNullOrEmpty(protectOptions.AzureKeyVaultCertificateThumbprint))
            {
                // Service Principal with Certificate
                var tenantId = protectOptions.AzureKeyVaultTenantId;

                if (string.IsNullOrEmpty(tenantId))
                {
                    throw new InvalidOperationException(
                        "Azure Key Vault TenantId is required when using ClientId and Certificate. " +
                        "Please add 'AzureKeyVaultTenantId' to your KeyProtectionOptions configuration.");
                }

                var certificate = LoadCertificateByThumbprint(protectOptions.AzureKeyVaultCertificateThumbprint);

                builder.ProtectKeysWithAzureKeyVault(
                    vaultUri,
                    keyName,
                    tenantId,
                    protectOptions.AzureKeyVaultClientId,
                    certificate);
            }
            else if (!string.IsNullOrEmpty(protectOptions.AzureKeyVaultManagedIdentityClientId))
            {
                // User-assigned Managed Identity
                builder.ProtectKeysWithAzureKeyVaultManagedIdentity(
                    vaultUri,
                    keyName,
                    protectOptions.AzureKeyVaultManagedIdentityClientId);
            }
            else
            {
                // DefaultAzureCredential (recommended)
                // Works with:
                // - Managed Identity (system-assigned or user-assigned)
                // - Azure CLI (development)
                // - Visual Studio (development)
                // - Environment variables
                builder.ProtectKeysWithAzureKeyVault(vaultUri, keyName);
            }
        }

        private static X509Certificate2 LoadCertificateByThumbprint(string thumbprint)
        {
            using var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);

            var certificates = store.Certificates.Find(
                X509FindType.FindByThumbprint,
                thumbprint,
                validOnly: false);

            if (certificates.Count == 0)
            {
                // Try LocalMachine if not found in CurrentUser
                using var machineStore = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                machineStore.Open(OpenFlags.ReadOnly);

                certificates = machineStore.Certificates.Find(
                    X509FindType.FindByThumbprint,
                    thumbprint,
                    validOnly: false);
            }

            if (certificates.Count == 0)
            {
                throw new InvalidOperationException($"Certificate with thumbprint '{thumbprint}' not found in CurrentUser or LocalMachine certificate store.");
            }

            return certificates[0];
        }
    }
}