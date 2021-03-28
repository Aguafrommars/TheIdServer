// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Admin.Configuration;
using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.IdentityServer.KeysRotation.RavenDb;
using Aguacongas.TheIdServer.Models;
using Azure.Identity;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Win32;
using StackExchange.Redis;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DataProtectionBuilderExtentions
    {       
        public static IDataProtectionBuilder ConfigureDataProtection(this IDataProtectionBuilder builder, IConfiguration configuration)
        {
            var dataProtectionsOptions = configuration.Get<Aguacongas.TheIdServer.Models.DataProtectionOptions>();
            if (dataProtectionsOptions == null)
            {
                return builder;
            }
            builder.AddKeyManagementOptions(options => configuration.GetSection(nameof(KeyManagementOptions))?.Bind(options));
            ConfigureEncryptionAlgorithm(builder, configuration);
            switch (dataProtectionsOptions.StorageKind)
            {
                case StorageKind.AzureStorage:
                    builder.PersistKeysToAzureBlobStorage(blobSasUri: new Uri(dataProtectionsOptions.StorageConnectionString));
                    break;
                case StorageKind.EntityFramework:
                    builder.PersistKeysToDbContext<OperationalDbContext>();
                    break;
                case StorageKind.RavenDb:
                    builder.PersistKeysToRavenDb<DocumentSessionWrapper>();
                    break;
                case StorageKind.FileSytem:
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
                case StorageKind.Registry:
#pragma warning disable CA1416 // Validate platform compatibility
                    builder.PersistKeysToRegistry(Registry.CurrentUser.OpenSubKey(dataProtectionsOptions.StorageConnectionString));
#pragma warning restore CA1416 // Validate platform compatibility
                    break;
            }
            var protectOptions = dataProtectionsOptions.KeyProtectionOptions;
            if (protectOptions != null)
            {
                switch (protectOptions.KeyProtectionKind)
                {
                    case KeyProtectionKind.AzureKeyVault:
                        builder.ProtectKeysWithAzureKeyVault(new Uri(protectOptions.AzureKeyVaultKeyId), new DefaultAzureCredential());
                        break;
                    case KeyProtectionKind.WindowsDpApi:
                        builder.ProtectKeysWithDpapi(protectOptions.WindowsDPAPILocalMachine);
                        break;
                    case KeyProtectionKind.WindowsDpApiNg:
                        ConfigureWindowsDpApiNg(builder, protectOptions);
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

            return builder;
        }

        private static void ConfigureEncryptionAlgorithm(IDataProtectionBuilder builder, IConfiguration configuration)
        {
            var encryptorConfiguration = configuration.GetSection(nameof(AuthenticatedEncryptorConfiguration)).Get<AuthenticatedEncryptorConfiguration>();
            if (encryptorConfiguration != null)
            {
                builder.UseCryptographicAlgorithms(encryptorConfiguration);
            }
        }

        private static void ConfigureWindowsDpApiNg(IDataProtectionBuilder builder, KeyProtectionOptions protectOptions)
        {
            if (!string.IsNullOrEmpty(protectOptions.WindowsDpApiNgCerticate))
            {
                builder.ProtectKeysWithDpapiNG($"CERTIFICATE=HashId:{protectOptions.WindowsDpApiNgCerticate}", flags: DpapiNGProtectionDescriptorFlags.None);
                return;
            }
            if (!string.IsNullOrEmpty(protectOptions.WindowsDpApiNgSid))
            {
                builder.ProtectKeysWithDpapiNG($"SID={protectOptions.WindowsDpApiNgSid}", flags: DpapiNGProtectionDescriptorFlags.None);
                return;
            }
            builder.ProtectKeysWithDpapiNG();
        }
    }
}
