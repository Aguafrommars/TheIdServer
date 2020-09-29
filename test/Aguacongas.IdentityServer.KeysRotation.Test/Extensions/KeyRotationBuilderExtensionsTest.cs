using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.IdentityServer.KeysRotation.EntityFrameworkCore;
using Aguacongas.IdentityServer.KeysRotation.XmlEncryption;
using Microsoft.AspNetCore.DataProtection.AzureStorage;
using Microsoft.AspNetCore.DataProtection.StackExchangeRedis;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Security.Cryptography.X509Certificates;
using Xunit;

namespace Aguacongas.IdentityServer.KeysRotation.Test.Extensions
{
    public class KeyRotationBuilderExtensionsTest
    {
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
        public void ProtectKeysWithCertificate_should_add_CertificateXmlEncryptor()
        {
            using var store = new X509Store(StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
            var collection = store.Certificates.Find(X509FindType.FindByTimeValid, DateTime.Now, true);
            
            var builder = new ServiceCollection()
                .AddKeysRotation()
                .ProtectKeysWithCertificate(collection[0].Thumbprint);

            var provider = builder.Services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<KeyRotationOptions>>();
            Assert.IsType<CertificateXmlEncryptor>(options.Value.XmlEncryptor);
        }

    }
}
