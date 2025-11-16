// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre

using Aguacongas.IdentityServer.KeysRotation.AzureKeyVault;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
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
                    ["KeyProtectionOptions:X509CertificatePassword"] = "test"
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
                config["StorageConnectionString"] = "https://test.blob.core.windows.net/keys";
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

        [Fact]
        public void ConfigureKey_WithAzureKeyVault_Certificate_ValidThumbprint_ShouldConfigure()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = services.AddIdentityServer();

            // Load a test certificate to get its thumbprint
            using var testCert = X509CertificateLoader.LoadPkcs12FromFile("TestCert1.pfx", "password");
            var thumbprint = testCert.Thumbprint;

            // Install the certificate in the store for the test
            using var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadWrite);
            store.Add(testCert);

            try
            {
                var configuration = new ConfigurationBuilder()
                    .AddInMemoryCollection(new Dictionary<string, string>
                    {
                        ["Type"] = "KeysRotation",
                        ["StorageKind"] = "EntityFramework",
                        ["KeyProtectionOptions:KeyProtectionKind"] = "AzureKeyVault",
                        ["KeyProtectionOptions:AzureKeyVaultKeyId"] = "https://test.vault.azure.net/keys/test-key",
                        ["KeyProtectionOptions:AzureKeyVaultTenantId"] = "tenant-id",
                        ["KeyProtectionOptions:AzureKeyVaultClientId"] = "client-id",
                        ["KeyProtectionOptions:AzureKeyVaultCertificateThumbprint"] = thumbprint
                    })
                    .Build();

                // Act
                var result = builder.ConfigureKey(configuration);

                // Assert
                Assert.NotNull(result);
            }
            finally
            {
                // Cleanup: remove the certificate from the store
                store.Remove(testCert);
                store.Close();
            }
        }

        [Fact]
        public void ConfigureKey_WithAzureKeyVault_Certificate_InvalidThumbprint_ShouldThrow()
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
                    ["KeyProtectionOptions:AzureKeyVaultCertificateThumbprint"] = "NONEXISTENTTHUMBPRINT1234567890ABCDEF"
                })
                .Build();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => builder.ConfigureKey(configuration));
            Assert.Contains("Certificate with thumbprint", exception.Message);
            Assert.Contains("not found", exception.Message);
        }

        [Fact]
        public void ConfigureKey_WithAzureKeyVault_Certificate_SearchInBothStores()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = services.AddIdentityServer();

            // Load a test certificate
            using var testCert = X509CertificateLoader.LoadPkcs12FromFile("TestCert1.pfx", "password");
            var thumbprint = testCert.Thumbprint;

            // Install the certificate in LocalMachine store
            using var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            try
            {
                store.Open(OpenFlags.ReadWrite);
                store.Add(testCert);
            }
            catch
            {
                // If we can't access LocalMachine store (permissions), skip this test
                // or use CurrentUser instead
                return;
            }

            try
            {
                var configuration = new ConfigurationBuilder()
                    .AddInMemoryCollection(new Dictionary<string, string>
                    {
                        ["Type"] = "KeysRotation",
                        ["StorageKind"] = "EntityFramework",
                        ["KeyProtectionOptions:KeyProtectionKind"] = "AzureKeyVault",
                        ["KeyProtectionOptions:AzureKeyVaultKeyId"] = "https://test.vault.azure.net/keys/test-key",
                        ["KeyProtectionOptions:AzureKeyVaultTenantId"] = "tenant-id",
                        ["KeyProtectionOptions:AzureKeyVaultClientId"] = "client-id",
                        ["KeyProtectionOptions:AzureKeyVaultCertificateThumbprint"] = thumbprint
                    })
                    .Build();

                // Act
                var result = builder.ConfigureKey(configuration);

                // Assert
                Assert.NotNull(result);
            }
            finally
            {
                // Cleanup: remove the certificate from the store
                try
                {
                    store.Remove(testCert);
                    store.Close();
                }
                catch
                {
                    // Ignore cleanup errors
                }
            }
        }

        [Fact]
        public void ConfigureKey_WithoutKeyProtectionOptions_ShouldConfigureWithoutProtection()
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
        public void ConfigureKey_WithNullDataProtectionOptions_ShouldNotThrow()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = services.AddIdentityServer();

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["Type"] = "KeysRotation"
                })
                .Build();

            // Act
            var result = builder.ConfigureKey(configuration);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void ConfigureKey_WithX509CertificateThumbprint_ShouldConfigure()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = services.AddIdentityServer();

            // Load and install a test certificate
            using var testCert = X509CertificateLoader.LoadPkcs12FromFile("TestCert1.pfx", "password");
            var thumbprint = testCert.Thumbprint;

            using var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadWrite);
            store.Add(testCert);

            try
            {
                var configuration = new ConfigurationBuilder()
                    .AddInMemoryCollection(new Dictionary<string, string>
                    {
                        ["Type"] = "KeysRotation",
                        ["StorageKind"] = "EntityFramework",
                        ["KeyProtectionOptions:KeyProtectionKind"] = "X509",
                        ["KeyProtectionOptions:X509CertificateThumbprint"] = thumbprint
                    })
                    .Build();

                // Act
                builder.ConfigureKey(configuration);
            }
            finally
            {
                store.Remove(testCert);
                store.Close();
            }

            Assert.True(true);
        }

        [Fact]
        public void ConfigureKey_WithMultipleAdditionalKeyTypes_ShouldConfigure()
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
                    ["AdditionalSigningKeyType:RS512:SigningAlgorithm"] = "RS512",
                    ["AdditionalSigningKeyType:PS256:SigningAlgorithm"] = "PS256",
                    ["AdditionalSigningKeyType:ES256:SigningAlgorithm"] = "ES256",
                    ["AdditionalSigningKeyType:ES384:SigningAlgorithm"] = "ES384",
                    ["AdditionalSigningKeyType:ES512:SigningAlgorithm"] = "ES512"
                })
                .Build();

            // Act
            var result = builder.ConfigureKey(configuration);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void ConfigureDiscovey_WithAllCustomEntryTypes_ShouldConfigure()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = services.AddIdentityServer();

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["CustomEntriesOfString:key1"] = "value1",
                    ["CustomEntriesOfString:key2"] = "value2",
                    ["CustomEntriesOfBool:boolKey1"] = "true",
                    ["CustomEntriesOfBool:boolKey2"] = "false",
                    ["CustomEntriesOfStringArray:arrayKey1:0"] = "item1",
                    ["CustomEntriesOfStringArray:arrayKey1:1"] = "item2"
                })
                .Build();

            // Act
            var result = builder.ConfigureDiscovey(configuration);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void ConfigureKey_WithKeyRotationOptions_ShouldBind()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = services.AddIdentityServer();

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["Type"] = "KeysRotation",
                    ["StorageKind"] = "EntityFramework",
                    ["KeyRotationOptions:AutoGenerateKeys"] = "true",
                    ["KeyRotationOptions:NewKeyLifetime"] = "90.00:00:00"
                })
                .Build();

            // Act
            var result = builder.ConfigureKey(configuration);

            // Assert
            Assert.NotNull(result);
        }
    }
}