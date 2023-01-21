// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Aguacongas.IdentityServer.Admin.Configuration;
using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.IdentityServer.KeysRotation;
using Aguacongas.IdentityServer.KeysRotation.Extensions;
using Aguacongas.TheIdServer.Models;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Duende.IdentityServer.Configuration;
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
                var customEntriesOfStringArray = configuration.GetSection("CustomEntriesOfStringArray").Get<Dictionary<string, string[]>>() ?? new Dictionary<string, string[]>(0);
                foreach(var entry in customEntriesOfStringArray)
                {
                    discovery.CustomEntries.Add(entry.Key, entry.Value);
                }
                var customEntriesOfString = configuration.GetSection("CustomEntriesOfString").Get<Dictionary<string, string>>() ?? new Dictionary<string, string>(0);
                foreach (var entry in customEntriesOfString)
                {
                    discovery.CustomEntries.Add(entry.Key, entry.Value);
                }
                var customEntriesOfBool = configuration.GetSection("CustomEntriesOfBool").Get<Dictionary<string, bool>>() ?? new Dictionary<string, bool>(0);
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
                if (key.StartsWith("E"))
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
            switch (dataProtectionsOptions.StorageKind)
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
            var protectOptions = dataProtectionsOptions.KeyProtectionOptions;
            if (protectOptions != null)
            {
                switch (protectOptions.KeyProtectionKind)
                {
                    case KeyProtectionKind.AzureKeyVault:
                        builder.ProtectKeysWithAzureKeyVault(protectOptions.AzureKeyVaultKeyId, protectOptions.AzureKeyVaultClientId, protectOptions.AzureKeyVaultClientSecret);
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
    }
}