// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
// Updated for Azure.Security.KeyVault.Keys SDK

using Aguacongas.IdentityServer.KeysRotation.AzureKeyVault;
using Azure.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Security.Cryptography.X509Certificates;
using Xunit;

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
            using var cert = new X509Certificate2("TestCert1.pfx", "test");

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

        // Helper class for testing
        private class KeyRotationBuilder : IKeyRotationBuilder
        {
            public IServiceCollection Services { get; set; }
        }
    }
}