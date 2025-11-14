// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
// Updated for Azure.Security.KeyVault.Keys SDK

using Aguacongas.IdentityServer.KeysRotation.AzureKeyVault;
using Azure.Security.KeyVault.Keys.Cryptography;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit;

namespace Aguacongas.IdentityServer.KeysRotation.Test.AzureKeyVault
{
    public class AzureKeyVaultXmlEncryptorTest
    {
        [Fact]
        public async Task UsesKeyVaultToEncryptKey()
        {
            // Arrange
            var mockWrapResult = CreateMockWrapResult("key", [0x0F, 0x0E, 0x0D, 0x0C, 0x0B, 0x0A, 0x09, 0x08, 0x07, 0x06, 0x05, 0x04, 0x03, 0x02, 0x01, 0x00]);
            
            var mock = new Mock<IKeyVaultWrappingClient>();
            mock.Setup(client => client.WrapKeyAsync(
                    "key",
                    KeyWrapAlgorithm.RsaOaep,
                    It.IsAny<byte[]>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockWrapResult);

            var encryptor = new AzureKeyVaultXmlEncryptor(mock.Object, "key", new MockNumberGenerator());

            // Act
            var result = encryptor.Encrypt(new XElement("Element"));

            // Assert
            var encryptedElement = result.EncryptedElement;
            var value = encryptedElement.Element("value");

            mock.VerifyAll();
            Assert.NotNull(result);
            Assert.NotNull(value);
            Assert.Equal(typeof(AzureKeyVaultXmlDecryptor), result.DecryptorType);
            
            // Verify structure
            Assert.Equal("VfLYL2prdymawfucH3Goso0zkPbQ4/GKqUsj2TRtLzsBPz7p7cL1SQaY6I29xSlsPQf6IjxHSz4sDJ427GvlLQ==", 
                encryptedElement.Element("value").Value);
            Assert.Equal("AAECAwQFBgcICQoLDA0ODw==", 
                encryptedElement.Element("iv").Value);
            Assert.Equal("Dw4NDAsKCQgHBgUEAwIBAA==", 
                encryptedElement.Element("key").Value);
            Assert.Equal("key", 
                encryptedElement.Element("kid").Value);
        }

        [Fact]
        public async Task UsesKeyVaultToDecryptKey()
        {
            // Arrange
            var mockUnwrapResult = CreateMockUnwrapResult("KeyId", [0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F]);
            
            var mock = new Mock<IKeyVaultWrappingClient>();
            mock.Setup(client => client.UnwrapKeyAsync(
                    "KeyId",
                    KeyWrapAlgorithm.RsaOaep,
                    It.IsAny<byte[]>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockUnwrapResult)
                .Verifiable();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(mock.Object);

            var encryptor = new AzureKeyVaultXmlDecryptor(serviceCollection.BuildServiceProvider());

            // Act
            var result = encryptor.Decrypt(XElement.Parse(
                @"<encryptedKey>
                  <kid>KeyId</kid>
                  <key>Dw4NDAsKCQgHBgUEAwIBAA==</key>
                  <iv>AAECAwQFBgcICQoLDA0ODw==</iv>
                  <value>VfLYL2prdymawfucH3Goso0zkPbQ4/GKqUsj2TRtLzsBPz7p7cL1SQaY6I29xSlsPQf6IjxHSz4sDJ427GvlLQ==</value>
                </encryptedKey>"));

            // Assert
            mock.VerifyAll();
            Assert.NotNull(result);
            Assert.Equal("<Element />", result.ToString());
        }

        [Fact]
        public void EncryptThrowsOnNullElement()
        {
            // Arrange
            var mock = new Mock<IKeyVaultWrappingClient>();
            var encryptor = new AzureKeyVaultXmlEncryptor(mock.Object, "key");

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => encryptor.Encrypt(null));
        }

        [Fact]
        public async Task WrapKeyIsCalledWithCorrectAlgorithm()
        {
            // Arrange
            KeyWrapAlgorithm capturedAlgorithm = default;
            var mockWrapResult = CreateMockWrapResult("test-key", new byte[16]);
            
            var mock = new Mock<IKeyVaultWrappingClient>();
            mock.Setup(client => client.WrapKeyAsync(
                    It.IsAny<string>(),
                    It.IsAny<KeyWrapAlgorithm>(),
                    It.IsAny<byte[]>(),
                    It.IsAny<CancellationToken>()))
                .Callback<string, KeyWrapAlgorithm, byte[], CancellationToken>((keyId, algorithm, data, ct) =>
                {
                    capturedAlgorithm = algorithm;
                })
                .ReturnsAsync(mockWrapResult);

            var encryptor = new AzureKeyVaultXmlEncryptor(mock.Object, "test-key");

            // Act
            encryptor.Encrypt(new XElement("Test"));

            // Assert
            Assert.Equal(KeyWrapAlgorithm.RsaOaep, capturedAlgorithm);
        }

        [Fact]
        public void EncryptedElementContainsExpectedStructure()
        {
            // Arrange
            var mockWrapResult = CreateMockWrapResult("test-key", new byte[16]);
            
            var mock = new Mock<IKeyVaultWrappingClient>();
            mock.Setup(client => client.WrapKeyAsync(
                    It.IsAny<string>(),
                    It.IsAny<KeyWrapAlgorithm>(),
                    It.IsAny<byte[]>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockWrapResult);

            var encryptor = new AzureKeyVaultXmlEncryptor(mock.Object, "test-key");

            // Act
            var result = encryptor.Encrypt(new XElement("Test", "Content"));

            // Assert
            var encryptedElement = result.EncryptedElement;
            
            Assert.NotNull(encryptedElement.Element("kid"));
            Assert.NotNull(encryptedElement.Element("key"));
            Assert.NotNull(encryptedElement.Element("iv"));
            Assert.NotNull(encryptedElement.Element("value"));
            
            // Verify the comment is present
            Assert.Contains(encryptedElement.Nodes().OfType<XComment>(), 
                c => c.Value.Contains("Azure KeyVault"));
        }

        // Helper methods to create mock results using reflection
        private static WrapResult CreateMockWrapResult(string keyId, byte[] encryptedKey)
        {
            // Use reflection to create WrapResult since it's sealed
            var type = typeof(WrapResult);
            var constructor = type.GetConstructors(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .FirstOrDefault();
            
            if (constructor != null)
            {
                // Try to invoke constructor with parameters
                try
                {
                    return (WrapResult)constructor.Invoke([keyId, encryptedKey, KeyWrapAlgorithm.RsaOaep]);
                }
                catch
                {
                    // If that fails, try with different parameter order
                }
            }
            
            // Fallback: create instance and set properties using reflection
            var instance = (WrapResult)RuntimeHelpers.GetUninitializedObject(type);
            
            type.GetProperty("KeyId")?.SetValue(instance, keyId);
            type.GetProperty("EncryptedKey")?.SetValue(instance, encryptedKey);
            type.GetProperty("Algorithm")?.SetValue(instance, KeyWrapAlgorithm.RsaOaep);
            
            return instance;
        }

        private static UnwrapResult CreateMockUnwrapResult(string keyId, byte[] key)
        {
            // Use reflection to create UnwrapResult since it's sealed
            var type = typeof(UnwrapResult);
            var constructor = type.GetConstructors(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .FirstOrDefault();
            
            if (constructor != null)
            {
                // Try to invoke constructor with parameters
                try
                {
                    return (UnwrapResult)constructor.Invoke([keyId, key, KeyWrapAlgorithm.RsaOaep]);
                }
                catch
                {
                    // If that fails, try with different parameter order
                }
            }
            
            // Fallback: create instance and set properties using reflection
            var instance = (UnwrapResult)RuntimeHelpers.GetUninitializedObject(type);
            
            type.GetProperty("KeyId")?.SetValue(instance, keyId);
            type.GetProperty("Key")?.SetValue(instance, key);
            type.GetProperty("Algorithm")?.SetValue(instance, KeyWrapAlgorithm.RsaOaep);
            
            return instance;
        }

        private class MockNumberGenerator : RandomNumberGenerator
        {
            public override void GetBytes(byte[] data)
            {
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = (byte)i;
                }
            }

#if NET5_0_OR_GREATER
            public override void GetBytes(Span<byte> data)
            {
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = (byte)i;
                }
            }
#endif
        }
    }
}