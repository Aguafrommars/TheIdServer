// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
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
    /// Implements <see cref="IAuthenticatedEncryptorDescriptor"/> for <see cref="RsaSecurityKey"/>
    /// </summary>
    /// <seealso cref="IAuthenticatedEncryptorDescriptor" />
    public sealed class RsaEncryptorDescriptor : IAuthenticatedEncryptorDescriptor
    {
        public RsaEncryptorDescriptor(RsaEncryptorConfiguration configuration, RsaSecurityKey masterKey)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            RsaSecurityKey = masterKey ?? throw new ArgumentNullException(nameof(masterKey));
        }

        public RsaEncryptorDescriptor(RsaEncryptorConfiguration configuration)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            RsaSecurityKey = GenerateNewKey();
        }

        private RsaSecurityKey GenerateNewKey()
        {
            var factory = GetAsymmetricBlockCipherAlgorithmFactory(Configuration);
            var algorythm = factory();
            algorythm.KeySize = Configuration.EncryptionAlgorithmKeySize;
            var key = new RsaSecurityKey(algorythm);
            int length = Configuration.KeyIdSize / 8;
            key.KeyId = CryptoRandom.CreateUniqueId(length, CryptoRandom.OutputFormat.Hex);

            return key;
        }

        public RsaSecurityKey RsaSecurityKey { get; }

        internal RsaEncryptorConfiguration Configuration { get; }

        public XmlSerializedDescriptorInfo ExportToXml()
        {
            // <descriptor>
            //   <!-- managed implementations -->
            //   <keyId>...</keyId>
            //   <key>...</key>
            // </descriptor>

            RSAParameters parameters;
            if (RsaSecurityKey.Rsa != null)
            {
                parameters = RsaSecurityKey.Rsa.ExportParameters(true);
            }
            else
            {
                parameters = RsaSecurityKey.Parameters;
            }

            var keyIdElement = new XElement("keyId", RsaSecurityKey.KeyId);

            var encryptionElement = new XElement("encryption",
                new XAttribute("algorithm", Configuration.EncryptionAlgorithmType.AssemblyQualifiedName),
                new XAttribute("keyLength", Configuration.EncryptionAlgorithmKeySize));

            var secret = new Secret(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(parameters)));

            var rootElement = new XElement("descriptor",
                new XComment(" Algorithms provided by specified AsymmetricAlgorithm "),
                encryptionElement,
                keyIdElement,
                secret.ToKeyElement());

            return new XmlSerializedDescriptorInfo(rootElement, typeof(RsaEncryptorDescriptorDeserializer));
        }

        private static Func<RSA> GetAsymmetricBlockCipherAlgorithmFactory(RsaEncryptorConfiguration configuration)
        {
            // basic argument checking
            if (configuration.EncryptionAlgorithmType == typeof(RSA))
            {
                return RSA.Create;
            }
            else
            {
                return AlgorithmActivator.CreateFactory<RSA>(configuration.EncryptionAlgorithmType);
            }
        }

        /// <summary>
        /// Contains helper methods for generating cryptographic algorithm factories.
        /// </summary>
        private static class AlgorithmActivator
        {
            /// <summary>
            /// Creates a factory that wraps a call to <see cref="Activator.CreateInstance{T}"/>.
            /// </summary>
            public static Func<T> CreateFactory<T>(Type implementation)
            {
                return ((IActivator<T>)Activator.CreateInstance(typeof(AlgorithmActivatorCore<>).MakeGenericType(implementation))).Creator;
            }

            private interface IActivator<out T>
            {
                Func<T> Creator { get; }
            }

            private sealed class AlgorithmActivatorCore<T> : IActivator<T> where T : new()
            {
                public Func<T> Creator { get; } = Activator.CreateInstance<T>;
            }
        }

    }
}
