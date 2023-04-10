// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.Security.Cryptography;
using Xunit;

namespace Aguacongas.IdentityServer.KeysRotation.Test
{
    public class RsaEncryptorFactoryTest
    {
        [Fact]
        public void CreateEncryptorInstance_should_return_null_if_key_descriptor_not_RsaEncryptorDescriptor()
        {
            var sut = new RsaEncryptorFactory(NullLoggerFactory.Instance);

            var decriptiorMock = new Mock<IAuthenticatedEncryptorDescriptor>();
            var keyMock = new Mock<IKey>();
            keyMock.SetupGet(m => m.Descriptor).Returns(decriptiorMock.Object);
            Assert.Null(sut.CreateEncryptorInstance(keyMock.Object));
        }

        [Fact]
        public void CreateAuthenticatedEncryptorInstance_should_return_null_if_configuration_is_null()
        {
            var sut = new RsaEncryptorFactory(NullLoggerFactory.Instance);

            var key = new RsaSecurityKey(RSA.Create());
            Assert.Null(sut.CreateAuthenticatedEncryptorInstance(key, null));
        }
    }
}
