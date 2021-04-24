// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using StackExchange.Redis;

namespace Aguacongas.TheIdServer.Models
{
    public enum StorageKind
    {
        None,
        EntityFramework,
        Redis,
        AzureStorage,
        FileSystem,
        Registry,
        RavenDb,
        MongoDb
    }

    public enum KeyProtectionKind
    {
        None,
        AzureKeyVault,
        WindowsDpApi,
        X509,
        WindowsDpApiNg
    }
    public class DataProtectionOptions : Microsoft.AspNetCore.DataProtection.DataProtectionOptions
    {
        public StorageKind StorageKind { get; set; }

        public string StorageConnectionString { get; set; }

        public KeyProtectionOptions KeyProtectionOptions { get; set; }
        public RedisKey RedisKey { get; set; } = "DataProtection-Keys";
    }

    public class KeyProtectionOptions
    {
        public KeyProtectionKind KeyProtectionKind { get; set; }

        public string X509CertificateThumbprint { get; set; }

        public string AzureKeyVaultKeyId { get; set; }

        public string AzureKeyVaultClientId { get; set; }

        public string AzureKeyVaultClientSecret { get; set; }

        public bool WindowsDPAPILocalMachine { get; set; }

        public string WindowsDpApiNgSid { get; set; }

        public string WindowsDpApiNgCerticate { get; set; }
        public string X509CertificatePath { get; set; }
        public string X509CertificatePassword { get; set; }
    }
}
