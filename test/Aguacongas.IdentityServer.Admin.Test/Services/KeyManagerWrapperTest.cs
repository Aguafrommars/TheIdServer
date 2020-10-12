using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using Moq;
using System;
using Xunit;

namespace Aguacongas.IdentityServer.Admin.Services.Test
{
    public class KeyManagerWrapperTest
    {
        [Fact]
        public void Constructor_should_throw_on_args_null()
        {
            Assert.Throws<ArgumentNullException>(() => new KeyManagerWrapper<IAuthenticatedEncryptorDescriptor>(null, null, null));
            Assert.Throws<ArgumentNullException>(() => new KeyManagerWrapper<IAuthenticatedEncryptorDescriptor>(new Mock<IKeyManager>().Object, null, null));
            Assert.Throws<ArgumentNullException>(() => new KeyManagerWrapper<IAuthenticatedEncryptorDescriptor>(new Mock<IKeyManager>().Object, new Mock<IDefaultKeyResolver>().Object, null));
        }
    }
}
