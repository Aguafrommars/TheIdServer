// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using IdentityModel;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace Aguacongas.IdentityServer.KeysRotation
{
    /// <summary>
    /// Implements <see cref="IAuthenticatedEncryptorDescriptor"/> for <see cref="ECDsaSecurityKey"/>
    /// </summary>
    /// <seealso cref="IAuthenticatedEncryptorDescriptor" />
    public sealed class ECDsaEncryptorDescriptor : IAuthenticatedEncryptorDescriptor
    {
        public ECDsaEncryptorDescriptor(ECDsaEncryptorConfiguration configuration, ECDsaSecurityKey masterKey)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            ECDsaSecurityKey = masterKey ?? throw new ArgumentNullException(nameof(masterKey));
        }

        public ECDsaEncryptorDescriptor(ECDsaEncryptorConfiguration configuration)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            ECDsaSecurityKey = GenerateNewKey();
        }

        private ECDsaSecurityKey GenerateNewKey()
        {
            var factory = GetAsymmetricBlockCipherAlgorithmFactory(Configuration);
            var algorythm = factory();
            algorythm.KeySize = Configuration.EncryptionAlgorithmKeySize;
            var key = new ECDsaSecurityKey(algorythm);
            int length = Configuration.KeyIdSize / 8;
            key.KeyId = CryptoRandom.CreateUniqueId(length, CryptoRandom.OutputFormat.Hex);

            return key;
        }

        public ECDsaSecurityKey ECDsaSecurityKey { get; }

        internal ECDsaEncryptorConfiguration Configuration { get; }

        public XmlSerializedDescriptorInfo ExportToXml()
        {
            // <descriptor>
            //   <!-- managed implementations -->
            //   <keyId>...</keyId>
            //   <key>...</key>
            // </descriptor>

            var parameters = ECDsaSecurityKey.ECDsa.ExportParameters(true);
            var keyInfo = new ECDsaKeyInfo
            {
                D = parameters.D,
                Q = parameters.Q,
                Curve = parameters.Curve.Oid.FriendlyName
            };

            var keyIdElement = new XElement("keyId", ECDsaSecurityKey.KeyId);

            var encryptionElement = new XElement("encryption",
                new XAttribute("algorithm", Configuration.EncryptionAlgorithmType.AssemblyQualifiedName),
                new XAttribute("keyLength", Configuration.EncryptionAlgorithmKeySize));

            var secret = new Secret(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(keyInfo)));

            var rootElement = new XElement("descriptor",
                new XComment(" Algorithms provided by specified AsymmetricAlgorithm "),
                encryptionElement,
                keyIdElement,
                secret.ToKeyElement());

            return new XmlSerializedDescriptorInfo(rootElement, typeof(ECDsaEncryptorDescriptorDeserializer));
        }

        private static Func<ECDsa> GetAsymmetricBlockCipherAlgorithmFactory(ECDsaEncryptorConfiguration configuration)
        {
            // basic argument checking
            if (configuration.EncryptionAlgorithmType == typeof(ECDsa))
            {
                return ECDsa.Create;
            }
            else
            {
                return AlgorithmActivator.CreateFactory<ECDsa>(configuration.EncryptionAlgorithmType);
            }
        }
    }
}
