// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Admin.Configuration;
using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.IdentityServer.KeysRotation;
using Aguacongas.IdentityServer.KeysRotation.Extensions;
using Aguacongas.TheIdServer.Models;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdentityServerBuilderExtensions
    {
        public static IIdentityServerBuilder ConfigureKey(this IIdentityServerBuilder identityServerBuilder, IConfiguration configuration)
        {
            if (configuration.GetValue<KeyKinds>("Type") != KeyKinds.KeysRotation)
            {
                identityServerBuilder.AddSigningCredentials();
                return identityServerBuilder;
            }

            var builder = identityServerBuilder.AddKeysRotation(options => configuration.GetSection(nameof(KeyRotationOptions))?.Bind(options))
                .AddRsaEncryptorConfiguration(options => configuration.GetSection(nameof(RsaEncryptorConfiguration))?.Bind(options));
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