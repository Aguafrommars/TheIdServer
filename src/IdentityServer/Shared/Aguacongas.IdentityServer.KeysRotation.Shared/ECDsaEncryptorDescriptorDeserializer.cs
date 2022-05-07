// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace Aguacongas.IdentityServer.KeysRotation
{
    public sealed class ECDsaEncryptorDescriptorDeserializer : IAuthenticatedEncryptorDescriptorDeserializer
    {
        /// <summary>
        /// Imports the <see cref="ManagedAuthenticatedEncryptorDescriptor"/> from serialized XML.
        /// </summary>
        public IAuthenticatedEncryptorDescriptor ImportFromXml(XElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            // <descriptor>
            //   <!-- managed implementations -->
            //   <encryption algorithm="..." keyLength="..." />
            //   <masterKey>...</masterKey>
            // </descriptor>

            var configuration = new ECDsaEncryptorConfiguration();

            var encryptionElement = element.Element("encryption");
            configuration.EncryptionAlgorithmType = FriendlyNameToType((string)encryptionElement.Attribute("algorithm"));
            configuration.EncryptionAlgorithmKeySize = (int)encryptionElement.Attribute("keyLength");
            configuration.SigningAlgorithm = (string)encryptionElement.Attribute("signingAlrotithm");

            var masterKey = ((string)element.Element("key")).ToSecret();

            byte[] unprotectedSecretRawBytes = new byte[masterKey.Length];
            var segment = new ArraySegment<byte>(unprotectedSecretRawBytes);
            masterKey.WriteSecretIntoBuffer(segment);
            var keyInfo = JsonConvert.DeserializeObject<ECDsaKeyInfo>(Encoding.UTF8.GetString(segment.Array));

            var curve = keyInfo.Curve switch
            {
                "nistP256" => ECCurve.NamedCurves.nistP256,
                "nistP384" => ECCurve.NamedCurves.nistP384,
                "nistP521" => ECCurve.NamedCurves.nistP521,
                _ => throw new InvalidOperationException("Invalid Curve name")
            };

            var parameters = new ECParameters
            {
                Curve = curve,
                D = keyInfo.D,
                Q = keyInfo.Q
            };
            var key = new ECDsaSecurityKey(ECDsa.Create(parameters))
            {
                KeyId = (string)element.Element("keyId")
            };
            return new ECDsaEncryptorDescriptor(configuration, key);
        }

        // Any changes to this method should also be be reflected
        // in ManagedAuthenticatedEncryptorDescriptor.TypeToFriendlyName.
        private static Type FriendlyNameToType(string typeName)
        {
            return Type.GetType(typeName, throwOnError: true);
        }
    }
}
