// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.IdentityServer.KeysRotation.AzureKeyVault;
using Aguacongas.IdentityServer.KeysRotation.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection.AzureStorage;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.AspNetCore.DataProtection.StackExchangeRedis;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.IdentityServer.KeysRotation.Test.Extensions
{
    public class KeyRotationBuilderExtensionsTest
    {
        [Fact]
        public void AddRsaEncryptorConfiguration_should_throw_ArgumentNulException_on_builder_null()
        {
            Assert.Throws<ArgumentNullException>(() => KeyRotationBuilderExtensions.AddRsaEncryptorConfiguration(null, null));
            Assert.Throws<ArgumentNullException>(() => KeyRotationBuilderExtensions.AddRsaEncryptorConfiguration(new KeyRotationBuilder(), null));
        }

        [Fact]
        public void AddRsaEncryptorConfiguration_should_configure_RsaEncryptorConfiguration()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddKeysRotation()
                .AddRsaEncryptorConfiguration(options =>
                {
                    options.RsaSigningAlgorithm = IdentityServer4.IdentityServerConstants.RsaSigningAlgorithm.PS256;
                });

            var services = serviceCollection.BuildServiceProvider();
            var options = services.GetRequiredService<IOptions<KeyRotationOptions>>();
            Assert.IsType<RsaEncryptorConfiguration>(options.Value.AuthenticatedEncryptorConfiguration);
            var configuration = options.Value.AuthenticatedEncryptorConfiguration as RsaEncryptorConfiguration;
            Assert.Equal(IdentityServer4.IdentityServerConstants.RsaSigningAlgorithm.PS256, configuration.RsaSigningAlgorithm);
        }

        [Fact]
        public void PersistKeysToAzureBlobStorage_should_throw_ArgumentNulException_on_builder_null()
        {
            Assert.Throws<ArgumentNullException>(() => KeyRotationBuilderExtensions.PersistKeysToAzureBlobStorage(null, blobReference: null));
            Assert.Throws<ArgumentNullException>(() => KeyRotationBuilderExtensions.PersistKeysToAzureBlobStorage(new KeyRotationBuilder(), blobReference: null));
            Assert.Throws<ArgumentNullException>(() => KeyRotationBuilderExtensions.PersistKeysToAzureBlobStorage(null, blobUri: null));
            Assert.Throws<ArgumentNullException>(() => KeyRotationBuilderExtensions.PersistKeysToAzureBlobStorage(new KeyRotationBuilder(), blobUri: null));
            Assert.Throws<ArgumentException>(() => KeyRotationBuilderExtensions.PersistKeysToAzureBlobStorage(new KeyRotationBuilder(), new Uri("http://www.example.com")));
            Assert.Throws<ArgumentNullException>(() => KeyRotationBuilderExtensions.PersistKeysToAzureBlobStorage(null, blobUri: null));
            Assert.Throws<ArgumentNullException>(() => KeyRotationBuilderExtensions.PersistKeysToAzureBlobStorage(null, storageAccount: null, null));
            Assert.Throws<ArgumentNullException>(() => KeyRotationBuilderExtensions.PersistKeysToAzureBlobStorage(new KeyRotationBuilder(), storageAccount: null, null));
            Assert.Throws<ArgumentNullException>(() => KeyRotationBuilderExtensions.PersistKeysToAzureBlobStorage(new KeyRotationBuilder(), new CloudStorageAccount(new StorageCredentials("test", "test"), true), null));
            Assert.Throws<ArgumentNullException>(() => KeyRotationBuilderExtensions.PersistKeysToAzureBlobStorage(null, container: null, null));
            Assert.Throws<ArgumentNullException>(() => KeyRotationBuilderExtensions.PersistKeysToAzureBlobStorage(new KeyRotationBuilder(), container: null, null));
            Assert.Throws<ArgumentNullException>(() => KeyRotationBuilderExtensions.PersistKeysToAzureBlobStorage(new KeyRotationBuilder(), new CloudBlobContainer(new Uri("http://www.example.com")), null));
        }

        [Fact]
        public void PersistKeysToAzureBlobStorage_uses_AzureBlobXmlRepository_with_CloudStorageAccount()
        {
            // Arrange
            var account = new CloudStorageAccount(new StorageCredentials("test", "test"), true);
            var serviceCollection = new ServiceCollection();
            var builder = serviceCollection.AddKeysRotation();

            // Act
            builder.PersistKeysToAzureBlobStorage(account, "keys.xml");
            var services = serviceCollection.BuildServiceProvider();

            // Assert
            var options = services.GetRequiredService<IOptions<KeyRotationOptions>>();
            Assert.IsType<AzureBlobXmlRepository>(options.Value.XmlRepository);
        }

        [Fact]
        public void PersistKeysToAzureBlobStorage_uses_AzureBlobXmlRepository_with_blobUri()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();
            var builder = serviceCollection.AddKeysRotation();

            // Act
            builder.PersistKeysToAzureBlobStorage(new Uri("http://www.example.com?blobUri=test"));
            var services = serviceCollection.BuildServiceProvider();

            // Assert
            var options = services.GetRequiredService<IOptions<KeyRotationOptions>>();
            Assert.IsType<AzureBlobXmlRepository>(options.Value.XmlRepository);
        }

        [Fact]
        public void PersistKeysToAzureBlobStorage_uses_AzureBlobXmlRepository_with_blobReference()
        {
            // Arrange
            var storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
            var container = storageAccount.CreateCloudBlobClient().GetContainerReference("temp");
            var blobReference = container.GetBlockBlobReference("test.txt");

            var serviceCollection = new ServiceCollection();
            var builder = serviceCollection.AddKeysRotation();

            // Act
            builder.PersistKeysToAzureBlobStorage(blobReference);
            var services = serviceCollection.BuildServiceProvider();

            // Assert
            var options = services.GetRequiredService<IOptions<KeyRotationOptions>>();
            Assert.IsType<AzureBlobXmlRepository>(options.Value.XmlRepository);
        }

        [Fact]
        public void PersistKeysToAzureBlobStorage_uses_AzureBlobXmlRepository_with_container()
        {
            // Arrange
            var container = new CloudBlobContainer(new Uri("http://www.example.com"));
            var serviceCollection = new ServiceCollection();
            var builder = serviceCollection.AddKeysRotation();

            // Act
            builder.PersistKeysToAzureBlobStorage(container, "keys.xml");
            var services = serviceCollection.BuildServiceProvider();

            // Assert
            var options = services.GetRequiredService<IOptions<KeyRotationOptions>>();
            Assert.IsType<AzureBlobXmlRepository>(options.Value.XmlRepository);
        }

        [Fact]
        public void PersistKeysToDbContext_should_throw_ArgumentNulException_on_builder_null()
        {
            Assert.Throws<ArgumentNullException>(() => KeyRotationBuilderExtensions.PersistKeysToDbContext<OperationalDbContext>(null));
        }

        [Fact]
        public void PersistKeysToDbContext_should_add_EntityFrameworkCoreXmlRepository()
        {
            var builder = new ServiceCollection()
                .AddKeysRotation()
                .PersistKeysToDbContext<OperationalDbContext>();
            var provider = builder.Services.BuildServiceProvider();
            var options =  provider.GetRequiredService<IOptions<KeyRotationOptions>>();

            Assert.NotNull(options.Value.XmlRepository);
            Assert.IsType<EntityFrameworkCoreXmlRepository<OperationalDbContext>>(options.Value.XmlRepository);
        }

        [Fact]
        public void PersistKeysToFileSystem_should_throw_ArgumentNulException_on_builder_null()
        {
            Assert.Throws<ArgumentNullException>(() => KeyRotationBuilderExtensions.PersistKeysToFileSystem(null, null));
            Assert.Throws<ArgumentNullException>(() => KeyRotationBuilderExtensions.PersistKeysToFileSystem(new KeyRotationBuilder(), null));
        }

        [Fact]
        public void PersistKeysToFileSystem_should_add_FileSystemXmlRepository()
        {
            var builder = new ServiceCollection()
                .AddKeysRotation()
                .PersistKeysToFileSystem(new DirectoryInfo(Path.GetTempPath()));
            var provider = builder.Services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<KeyRotationOptions>>();

            Assert.NotNull(options.Value.XmlRepository);
            Assert.IsType<FileSystemXmlRepository>(options.Value.XmlRepository);
        }

        [Fact]
        public void PersistKeysToStackExchangeRedis_should_throw_ArgumentNulException_on_builder_null()
        {
            Assert.Throws<ArgumentNullException>(() => KeyRotationBuilderExtensions.PersistKeysToStackExchangeRedis(null, null));
            Assert.Throws<ArgumentNullException>(() => KeyRotationBuilderExtensions.PersistKeysToStackExchangeRedis(null, databaseFactory: null, ""));
            Assert.Throws<ArgumentNullException>(() => KeyRotationBuilderExtensions.PersistKeysToStackExchangeRedis(new KeyRotationBuilder(), null));
            Assert.Throws<ArgumentNullException>(() => KeyRotationBuilderExtensions.PersistKeysToStackExchangeRedis(new KeyRotationBuilder(), databaseFactory: null, ""));
            Assert.Throws<ArgumentNullException>(() => KeyRotationBuilderExtensions.PersistKeysToStackExchangeRedis(new KeyRotationBuilder(), connectionMultiplexer: null, ""));
        }

        [Fact]
        public void PersistKeysToStackExchangeRedis_should_add_RedisXmlRepository()
        {
            var builder = new ServiceCollection()
                .AddKeysRotation()
                .PersistKeysToStackExchangeRedis(ConnectionMultiplexer.Connect("localhost:6379"));
            var provider = builder.Services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<KeyRotationOptions>>();

            Assert.NotNull(options.Value.XmlRepository);
            Assert.IsType<RedisXmlRepository>(options.Value.XmlRepository);
        }

        [Fact]
        public void PersistKeysToStackExchangeRedis_should_add_RedisXmlRepository2()
        {
            var builder = new ServiceCollection()
                .AddKeysRotation()
                .PersistKeysToStackExchangeRedis(() => ConnectionMultiplexer.Connect("localhost:6379").GetDatabase(), "test");
            var provider = builder.Services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<KeyRotationOptions>>();

            Assert.NotNull(options.Value.XmlRepository);
            Assert.IsType<RedisXmlRepository>(options.Value.XmlRepository);
        }

        [Fact]
        public void PersistKeysToStackExchangeRedis_should_add_RedisXmlRepository3()
        {
            var builder = new ServiceCollection()
                .AddKeysRotation()
                .PersistKeysToStackExchangeRedis(ConnectionMultiplexer.Connect("localhost:6379"), "test");
            var provider = builder.Services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<KeyRotationOptions>>();

            Assert.NotNull(options.Value.XmlRepository);
            Assert.IsType<RedisXmlRepository>(options.Value.XmlRepository);
        }

        [Fact]
        public void ProtectKeysWithCertificate_should_throw_ArgumentNulException_on_builder_null()
        {
            Assert.Throws<ArgumentNullException>(() => KeyRotationBuilderExtensions.ProtectKeysWithCertificate(null, certificate: null));
            Assert.Throws<ArgumentNullException>(() => KeyRotationBuilderExtensions.ProtectKeysWithCertificate(null, thumbprint: null));
            Assert.Throws<ArgumentNullException>(() => KeyRotationBuilderExtensions.ProtectKeysWithCertificate(new KeyRotationBuilder(), certificate: null));
            Assert.Throws<ArgumentNullException>(() => KeyRotationBuilderExtensions.ProtectKeysWithCertificate(new KeyRotationBuilder(), thumbprint: null));
            Assert.Throws<InvalidOperationException>(() => KeyRotationBuilderExtensions.ProtectKeysWithCertificate(new KeyRotationBuilder(), thumbprint: "test"));
        }

        [Fact]
        public void ProtectKeysWithAzureKeyVault_should_throw_ArgumentNulException_on_builder_null()
        {
            Assert.Throws<ArgumentNullException>(() => KeyRotationBuilderExtensions.ProtectKeysWithAzureKeyVault(null, null, null));
            Assert.Throws<ArgumentNullException>(() => KeyRotationBuilderExtensions.ProtectKeysWithAzureKeyVault(new KeyRotationBuilder(), null, null));
            Assert.Throws<ArgumentNullException>(() => KeyRotationBuilderExtensions.ProtectKeysWithAzureKeyVault(new KeyRotationBuilder(), new KeyVaultClient((a, r, s) => Task.FromResult("test")), null));
            Assert.Throws<ArgumentNullException>(() => KeyRotationBuilderExtensions.ProtectKeysWithAzureKeyVault(null, null, null, certificate: null));
            Assert.Throws<ArgumentNullException>(() => KeyRotationBuilderExtensions.ProtectKeysWithAzureKeyVault(null, null, "test", certificate: null));
            Assert.Throws<ArgumentNullException>(() => KeyRotationBuilderExtensions.ProtectKeysWithAzureKeyVault(null, null, null, clientSecret: null));
            Assert.Throws<ArgumentNullException>(() => KeyRotationBuilderExtensions.ProtectKeysWithAzureKeyVault(null, null, "test", clientSecret: null));            
        }

        [Fact]
        public void ProtectKeysWithAzureKeyVault_should_add_AzureKeyVaultXmlEncryptor_with_cert()
        {
            var builder = new ServiceCollection()
                .AddKeysRotation()
                .ProtectKeysWithAzureKeyVault("test", "test", new X509Certificate2());
            var provider = builder.Services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<KeyRotationOptions>>();

            Assert.NotNull(options.Value.XmlEncryptor);
            Assert.IsType<AzureKeyVaultXmlEncryptor>(options.Value.XmlEncryptor);
        }

        [Fact]
        public void ProtectKeysWithAzureKeyVault_should_add_AzureKeyVaultXmlEncryptor_with_secret()
        {
            var builder = new ServiceCollection()
                .AddKeysRotation()
                .ProtectKeysWithAzureKeyVault("test", "test", "test");
            var provider = builder.Services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<KeyRotationOptions>>();

            Assert.NotNull(options.Value.XmlEncryptor);
            Assert.IsType<AzureKeyVaultXmlEncryptor>(options.Value.XmlEncryptor);
        }
    }
}
