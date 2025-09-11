// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace Aguacongas.IdentityServer.KeysRotation
{
    public sealed class RsaEncryptorDescriptorDeserializer : IAuthenticatedEncryptorDescriptorDeserializer
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

            var configuration = new RsaEncryptorConfiguration();

            var encryptionElement = element.Element("encryption");
            configuration.EncryptionAlgorithmType = FriendlyNameToType((string)encryptionElement.Attribute("algorithm"));
            configuration.EncryptionAlgorithmKeySize = (int)encryptionElement.Attribute("keyLength");
            configuration.SigningAlgorithm = (string)encryptionElement.Attribute("signingAlrotithm");

            var masterKey = ((string)element.Element("key")).ToSecret();

            byte[] unprotectedSecretRawBytes = new byte[masterKey.Length];
            var segment = new ArraySegment<byte>(unprotectedSecretRawBytes);
            masterKey.WriteSecretIntoBuffer(segment);
            var parameters = JsonConvert.DeserializeObject<RSAParameters>(Encoding.UTF8.GetString(segment.Array));
            var key = new RsaSecurityKey(parameters)
            {
                KeyId = (string)element.Element("keyId")
            };
            return new RsaEncryptorDescriptor(configuration, key);
        }

        // Any changes to this method should also be be reflected
        // in ManagedAuthenticatedEncryptorDescriptor.TypeToFriendlyName.
        private static Type FriendlyNameToType(string typeName)
        {
            return Type.GetType(typeName, throwOnError: true);
        }
    }
}
