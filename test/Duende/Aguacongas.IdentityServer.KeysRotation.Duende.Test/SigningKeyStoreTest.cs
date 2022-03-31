using Duende.IdentityServer.Models;
using Duende.IdentityServer.Stores;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.IdentityServer.KeysRotation.Duende.Test
{
    public class SigningKeyStoreTest
    {
        [Fact]
        public async Task LoadKeysAsync_should_load_keys_from_key_ring_provider()
        {
            var rsaDescriptor = new RsaEncryptorDescriptor(new RsaEncryptorConfiguration());
            var keyMock = new Mock<IKey>();
            keyMock.SetupGet(m => m.Descriptor).Returns(rsaDescriptor);
            keyMock.SetupGet(m => m.CreationDate).Returns(DateTimeOffset.Now);
            var mockKeyManager = new Mock<IKeyManager>();
            mockKeyManager.Setup(m => m.GetAllKeys()).Returns(new[]
            {
                keyMock.Object
            });

            var mockKeyRingProvider = new Mock<ICacheableKeyRingProvider>();
            mockKeyRingProvider.SetupGet(m => m.KeyManager).Returns(mockKeyManager.Object);
            var services = new ServiceCollection()
                .AddTransient(p => mockKeyRingProvider.Object)
                .AddSigningKeyStore()
                .BuildServiceProvider();

            var sut = services.GetRequiredService<ISigningKeyStore>();
            var result = await  sut.LoadKeysAsync().ConfigureAwait(false);

            Assert.Single(result);
        }

        [Fact]
        public async Task DeleteKeyAsync_should_not_be_implemented()
        {
            var mockKeyRingProvider = new Mock<ICacheableKeyRingProvider>();
            var sut = new SigningKeyStore(mockKeyRingProvider.Object);

            await Assert.ThrowsAsync<NotImplementedException>(() => sut.DeleteKeyAsync("tets")).ConfigureAwait(false);
        }

        [Fact]
        public async Task StoreKeyAsync_should_not_be_implemented()
        {
            var mockKeyRingProvider = new Mock<ICacheableKeyRingProvider>();
            var sut = new SigningKeyStore(mockKeyRingProvider.Object);

            await Assert.ThrowsAsync<NotImplementedException>(() => sut.StoreKeyAsync(new SerializedKey())).ConfigureAwait(false);
        }
    }
}
