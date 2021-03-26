﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// Modifications copyright (c) 2021 @Olivier Lefebvre

// This file is a copy of https://github.com/dotnet/aspnetcore/blob/v3.1.8/src/DataProtection/DataProtection/src/XmlEncryption/CertificateXmlEncryptor.cs
// with:
// namespace change from original Microsoft.AspNetCore.DataProtection.XmlEncryption
// add internal constructor argument check
// original Error.CertificateXmlEncryptor_CertificateNotFound call replaced
// original CryptoUtil.Fail call removed
// original ILogger extensions calls replaced
using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using System.Xml.Linq;

// namespace change from original Microsoft.AspNetCore.DataProtection.XmlEncryption
namespace Aguacongas.IdentityServer.KeysRotation.XmlEncryption
{
    /// <summary>
    /// An <see cref="IXmlEncryptor"/> that can perform XML encryption by using an X.509 certificate.
    /// </summary>
    public sealed class CertificateXmlEncryptor : IInternalCertificateXmlEncryptor, IXmlEncryptor
    {
        private readonly Func<X509Certificate2> _certFactory;
        private readonly IInternalCertificateXmlEncryptor _encryptor;
        private readonly ILogger _logger;

        /// <summary>
        /// Creates a <see cref="CertificateXmlEncryptor"/> given a certificate's thumbprint, an
        /// <see cref="ICertificateResolver"/> that can be used to resolve the certificate, and
        /// an <see cref="IServiceProvider"/>.
        /// </summary>
        public CertificateXmlEncryptor(string thumbprint, ICertificateResolver certificateResolver, ILoggerFactory loggerFactory)
            : this(loggerFactory, encryptor: null)
        {
            if (thumbprint == null)
            {
                throw new ArgumentNullException(nameof(thumbprint));
            }

            if (certificateResolver == null)
            {
                throw new ArgumentNullException(nameof(certificateResolver));
            }

            _certFactory = CreateCertFactory(thumbprint, certificateResolver);
        }

        /// <summary>
        /// Creates a <see cref="CertificateXmlEncryptor"/> given an <see cref="X509Certificate2"/> instance
        /// and an <see cref="IServiceProvider"/>.
        /// </summary>
        public CertificateXmlEncryptor(X509Certificate2 certificate, ILoggerFactory loggerFactory)
            : this(loggerFactory, encryptor: null)
        {
            if (certificate == null)
            {
                throw new ArgumentNullException(nameof(certificate));
            }

            _certFactory = () => certificate;
        }

        // change to private
        private CertificateXmlEncryptor(ILoggerFactory loggerFactory, IInternalCertificateXmlEncryptor encryptor)
        {
            if (loggerFactory == null) // add internal constructor argument check
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }
            _encryptor = encryptor ?? this;
            _logger = loggerFactory.CreateLogger<CertificateXmlEncryptor>();
        }

        /// <summary>
        /// Encrypts the specified <see cref="XElement"/> with an X.509 certificate.
        /// </summary>
        /// <param name="plaintextElement">The plaintext to encrypt.</param>
        /// <returns>
        /// An <see cref="EncryptedXmlInfo"/> that contains the encrypted value of
        /// <paramref name="plaintextElement"/> along with information about how to
        /// decrypt it.
        /// </returns>
        public EncryptedXmlInfo Encrypt(XElement plaintextElement)
        {
            if (plaintextElement == null)
            {
                throw new ArgumentNullException(nameof(plaintextElement));
            }

            // <EncryptedData Type="http://www.w3.org/2001/04/xmlenc#Element" xmlns="http://www.w3.org/2001/04/xmlenc#">
            //   ...
            // </EncryptedData>

            var encryptedElement = EncryptElement(plaintextElement);
            return new EncryptedXmlInfo(encryptedElement, typeof(EncryptedXmlDecryptor));
        }

        private XElement EncryptElement(XElement plaintextElement)
        {
            // EncryptedXml works with XmlDocument, not XLinq. When we perform the conversion
            // we'll wrap the incoming element in a dummy <root /> element since encrypted XML
            // doesn't handle encrypting the root element all that well.
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(new XElement("root", plaintextElement).CreateReader());
            var elementToEncrypt = (XmlElement)xmlDocument.DocumentElement.FirstChild;

            // Perform the encryption and update the document in-place.
            var encryptedXml = new EncryptedXml(xmlDocument);
            var encryptedData = _encryptor.PerformEncryption(encryptedXml, elementToEncrypt);
            EncryptedXml.ReplaceElement(elementToEncrypt, encryptedData, content: false);

            // Strip the <root /> element back off and convert the XmlDocument to an XElement.
            return XElement.Load(xmlDocument.DocumentElement.FirstChild.CreateNavigator().ReadSubtree());
        }

        private Func<X509Certificate2> CreateCertFactory(string thumbprint, ICertificateResolver resolver)
        {
            return () =>
            {
                try
                {
                    var cert = resolver.ResolveCertificate(thumbprint);
                    if (cert == null)
                    {
                        throw new InvalidOperationException($"A certificate with the thumbprint '{thumbprint}' could not be found."); // original Error.CertificateXmlEncryptor_CertificateNotFound call replaced
                    }
                    return cert;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An exception occurred while trying to resolve certificate with thumbprint '{Thumbprint}'.", thumbprint); // original ILogger extensions calls replaced

                    throw;
                }
            };
        }

        EncryptedData IInternalCertificateXmlEncryptor.PerformEncryption(EncryptedXml encryptedXml, XmlElement elementToEncrypt)
        {
            var cert = _certFactory(); // original CryptoUtil.Fail call removed

            _logger.LogDebug("Encrypting to X.509 certificate with thumbprint '{Thumbprint}'.", cert.Thumbprint); // original ILogger extensions calls replaced

            try
            {
                return encryptedXml.Encrypt(elementToEncrypt, cert);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while encrypting to X.509 certificate with thumbprint '{Thumbprint}'.", cert.Thumbprint); // original ILogger extensions calls replaced
                throw;
            }
        }
    }
}
