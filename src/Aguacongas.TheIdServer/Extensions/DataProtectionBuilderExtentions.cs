using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.TheIdServer.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;
using StackExchange.Redis;
using System;
using System.IO;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DataProtectionBuilderExtentions
    {       
        public static IDataProtectionBuilder ConfigureDataProtection(this IDataProtectionBuilder builder, IConfiguration configuration)
        {
            var dataProtectionsOptions = configuration.Get<Aguacongas.TheIdServer.Models.DataProtectionOptions>();
            switch (dataProtectionsOptions.StorageKind)
            {
                case StorageKind.AzureStorage:
                    builder.PersistKeysToAzureBlobStorage(new Uri(dataProtectionsOptions.StorageConnectionString));
                    break;
                case StorageKind.EntityFramework:
                    builder.PersistKeysToDbContext<OperationalDbContext>();
                    break;
                case StorageKind.FileSytem:
                    builder.PersistKeysToFileSystem(new DirectoryInfo(dataProtectionsOptions.StorageConnectionString));
                    break;
                case StorageKind.Redis:
                    var redis = ConnectionMultiplexer.Connect(dataProtectionsOptions.StorageConnectionString);
                    builder.PersistKeysToStackExchangeRedis(redis, dataProtectionsOptions.RedisKey);
                    break;
                case StorageKind.Registry:
                    builder.PersistKeysToRegistry(Registry.CurrentUser.OpenSubKey(dataProtectionsOptions.StorageConnectionString));
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
                    case KeyProtectionKind.WindowsDpApi:
                        builder.ProtectKeysWithDpapi(protectOptions.WindowsDPAPILocalMachine);
                        break;
                    case KeyProtectionKind.WindowsDpApiNg:
                        if (!string.IsNullOrEmpty(protectOptions.WindowsDpApiNgCerticate))
                        {
                            builder.ProtectKeysWithDpapiNG($"CERTIFICATE=HashId:{protectOptions.WindowsDpApiNgCerticate}", flags: DpapiNGProtectionDescriptorFlags.None);
                            break;
                        }
                        if (!string.IsNullOrEmpty(protectOptions.WindowsDpApiNgSid))
                        {
                            builder.ProtectKeysWithDpapiNG($"SID={protectOptions.WindowsDpApiNgSid}", flags: DpapiNGProtectionDescriptorFlags.None);
                            break;
                        }
                        builder.ProtectKeysWithDpapiNG();
                        break;
                    case KeyProtectionKind.X509:
                        builder.ProtectKeysWithCertificate(protectOptions.X509CertificateThumbprint);
                        break;
                }                
            }

            return builder;
        }
    }
}
