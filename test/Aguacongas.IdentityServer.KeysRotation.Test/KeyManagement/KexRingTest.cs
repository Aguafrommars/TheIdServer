using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.IdentityServer.KeysRotation.Test.KeyManagement
{
    public class KexRingTest
    {
        [Fact]
        public async Task GetSigningCredentialsAsync_should_verify_encryptor_type()
        {
            var sut = new KeyRing(new Mock<IKey>().Object, new[] { new Mock<IKey>().Object }, new RsaEncryptorConfiguration());

            await Assert.ThrowsAsync<InvalidOperationException>(() => sut.GetSigningCredentialsAsync());
        }

        [Fact]
        public void GetAuthenticatedEncryptorByKeyId_should_return_encryptor()
        {
            var sut = new KeyRing(new Mock<IKey>().Object, new[] { new Mock<IKey>().Object }, new RsaEncryptorConfiguration());

            var result = sut.GetAuthenticatedEncryptorByKeyId(Guid.NewGuid(), out bool _);

            Assert.Null(result);
        }

        [Fact]
        public void DefaultAuthenticatedEncryptor_should_return_encryptor()
        {
            var sut = new KeyRing(new Mock<IKey>().Object, new[] { new Mock<IKey>().Object }, new RsaEncryptorConfiguration());

            var result = sut.DefaultAuthenticatedEncryptor;

            Assert.Null(result);
        }
    }
}
