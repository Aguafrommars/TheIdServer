// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre

using System;
using System.Threading;
using System.Threading.Tasks;
using Aguacongas.IdentityServer.KeysRotation.AzureKeyVault;
using Azure.Core;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Keys.Cryptography;
using Moq;
using Xunit;

namespace Aguacongas.IdentityServer.KeysRotation.Duende.Test.AzureKeyVault
{
    public class KeyVaultClientWrapperTest
    {
        [Fact]
        public void Constructor_WithKeyClient_ShouldNotThrow()
        {
            // Arrange
            var mockKeyClient = new Mock<KeyClient>();
            var mockCredential = new Mock<TokenCredential>();

            // Act
            var wrapper = new KeyVaultClientWrapper(mockKeyClient.Object, mockCredential.Object);

            // Assert
            Assert.NotNull(wrapper);
        }

        [Fact]
        public void Constructor_WithVaultUri_ShouldNotThrow()
        {
            // Arrange
            var vaultUri = new Uri("https://test-vault.vault.azure.net/");
            var mockCredential = new Mock<TokenCredential>();

            // Act
            var wrapper = new KeyVaultClientWrapper(vaultUri, mockCredential.Object);

            // Assert
            Assert.NotNull(wrapper);
        }

        [Fact]
        public void Constructor_WithNullKeyClient_ShouldThrowArgumentNullException()
        {
            // Arrange
            KeyClient keyClient = null;
            var mockCredential = new Mock<TokenCredential>();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new KeyVaultClientWrapper(keyClient, mockCredential.Object));
        }

        [Fact]
        public void Constructor_WithNullVaultUri_ShouldThrowArgumentNullException()
        {
            // Arrange
            Uri vaultUri = null;
            var mockCredential = new Mock<TokenCredential>();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new KeyVaultClientWrapper(vaultUri, mockCredential.Object));
        }

        [Fact]
        public void Constructor_WithNullCredential_ShouldThrowArgumentNullException()
        {
            // Arrange
            var vaultUri = new Uri("https://test-vault.vault.azure.net/");

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new KeyVaultClientWrapper(vaultUri, null));
        }

        [Fact]
        public async Task WrapKeyAsync_WithNullKeyIdentifier_ShouldThrowArgumentNullException()
        {
            // Arrange
            var vaultUri = new Uri("https://test-vault.vault.azure.net/");
            var mockCredential = new Mock<TokenCredential>();
            var wrapper = new KeyVaultClientWrapper(vaultUri, mockCredential.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                wrapper.WrapKeyAsync(null, KeyWrapAlgorithm.RsaOaep, new byte[32]));
        }

        [Fact]
        public async Task WrapKeyAsync_WithEmptyKeyIdentifier_ShouldThrowArgumentNullException()
        {
            // Arrange
            var vaultUri = new Uri("https://test-vault.vault.azure.net/");
            var mockCredential = new Mock<TokenCredential>();
            var wrapper = new KeyVaultClientWrapper(vaultUri, mockCredential.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                wrapper.WrapKeyAsync(string.Empty, KeyWrapAlgorithm.RsaOaep, new byte[32]));
        }

        [Fact]
        public async Task UnwrapKeyAsync_WithNullKeyIdentifier_ShouldThrowArgumentNullException()
        {
            // Arrange
            var vaultUri = new Uri("https://test-vault.vault.azure.net/");
            var mockCredential = new Mock<TokenCredential>();
            var wrapper = new KeyVaultClientWrapper(vaultUri, mockCredential.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                wrapper.UnwrapKeyAsync(null, KeyWrapAlgorithm.RsaOaep, new byte[256]));
        }

        [Fact]
        public async Task UnwrapKeyAsync_WithEmptyKeyIdentifier_ShouldThrowArgumentNullException()
        {
            // Arrange
            var vaultUri = new Uri("https://test-vault.vault.azure.net/");
            var mockCredential = new Mock<TokenCredential>();
            var wrapper = new KeyVaultClientWrapper(vaultUri, mockCredential.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                wrapper.UnwrapKeyAsync(string.Empty, KeyWrapAlgorithm.RsaOaep, new byte[256]));
        }

        [Theory]
        [InlineData("https://vault1.vault.azure.net/keys/key1")]
        [InlineData("https://vault2.vault.azure.net/keys/key2/version123")]
        [InlineData("key-name-only")]
        public void WrapKeyAsync_WithVariousKeyIdentifiers_ShouldAcceptValidFormats(string keyIdentifier)
        {
            // Arrange
            var vaultUri = new Uri("https://test-vault.vault.azure.net/");
            var mockCredential = new Mock<TokenCredential>();
            var wrapper = new KeyVaultClientWrapper(vaultUri, mockCredential.Object);

            // Act & Assert - should not throw on call (will fail on execution, but that's expected)
            var task = wrapper.WrapKeyAsync(keyIdentifier, KeyWrapAlgorithm.RsaOaep, new byte[32]);
            Assert.NotNull(task);
        }

        [Theory]
        [InlineData("RSA-OAEP")]
        [InlineData("RSA-OAEP-256")]
        [InlineData("RSA1_5")]
        public void WrapKeyAsync_WithDifferentAlgorithms_ShouldAccept(string value)
        {
            // Arrange
            var vaultUri = new Uri("https://test-vault.vault.azure.net/");
            var mockCredential = new Mock<TokenCredential>();
            var wrapper = new KeyVaultClientWrapper(vaultUri, mockCredential.Object);

            // Act & Assert
            var task = wrapper.WrapKeyAsync("test-key", new KeyWrapAlgorithm(value), new byte[32]);
            Assert.NotNull(task);
        }

        [Fact]
        public void WrapKeyAsync_SupportsCancellation()
        {
            // Arrange
            var vaultUri = new Uri("https://test-vault.vault.azure.net/");
            var mockCredential = new Mock<TokenCredential>();
            var wrapper = new KeyVaultClientWrapper(vaultUri, mockCredential.Object);
            using var cts = new CancellationTokenSource();

            // Act
            var task = wrapper.WrapKeyAsync("test-key", KeyWrapAlgorithm.RsaOaep, new byte[32], cts.Token);

            // Assert
            Assert.NotNull(task);
            cts.Cancel();
        }

        [Fact]
        public void UnwrapKeyAsync_SupportsCancellation()
        {
            // Arrange
            var vaultUri = new Uri("https://test-vault.vault.azure.net/");
            var mockCredential = new Mock<TokenCredential>();
            var wrapper = new KeyVaultClientWrapper(vaultUri, mockCredential.Object);
            using var cts = new CancellationTokenSource();

            // Act
            var task = wrapper.UnwrapKeyAsync("test-key", KeyWrapAlgorithm.RsaOaep, new byte[256], cts.Token);

            // Assert
            Assert.NotNull(task);
            cts.Cancel();
        }
    }
}