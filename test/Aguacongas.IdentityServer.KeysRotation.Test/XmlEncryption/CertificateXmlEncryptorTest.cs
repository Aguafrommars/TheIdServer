using Aguacongas.IdentityServer.KeysRotation.XmlEncryption;
using System;
using Xunit;

namespace Aguacongas.IdentityServer.KeysRotation.Test.XmlEncryption
{
    public class CertificateXmlEncryptorTest
    {
        [Fact]
        public void Constructor_should_throw_on_args_null()
        {
            Assert.Throws<ArgumentNullException>(() => new CertificateXmlEncryptor(null, null));
            Assert.Throws<ArgumentNullException>(() => new CertificateXmlEncryptor(null, null, null));
            Assert.Throws<ArgumentNullException>(() => new CertificateXmlEncryptor("test", null, null));
        }
    }
}
