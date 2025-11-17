// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
// Updated for Azure.Security.KeyVault.Keys SDK

using Aguacongas.IdentityServer.KeysRotation.AzureKeyVault;
using Azure.Core;
using Azure.Identity;
using Azure.Storage;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Moq;
using Raven.Client.Documents.Session;
using StackExchange.Redis;
using System;
using System.Security.Cryptography.X509Certificates;
using Xunit;
using static Duende.IdentityServer.IdentityServerConstants;

namespace Aguacongas.IdentityServer.KeysRotation.Test.Extensions
{
    public class KeyRotationBuilderExtensionsTest
    {
        [Fact]
        public void ProtectKeysWithAzureKeyVault_WithDefaultCredential_RegistersServices()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = new KeyRotationBuilder { Services = services };
            var vaultUri = "https://test-vault.vault.azure.net/";
            var keyName = "test-key";

            // Act
            builder.ProtectKeysWithAzureKeyVault(vaultUri, keyName);

            // Assert
            var serviceProvider = services.BuildServiceProvider();
            var keyVaultClient = serviceProvider.GetService<IKeyVaultWrappingClient>();

            Assert.NotNull(keyVaultClient);
        }

        [Fact]
        public void ProtectKeysWithAzureKeyVault_WithClientSecret_RegistersServices()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = new KeyRotationBuilder { Services = services };
            var vaultUri = "https://test-vault.vault.azure.net/";
            var keyName = "test-key";
            var tenantId = "tenant-id";
            var clientId = "client-id";
            var clientSecret = "client-secret";

            // Act
            builder.ProtectKeysWithAzureKeyVault(vaultUri, keyName, tenantId, clientId, clientSecret);

            // Assert
            var serviceProvider = services.BuildServiceProvider();
            var keyVaultClient = serviceProvider.GetService<IKeyVaultWrappingClient>();

            Assert.NotNull(keyVaultClient);
        }

        [Fact]
        public void ProtectKeysWithAzureKeyVault_WithCertificate_RegistersServices()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = new KeyRotationBuilder { Services = services };
            var vaultUri = "https://test-vault.vault.azure.net/";
            var keyName = "test-key";
            var tenantId = "tenant-id";
            var clientId = "client-id";

            // Load test certificate
            using var cert = X509CertificateLoader.LoadPkcs12FromFile("TestCert1.pfx", "password");

            // Act
            builder.ProtectKeysWithAzureKeyVault(vaultUri, keyName, tenantId, clientId, cert);

            // Assert
            var serviceProvider = services.BuildServiceProvider();
            var keyVaultClient = serviceProvider.GetService<IKeyVaultWrappingClient>();

            Assert.NotNull(keyVaultClient);
        }

        [Fact]
        public void ProtectKeysWithAzureKeyVaultManagedIdentity_RegistersServices()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = new KeyRotationBuilder { Services = services };
            var vaultUri = "https://test-vault.vault.azure.net/";
            var keyName = "test-key";

            // Act
            builder.ProtectKeysWithAzureKeyVaultManagedIdentity(vaultUri, keyName);

            // Assert
            var serviceProvider = services.BuildServiceProvider();
            var keyVaultClient = serviceProvider.GetService<IKeyVaultWrappingClient>();

            Assert.NotNull(keyVaultClient);
        }

        [Fact]
        public void ProtectKeysWithAzureKeyVaultManagedIdentity_WithClientId_RegistersServices()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = new KeyRotationBuilder { Services = services };
            var vaultUri = "https://test-vault.vault.azure.net/";
            var keyName = "test-key";
            var managedIdentityClientId = "managed-identity-client-id";

            // Act
            builder.ProtectKeysWithAzureKeyVaultManagedIdentity(vaultUri, keyName, managedIdentityClientId);

            // Assert
            var serviceProvider = services.BuildServiceProvider();
            var keyVaultClient = serviceProvider.GetService<IKeyVaultWrappingClient>();

            Assert.NotNull(keyVaultClient);
        }

        [Fact]
        public void ProtectKeysWithAzureKeyVault_WithTokenCredential_RegistersServices()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = new KeyRotationBuilder { Services = services };
            var vaultUri = new Uri("https://test-vault.vault.azure.net/");
            var keyName = "test-key";
            var credential = new DefaultAzureCredential();

            // Act
            builder.ProtectKeysWithAzureKeyVault(vaultUri, keyName, credential);

            // Assert
            var serviceProvider = services.BuildServiceProvider();
            var keyVaultClient = serviceProvider.GetService<IKeyVaultWrappingClient>();

            Assert.NotNull(keyVaultClient);
        }

        [Fact]
        public void ProtectKeysWithAzureKeyVault_WithKeyIdentifier_RegistersServices()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = new KeyRotationBuilder { Services = services };
            var keyIdentifier = "https://test-vault.vault.azure.net/keys/test-key";
            var credential = new DefaultAzureCredential();

            // Act
            builder.ProtectKeysWithAzureKeyVault(keyIdentifier, credential);

            // Assert
            var serviceProvider = services.BuildServiceProvider();
            var keyVaultClient = serviceProvider.GetService<IKeyVaultWrappingClient>();

            Assert.NotNull(keyVaultClient);
        }

        [Fact]
        public void ProtectKeysWithAzureKeyVault_NullBuilder_ThrowsArgumentNullException()
        {
            // Arrange
            IKeyRotationBuilder builder = null;
            var vaultUri = "https://test-vault.vault.azure.net/";
            var keyName = "test-key";

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                builder.ProtectKeysWithAzureKeyVault(vaultUri, keyName));
        }

        [Fact]
        public void ProtectKeysWithAzureKeyVault_NullVaultUri_ThrowsArgumentNullException()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = new KeyRotationBuilder { Services = services };
            string vaultUri = null;
            var keyName = "test-key";

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                builder.ProtectKeysWithAzureKeyVault(vaultUri, keyName));
        }

        [Fact]
        public void ProtectKeysWithAzureKeyVault_NullKeyName_ThrowsArgumentNullException()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = new KeyRotationBuilder { Services = services };
            var vaultUri = "https://test-vault.vault.azure.net/";
            string keyName = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                builder.ProtectKeysWithAzureKeyVault(vaultUri, keyName));
        }

        [Fact]
        public void ProtectKeysWithAzureKeyVault_InvalidKeyIdentifier_ThrowsArgumentException()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = new KeyRotationBuilder { Services = services };
            var keyIdentifier = "invalid-uri";
            var credential = new DefaultAzureCredential();

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                builder.ProtectKeysWithAzureKeyVault(keyIdentifier, credential));
        }

        [Fact]
        public void ProtectKeysWithAzureKeyVault_WithClientSecret_NullTenantId_ThrowsArgumentNullException()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = new KeyRotationBuilder { Services = services };
            var vaultUri = "https://test-vault.vault.azure.net/";
            var keyName = "test-key";
            string tenantId = null;
            var clientId = "client-id";
            var clientSecret = "client-secret";

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                builder.ProtectKeysWithAzureKeyVault(vaultUri, keyName, tenantId, clientId, clientSecret));
        }

        [Fact]
        public void ProtectKeysWithAzureKeyVault_WithCertificate_NullCertificate_ThrowsArgumentNullException()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = new KeyRotationBuilder { Services = services };
            var vaultUri = "https://test-vault.vault.azure.net/";
            var keyName = "test-key";
            var tenantId = "tenant-id";
            var clientId = "client-id";
            X509Certificate2 certificate = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                builder.ProtectKeysWithAzureKeyVault(vaultUri, keyName, tenantId, clientId, certificate));
        }

        [Fact]
        public void PersistKeysToAzureBlobStorage_WithBlobUriAndTokenCredential_RegistersServices()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = new KeyRotationBuilder { Services = services };
            // Act
            builder.PersistKeysToAzureBlobStorage(new Uri("https://test.blob.core.windows.net/keys?sv=test"), new Mock<TokenCredential>().Object);
            // Assert
            services.BuildServiceProvider();
            Assert.NotEmpty(services);
        }

        [Fact]
        public void PersistKeysToAzureBlobStorage_WithBlobUriAndStorageSharedKeyCredential_RegistersServices()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = new KeyRotationBuilder { Services = services };
            // Act
            builder.PersistKeysToAzureBlobStorage(new Uri("https://test.blob.core.windows.net/keys?sv=test"), new StorageSharedKeyCredential("test", "test"));
            // Assert
            services.BuildServiceProvider();
            Assert.NotEmpty(services);
        }

        [Fact]
        public void PersistKeysToAzureBlobStorage_WithInvalideConnectionString_ShouldThrow()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = new KeyRotationBuilder { Services = services };
            // Assert
            Assert.Throws<FormatException>(() => builder.PersistKeysToAzureBlobStorage("DefaultEndpointsProtocol=https;AccountName=test;AccountKey=key;EndpointSuffix=core.windows.net", "keys/container/blob", "test"));
        }

        [Fact]
        public void PersistKeysToStackExchangeRedis_WithDbFactory_RegistersServices()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = new KeyRotationBuilder { Services = services };
            // Act
            builder.PersistKeysToStackExchangeRedis(() => new Mock<IDatabase>().Object, "testInstance");
            // Assert
            services.BuildServiceProvider();
            Assert.NotEmpty(services);
        }

        [Fact]
        public void PersistKeysToStackExchangeRedis_WithConnectionMultiplexer_RegistersServices()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = new KeyRotationBuilder { Services = services };
            // Act
            builder.PersistKeysToStackExchangeRedis(new Mock<IConnectionMultiplexer>().Object);
            // Assert
            services.BuildServiceProvider();
            Assert.NotEmpty(services);
        }

        [Fact]
        public void ProtectKeysWithCertificate_WithThumbprint_RegistersServices()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = new KeyRotationBuilder { Services = services };
            var certificate = X509CertificateLoader.LoadPkcs12FromFile("TestCert1.pfx", "password");

            // Assert
            Assert.Throws<InvalidOperationException>(() => builder.ProtectKeysWithCertificate(certificate.Thumbprint));            
        }

        [Fact]
        public void AddRsaEncryptorConfiguration_ShouldConfigureOptions()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = new KeyRotationBuilder { Services = services };
            var configured = false;

            // Act
            builder.AddRsaEncryptorConfiguration(RsaSigningAlgorithm.RS256, options =>
            {
                options.EncryptionAlgorithmKeySize = 4096;
                configured = true;
            });

            // Assert
            Assert.NotEmpty(services);
        }

        [Fact]
        public void AddRsaEncryptorConfiguration_WithNullBuilder_ThrowsArgumentNullException()
        {
            // Arrange
            IKeyRotationBuilder builder = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                builder.AddRsaEncryptorConfiguration(RsaSigningAlgorithm.RS256, options => { }));
        }

        [Fact]
        public void AddRsaEncryptorConfiguration_WithNullSetupAction_ThrowsArgumentNullException()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = new KeyRotationBuilder { Services = services };

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                builder.AddRsaEncryptorConfiguration(RsaSigningAlgorithm.RS256, null));
        }

        [Fact]
        public void AddECDsaEncryptorConfiguration_ShouldConfigureOptions()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = new KeyRotationBuilder { Services = services };

            // Act
            builder.AddECDsaEncryptorConfiguration(ECDsaSigningAlgorithm.ES256, options =>
            {
                options.EncryptionAlgorithmKeySize = 256;
            });

            // Assert
            Assert.NotEmpty(services);
        }

        [Fact]
        public void AddECDsaEncryptorConfiguration_WithNullBuilder_ThrowsArgumentNullException()
        {
            // Arrange
            IKeyRotationBuilder builder = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                builder.AddECDsaEncryptorConfiguration(ECDsaSigningAlgorithm.ES256, options => { }));
        }

        [Fact]
        public void AddECDsaEncryptorConfiguration_WithNullSetupAction_ThrowsArgumentNullException()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = new KeyRotationBuilder { Services = services };

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                builder.AddECDsaEncryptorConfiguration(ECDsaSigningAlgorithm.ES256, null));
        }

        [Fact]
        public void PersistKeysToAzureBlobStorage_WithSasUri_WithoutSasToken_ThrowsArgumentException()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = new KeyRotationBuilder { Services = services };
            var uriWithoutSas = new Uri("https://test.blob.core.windows.net/container/blob");

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                builder.PersistKeysToAzureBlobStorage(uriWithoutSas));
        }

        [Fact]
        public void PersistKeysToAzureBlobStorage_WithNullBuilder_ThrowsArgumentNullException()
        {
            // Arrange
            IKeyRotationBuilder builder = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                builder.PersistKeysToAzureBlobStorage(new Uri("https://test.blob.core.windows.net/keys?sv=test")));
        }

        [Fact]
        public void PersistKeysToAzureBlobStorage_WithNullUri_ThrowsArgumentNullException()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = new KeyRotationBuilder { Services = services };

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                builder.PersistKeysToAzureBlobStorage((Uri)null));
        }

        [Fact]
        public void PersistKeysToAzureBlobStorage_WithTokenCredential_NullCredential_ThrowsArgumentNullException()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = new KeyRotationBuilder { Services = services };
            var uri = new Uri("https://test.blob.core.windows.net/keys");

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                builder.PersistKeysToAzureBlobStorage(uri, (TokenCredential)null));
        }

        [Fact]
        public void PersistKeysToAzureBlobStorage_WithSharedKeyCredential_NullCredential_ThrowsArgumentNullException()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = new KeyRotationBuilder { Services = services };
            var uri = new Uri("https://test.blob.core.windows.net/keys");

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                builder.PersistKeysToAzureBlobStorage(uri, (StorageSharedKeyCredential)null));
        }

        [Fact]
        public void PersistKeysToAzureBlobStorage_WithConnectionString_NullConnectionString_ThrowsArgumentNullException()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = new KeyRotationBuilder { Services = services };

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                builder.PersistKeysToAzureBlobStorage(null, "container", "blob"));
        }

        [Fact]
        public void PersistKeysToAzureBlobStorage_WithConnectionString_NullContainerName_ThrowsArgumentNullException()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = new KeyRotationBuilder { Services = services };

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                builder.PersistKeysToAzureBlobStorage("connection", null, "blob"));
        }

        [Fact]
        public void PersistKeysToAzureBlobStorage_WithConnectionString_NullBlobName_ThrowsArgumentNullException()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = new KeyRotationBuilder { Services = services };

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                builder.PersistKeysToAzureBlobStorage("connection", "container", null));
        }

        [Fact]
        public void PersistKeysToDbContext_ShouldConfigureRepository()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = new KeyRotationBuilder { Services = services };

            // Act
            builder.PersistKeysToDbContext<TestDbContext>();

            // Assert
            Assert.NotEmpty(services);
        }

        [Fact]
        public void PersistKeysToDbContext_WithNullBuilder_ThrowsArgumentNullException()
        {
            // Arrange
            IKeyRotationBuilder builder = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                builder.PersistKeysToDbContext<TestDbContext>());
        }

        [Fact]
        public void PersistKeysToRavenDb_WithDefaultGetSession_ShouldConfigure()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = new KeyRotationBuilder { Services = services };

            // Act
            builder.PersistKeysToRavenDb();

            // Assert
            Assert.NotEmpty(services);
        }

        [Fact]
        public void PersistKeysToRavenDb_WithCustomGetSession_ShouldConfigure()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = new KeyRotationBuilder { Services = services };

            // Act
            builder.PersistKeysToRavenDb(sp => new Mock<IDocumentSession>().Object);

            // Assert
            Assert.NotEmpty(services);
        }

        [Fact]
        public void PersistKeysToRavenDb_WithNullBuilder_ThrowsArgumentNullException()
        {
            // Arrange
            IKeyRotationBuilder builder = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                builder.PersistKeysToRavenDb());
        }

        [Fact]
        public void PersistKeysToMongoDb_WithDefaultGetCollection_ShouldConfigure()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = new KeyRotationBuilder { Services = services };

            // Act
            builder.PersistKeysToMongoDb();

            // Assert
            Assert.NotEmpty(services);
        }

        [Fact]
        public void PersistKeysToMongoDb_WithCustomGetCollection_ShouldConfigure()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = new KeyRotationBuilder { Services = services };

            // Act
            builder.PersistKeysToMongoDb(sp => new Mock<IMongoCollection<Aguacongas.IdentityServer.KeysRotation.MongoDb.KeyRotationKey>>().Object);

            // Assert
            Assert.NotEmpty(services);
        }

        [Fact]
        public void PersistKeysToMongoDb_WithNullBuilder_ThrowsArgumentNullException()
        {
            // Arrange
            IKeyRotationBuilder builder = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                builder.PersistKeysToMongoDb());
        }

        [Fact]
        public void PersistKeysToFileSystem_ShouldConfigure()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = new KeyRotationBuilder { Services = services };
            var directory = new System.IO.DirectoryInfo(System.IO.Path.GetTempPath());

            // Act
            builder.PersistKeysToFileSystem(directory);

            // Assert
            Assert.NotEmpty(services);
        }

        [Fact]
        public void PersistKeysToFileSystem_WithNullBuilder_ThrowsArgumentNullException()
        {
            // Arrange
            IKeyRotationBuilder builder = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                builder.PersistKeysToFileSystem(new System.IO.DirectoryInfo("C:\\temp")));
        }

        [Fact]
        public void PersistKeysToFileSystem_WithNullDirectory_ThrowsArgumentNullException()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = new KeyRotationBuilder { Services = services };

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                builder.PersistKeysToFileSystem(null));
        }

        [Fact]
        public void PersistKeysToStackExchangeRedis_WithDatabaseFactory_NullFactory_ThrowsArgumentNullException()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = new KeyRotationBuilder { Services = services };

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                builder.PersistKeysToStackExchangeRedis((IConnectionMultiplexer)null, "key"));
        }

        [Fact]
        public void PersistKeysToStackExchangeRedis_WithConnectionMultiplexer_WithKey_ShouldConfigure()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = new KeyRotationBuilder { Services = services };
            var mockMultiplexer = new Mock<IConnectionMultiplexer>();
            mockMultiplexer.Setup(m => m.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                .Returns(new Mock<IDatabase>().Object);

            // Act
            builder.PersistKeysToStackExchangeRedis(mockMultiplexer.Object, "custom-key");

            // Assert
            Assert.NotEmpty(services);
        }

        [Fact]
        public void PersistKeysToStackExchangeRedis_WithConnectionMultiplexer_NullMultiplexer_ThrowsArgumentNullException()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = new KeyRotationBuilder { Services = services };

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                builder.PersistKeysToStackExchangeRedis((IConnectionMultiplexer)null));
        }

        [Fact]
        public void ProtectKeysWithCertificate_WithCertificate_ShouldConfigure()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = new KeyRotationBuilder { Services = services };
            var certificate = X509CertificateLoader.LoadPkcs12FromFile("TestCert1.pfx", "password");

            // Act
            builder.ProtectKeysWithCertificate(certificate);

            // Assert
            Assert.NotEmpty(services);
        }

        [Fact]
        public void ProtectKeysWithCertificate_WithNullBuilder_ThrowsArgumentNullException()
        {
            // Arrange
            IKeyRotationBuilder builder = null;
            var certificate = X509CertificateLoader.LoadPkcs12FromFile("TestCert1.pfx", "password");

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                builder.ProtectKeysWithCertificate(certificate));
        }

        [Fact]
        public void ProtectKeysWithCertificate_WithNullCertificate_ThrowsArgumentNullException()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = new KeyRotationBuilder { Services = services };

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                builder.ProtectKeysWithCertificate((X509Certificate2)null));
        }

        [Fact]
        public void ProtectKeysWithCertificate_WithThumbprint_NullThumbprint_ThrowsArgumentNullException()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = new KeyRotationBuilder { Services = services };

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                builder.ProtectKeysWithCertificate((string)null));
        }

        [Fact]
        public void ProtectKeysWithAzureKeyVault_ObsoleteMethod_WithCertificate_ShouldWork()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = new KeyRotationBuilder { Services = services };
            var cert = X509CertificateLoader.LoadPkcs12FromFile("TestCert1.pfx", "password");

#pragma warning disable CS0618 // Type or member is obsolete
            // Act
            builder.ProtectKeysWithAzureKeyVault(
                "https://vault.vault.azure.net/keys/key-name",
                "client-id",
                cert);
#pragma warning restore CS0618

            // Assert
            var serviceProvider = services.BuildServiceProvider();
            Assert.NotNull(serviceProvider.GetService<IKeyVaultWrappingClient>());
        }

        [Fact]
        public void ProtectKeysWithAzureKeyVault_ObsoleteMethod_WithClientSecret_ShouldWork()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = new KeyRotationBuilder { Services = services };

#pragma warning disable CS0618
            // Act
            builder.ProtectKeysWithAzureKeyVault(
                "https://vault.vault.azure.net/keys/key-name",
                "client-id",
                "client-secret");
#pragma warning restore CS0618

            // Assert
            var serviceProvider = services.BuildServiceProvider();
            Assert.NotNull(serviceProvider.GetService<IKeyVaultWrappingClient>());
        }

        [Fact]
        public void ProtectKeysWithAzureKeyVault_ObsoleteMethod_WithInvalidKeyIdentifier_ThrowsArgumentException()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = new KeyRotationBuilder { Services = services };

#pragma warning disable CS0618
            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                builder.ProtectKeysWithAzureKeyVault(
                    "https://vault.vault.azure.net/invalid-path",
                    "client-id",
                    "client-secret"));
#pragma warning restore CS0618
        }

        [Theory]
        [InlineData("https://vault.vault.azure.net/", "key-name")]
        [InlineData("https://test-vault.vault.azure.net/", "test-key")]
        [InlineData("https://prod.vault.azure.net/", "prod-signing-key")]
        public void ProtectKeysWithAzureKeyVault_WithVariousUris_ShouldConfigure(string vaultUri, string keyName)
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = new KeyRotationBuilder { Services = services };

            // Act
            builder.ProtectKeysWithAzureKeyVault(vaultUri, keyName);

            // Assert
            var serviceProvider = services.BuildServiceProvider();
            Assert.NotNull(serviceProvider.GetService<IKeyVaultWrappingClient>());
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void ProtectKeysWithAzureKeyVault_WithEmptyKeyName_ThrowsArgumentNullException(string keyName)
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = new KeyRotationBuilder { Services = services };

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                builder.ProtectKeysWithAzureKeyVault("https://vault.vault.azure.net/", keyName));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void ProtectKeysWithAzureKeyVault_WithClientSecret_EmptyTenantId_ThrowsArgumentNullException(string tenantId)
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = new KeyRotationBuilder { Services = services };

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                builder.ProtectKeysWithAzureKeyVault(
                    "https://vault.vault.azure.net/",
                    "key-name",
                    tenantId,
                    "client-id",
                    "client-secret"));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void ProtectKeysWithAzureKeyVault_WithClientSecret_EmptyClientId_ThrowsArgumentNullException(string clientId)
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = new KeyRotationBuilder { Services = services };

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                builder.ProtectKeysWithAzureKeyVault(
                    "https://vault.vault.azure.net/",
                    "key-name",
                    "tenant-id",
                    clientId,
                    "client-secret"));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void ProtectKeysWithAzureKeyVault_WithClientSecret_EmptyClientSecret_ThrowsArgumentNullException(string clientSecret)
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = new KeyRotationBuilder { Services = services };

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                builder.ProtectKeysWithAzureKeyVault(
                    "https://vault.vault.azure.net/",
                    "key-name",
                    "tenant-id",
                    "client-id",
                    clientSecret));
        }

        // Helper class for testing
        private class KeyRotationBuilder : IKeyRotationBuilder
        {
            public IServiceCollection Services { get; set; }
        }

        private class TestDbContext : Microsoft.EntityFrameworkCore.DbContext, Aguacongas.IdentityServer.KeysRotation.EntityFrameworkCore.IKeyRotationContext
        {
            public Microsoft.EntityFrameworkCore.DbSet<Aguacongas.IdentityServer.KeysRotation.EntityFrameworkCore.KeyRotationKey> KeyRotationKeys { get; set; }
        }

    }
}