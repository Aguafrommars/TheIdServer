// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre

using Aguacongas.IdentityServer.KeysRotation.AzureKeyVault;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using Xunit;

namespace Aguacongas.TheIdServer.Duende.Test.Extensions
{
    public class IdentityServerBuilderExtensionsTest
    {
        [Fact]
        public void ConfigureKey_WithNonKeysRotationType_ShouldAddSigningCredentials()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = services.AddIdentityServer();

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["Type"] = "File"
                })
                .Build();

            // Act
            var result = builder.ConfigureKey(configuration);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void ConfigureKey_WithKeysRotation_EntityFramework_ShouldConfigureCorrectly()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = services.AddIdentityServer();

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["Type"] = "KeysRotation",
                    ["StorageKind"] = "EntityFramework"
                })
                .Build();

            // Act
            var result = builder.ConfigureKey(configuration);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void ConfigureKey_WithAzureKeyVault_DefaultCredential_ShouldConfigure()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = services.AddIdentityServer();

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["Type"] = "KeysRotation",
                    ["StorageKind"] = "EntityFramework",
                    ["KeyProtectionOptions:KeyProtectionKind"] = "AzureKeyVault",
                    ["KeyProtectionOptions:AzureKeyVaultKeyId"] = "https://test.vault.azure.net/keys/test-key"
                })
                .Build();

            // Act
            var result = builder.ConfigureKey(configuration);

            // Assert
            Assert.NotNull(result);
            var provider = services.BuildServiceProvider();
            var keyVaultClient = provider.GetService<IKeyVaultWrappingClient>();
            Assert.NotNull(keyVaultClient);
        }

        [Fact]
        public void ConfigureKey_WithAzureKeyVault_ClientSecret_ShouldConfigure()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = services.AddIdentityServer();

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["Type"] = "KeysRotation",
                    ["StorageKind"] = "EntityFramework",
                    ["KeyProtectionOptions:KeyProtectionKind"] = "AzureKeyVault",
                    ["KeyProtectionOptions:AzureKeyVaultKeyId"] = "https://test.vault.azure.net/keys/test-key",
                    ["KeyProtectionOptions:AzureKeyVaultTenantId"] = "tenant-id",
                    ["KeyProtectionOptions:AzureKeyVaultClientId"] = "client-id",
                    ["KeyProtectionOptions:AzureKeyVaultClientSecret"] = "client-secret"
                })
                .Build();

            // Act
            var result = builder.ConfigureKey(configuration);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void ConfigureKey_WithAzureKeyVault_ClientSecret_NoTenantId_ShouldThrow()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = services.AddIdentityServer();

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["Type"] = "KeysRotation",
                    ["StorageKind"] = "EntityFramework",
                    ["KeyProtectionOptions:KeyProtectionKind"] = "AzureKeyVault",
                    ["KeyProtectionOptions:AzureKeyVaultKeyId"] = "https://test.vault.azure.net/keys/test-key",
                    ["KeyProtectionOptions:AzureKeyVaultClientId"] = "client-id",
                    ["KeyProtectionOptions:AzureKeyVaultClientSecret"] = "client-secret"
                })
                .Build();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => builder.ConfigureKey(configuration));
        }

        [Fact]
        public void ConfigureKey_WithAzureKeyVault_Certificate_ShouldRequireTenantId()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = services.AddIdentityServer();

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["Type"] = "KeysRotation",
                    ["StorageKind"] = "EntityFramework",
                    ["KeyProtectionOptions:KeyProtectionKind"] = "AzureKeyVault",
                    ["KeyProtectionOptions:AzureKeyVaultKeyId"] = "https://test.vault.azure.net/keys/test-key",
                    ["KeyProtectionOptions:AzureKeyVaultClientId"] = "client-id",
                    ["KeyProtectionOptions:AzureKeyVaultCertificateThumbprint"] = "cert-thumbprint"
                })
                .Build();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => builder.ConfigureKey(configuration));
        }

        [Fact]
        public void ConfigureKey_WithAzureKeyVault_ManagedIdentity_ShouldConfigure()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = services.AddIdentityServer();

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["Type"] = "KeysRotation",
                    ["StorageKind"] = "EntityFramework",
                    ["KeyProtectionOptions:KeyProtectionKind"] = "AzureKeyVault",
                    ["KeyProtectionOptions:AzureKeyVaultKeyId"] = "https://test.vault.azure.net/keys/test-key",
                    ["KeyProtectionOptions:AzureKeyVaultManagedIdentityClientId"] = "managed-id"
                })
                .Build();

            // Act
            var result = builder.ConfigureKey(configuration);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void ConfigureKey_WithInvalidKeyId_ShouldThrow()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = services.AddIdentityServer();

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["Type"] = "KeysRotation",
                    ["StorageKind"] = "EntityFramework",
                    ["KeyProtectionOptions:KeyProtectionKind"] = "AzureKeyVault",
                    ["KeyProtectionOptions:AzureKeyVaultKeyId"] = "invalid-uri"
                })
                .Build();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => builder.ConfigureKey(configuration));
        }

        [Fact]
        public void ConfigureKey_WithInvalidKeyIdFormat_ShouldThrow()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = services.AddIdentityServer();

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["Type"] = "KeysRotation",
                    ["StorageKind"] = "EntityFramework",
                    ["KeyProtectionOptions:KeyProtectionKind"] = "AzureKeyVault",
                    ["KeyProtectionOptions:AzureKeyVaultKeyId"] = "https://test.vault.azure.net/wrong-path/test-key"
                })
                .Build();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => builder.ConfigureKey(configuration));
        }

        [Fact]
        public void ConfigureKey_WithX509Certificate_ShouldConfigure()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = services.AddIdentityServer();

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["Type"] = "KeysRotation",
                    ["StorageKind"] = "EntityFramework",
                    ["KeyProtectionOptions:KeyProtectionKind"] = "X509",
                    ["KeyProtectionOptions:X509CertificatePath"] = "TestCert1.pfx",
                    ["KeyProtectionOptions:X509CertificatePassword"] = "password"
                })
                .Build();

            // Act
            var result = builder.ConfigureKey(configuration);

            // Assert
            Assert.NotNull(result);
        }

        [Theory]
        [InlineData("AzureStorage")]
        [InlineData("EntityFramework")]
        [InlineData("Redis")]
        [InlineData("FileSystem")]
        [InlineData("RavenDb")]
        [InlineData("MongoDb")]
        public void ConfigureKey_WithDifferentStorageKinds_ShouldConfigure(string storageKind)
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = services.AddIdentityServer();

            var config = new Dictionary<string, string>
            {
                ["Type"] = "KeysRotation",
                ["StorageKind"] = storageKind
            };

            if (storageKind == "AzureStorage")
            {
                config["StorageConnectionString"] = "https://test.blob.core.windows.net/keys?sv=test";
            }
            else if (storageKind == "Redis")
            {
                config["StorageConnectionString"] = "localhost:6379";
            }
            else if (storageKind == "FileSystem")
            {
                config["StorageConnectionString"] = "C:\\temp\\keys";
            }

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(config)
                .Build();

            // Act
            var result = builder.ConfigureKey(configuration);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void ConfigureKey_WithAdditionalSigningKeyTypes_ShouldConfigure()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = services.AddIdentityServer();

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["Type"] = "KeysRotation",
                    ["StorageKind"] = "EntityFramework",
                    ["AdditionalSigningKeyType:RS384:SigningAlgorithm"] = "RS384",
                    ["AdditionalSigningKeyType:ES256:SigningAlgorithm"] = "ES256"
                })
                .Build();

            // Act
            var result = builder.ConfigureKey(configuration);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void ConfigureDiscovey_WithCustomEntries_ShouldConfigure()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = services.AddIdentityServer();

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["CustomEntriesOfString:key1"] = "value1",
                    ["CustomEntriesOfBool:key2"] = "true"
                })
                .Build();

            // Act
            var result = builder.ConfigureDiscovey(configuration);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void ConfigureKey_WithRedisKey_ShouldConfigure()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = services.AddIdentityServer();

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["Type"] = "KeysRotation",
                    ["StorageKind"] = "Redis",
                    ["StorageConnectionString"] = "localhost:6379",
                    ["RedisKey"] = "CustomKeyName"
                })
                .Build();

            // Act
            var result = builder.ConfigureKey(configuration);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void ConfigureKey_WithRsaEncryptorConfiguration_ShouldConfigure()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = services.AddIdentityServer();

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["Type"] = "KeysRotation",
                    ["StorageKind"] = "EntityFramework",
                    ["RsaEncryptorConfiguration:SigningAlgorithm"] = "RS512",
                    ["RsaEncryptorConfiguration:EncryptionAlgorithmKeySize"] = "4096"
                })
                .Build();

            // Act
            var result = builder.ConfigureKey(configuration);

            // Assert
            Assert.NotNull(result);
        }
    }
}