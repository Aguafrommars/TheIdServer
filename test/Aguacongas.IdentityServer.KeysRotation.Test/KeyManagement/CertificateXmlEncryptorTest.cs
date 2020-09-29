using Aguacongas.IdentityServer.Admin.Configuration;
using Aguacongas.IdentityServer.KeysRotation.XmlEncryption;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;
using Xunit;

namespace Aguacongas.IdentityServer.KeysRotation.Test.KeyManagement
{
    public class CertificateXmlEncryptorTest
    {
        [Fact]
        public void Constructor_should_throw_on_arguments_null()
        {
            Assert.Throws<ArgumentNullException>(() => new CertificateXmlEncryptor(null, null));
            Assert.Throws<ArgumentNullException>(() => new CertificateXmlEncryptor(new X509Certificate2(), null));
            Assert.Throws<ArgumentNullException>(() => new CertificateXmlEncryptor(null, null, null));
            Assert.Throws<ArgumentNullException>(() => new CertificateXmlEncryptor("test", null, null));
        }

        [Fact]
        public void Encrypt_should_throw_on_argument_null()
        {
            var sut = new CertificateXmlEncryptor(new X509Certificate2(), NullLoggerFactory.Instance);

            Assert.Throws<ArgumentNullException>(() => sut.Encrypt(null));
        }

        [Fact]
        public void Encrypt_should_throw_on_cert_not_found()
        {
            var resolverMock = new Mock<Microsoft.AspNetCore.DataProtection.XmlEncryption.ICertificateResolver>();
            var sut = new CertificateXmlEncryptor("test", resolverMock.Object, NullLoggerFactory.Instance);

            Assert.Throws<InvalidOperationException>(() => sut.Encrypt(new XElement("test")));
        }

        [Fact]
        public void Encrypt_retreive_cert_from_thumbrint()
        {
            var cert = SigningKeysLoader.LoadFromFile("theidserver.pfx", "YourSecurePassword", X509KeyStorageFlags.Exportable | X509KeyStorageFlags.UserKeySet);
            var resolverMock = new Mock<Microsoft.AspNetCore.DataProtection.XmlEncryption.ICertificateResolver>();
            resolverMock.Setup(m => m.ResolveCertificate(It.IsAny<string>())).Returns(cert);
            var sut = new CertificateXmlEncryptor(cert.Thumbprint, resolverMock.Object, NullLoggerFactory.Instance);

            var result = sut.Encrypt(new XElement("test"));
            Assert.NotNull(result);
        }
    }
}
