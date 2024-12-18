// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Aguacongas.IdentityServer.KeysRotation.XmlEncryption;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;
using Xunit;

namespace Aguacongas.IdentityServer.KeysRotation.Test.KeyManagement
{
    public class EncryptedXmlDecryptorTest
    {
        [Fact]
        public void Decrypt_should_throw_on_argument_null()
        {
            var sut = new EncryptedXmlDecryptor();
            Assert.Throws<ArgumentNullException>(() => sut.Decrypt(null));
        }

        [Fact]
        public void ThrowsIfCannotDecrypt()
        {
            var testCert1 = X509CertificateLoader.LoadPkcs12FromFile("TestCert1.pfx", "password");
            var encryptor = new CertificateXmlEncryptor(testCert1, NullLoggerFactory.Instance);
            var data = new XElement("SampleData", "Lorem ipsum");
            var encryptedXml = encryptor.Encrypt(data);
            var decryptor = new EncryptedXmlDecryptor();

            var ex = Assert.Throws<CryptographicException>(() =>
                decryptor.Decrypt(encryptedXml.EncryptedElement));
            Assert.Equal("Unable to retrieve the decryption key.", ex.Message);
        }

        [Fact]
        public void ThrowsIfProvidedCertificateDoesNotMatch()
        {
            var testCert1 = X509CertificateLoader.LoadPkcs12FromFile("TestCert1.pfx", "password");
            var testCert2 = X509CertificateLoader.LoadPkcs12FromFile("TestCert2.pfx", "password");
            var services = new ServiceCollection()
                .Configure<XmlKeyDecryptionOptions>(o => o.AddKeyDecryptionCertificate(testCert2))
                .BuildServiceProvider();
            var encryptor = new CertificateXmlEncryptor(testCert1, NullLoggerFactory.Instance);
            var data = new XElement("SampleData", "Lorem ipsum");
            var encryptedXml = encryptor.Encrypt(data);
            var decryptor = new EncryptedXmlDecryptor(services);

            var ex = Assert.Throws<CryptographicException>(() =>
                    decryptor.Decrypt(encryptedXml.EncryptedElement));
            Assert.Equal("Unable to retrieve the decryption key.", ex.Message);
        }

        [Fact]
        public void ThrowsIfProvidedCertificateDoesHavePrivateKey()
        {
            var fullCert = X509CertificateLoader.LoadPkcs12FromFile("TestCert1.pfx", "password");
            var publicKeyOnly = X509CertificateLoader.LoadCertificateFromFile("TestCert1.PublicKeyOnly.cer");
            var services = new ServiceCollection()
                .Configure<XmlKeyDecryptionOptions>(o => o.AddKeyDecryptionCertificate(publicKeyOnly))
                .BuildServiceProvider();
            var encryptor = new CertificateXmlEncryptor(fullCert, NullLoggerFactory.Instance);
            var data = new XElement("SampleData", "Lorem ipsum");
            var encryptedXml = encryptor.Encrypt(data);
            var decryptor = new EncryptedXmlDecryptor(services);

            var ex = Assert.Throws<CryptographicException>(() =>
                    decryptor.Decrypt(encryptedXml.EncryptedElement));
            Assert.Equal("Unable to retrieve the decryption key.", ex.Message);
        }

        [Fact]
        public void XmlCanRoundTrip()
        {
            var testCert1 = X509CertificateLoader.LoadPkcs12FromFile("TestCert1.pfx", "password");
            var testCert2 = X509CertificateLoader.LoadPkcs12FromFile("TestCert2.pfx", "password");
            var services = new ServiceCollection()
                .Configure<XmlKeyDecryptionOptions>(o =>
                {
                    o.AddKeyDecryptionCertificate(testCert1);
                    o.AddKeyDecryptionCertificate(testCert2);
                })
                .BuildServiceProvider();
            var encryptor = new CertificateXmlEncryptor(testCert1, NullLoggerFactory.Instance);
            var data = new XElement("SampleData", "Lorem ipsum");
            var encryptedXml = encryptor.Encrypt(data);
            var decryptor = new EncryptedXmlDecryptor(services);

            var decrypted = decryptor.Decrypt(encryptedXml.EncryptedElement);

            Assert.Equal("SampleData", decrypted.Name);
            Assert.Equal("Lorem ipsum", decrypted.Value);
        }
    }
}
