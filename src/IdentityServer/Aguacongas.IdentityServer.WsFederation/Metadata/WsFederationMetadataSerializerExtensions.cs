// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using System;
using System.Linq;
using System.Xml;
using Microsoft.IdentityModel.Protocols.WsFederation;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Xml;
using static Microsoft.IdentityModel.Protocols.WsFederation.WsFederationConstants;

namespace Aguacongas.IdentityServer.WsFederation
{
    /// <summary>
    /// 
    /// </summary>
    public static class WsFederationMetadataSerializerExtensions
    {

        /// <summary>
        /// Writes the metadata.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        /// <param name="writer">The writer.</param>
        /// <param name="configuration">The configuration.</param>
        /// <exception cref="ArgumentNullException">
        /// writer
        /// or
        /// configuration
        /// </exception>
        public static void WriteMetadata(this WsFederationMetadataSerializer serializer, XmlWriter writer, WsFederationConfiguration configuration)
        {
            writer = writer ?? throw new ArgumentNullException(nameof(writer));

            configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            if (string.IsNullOrEmpty(configuration.Issuer))
            {
                throw XmlUtil.LogWriteException($"{nameof(configuration.Issuer)} is null or empty");
            }
                
            if (string.IsNullOrEmpty(configuration.TokenEndpoint))
            {
                throw XmlUtil.LogWriteException($"{nameof(configuration.TokenEndpoint)} is null or empty");
            }
                
            var entityDescriptorId = $"_{Guid.NewGuid()}";
            if (configuration.SigningKeys.FirstOrDefault() is X509SecurityKey)
            {
                EnvelopedSignatureWriter envelopeWriter = new(
                    writer,
                    configuration.SigningCredentials,
                    $"#{entityDescriptorId}");
                writer = envelopeWriter;
            }

            writer.WriteStartDocument();

            // <EntityDescriptor>
            writer.WriteStartElement(Elements.EntityDescriptor, MetadataNamespace);
            // @entityID
            writer.WriteAttributeString(Attributes.EntityId, configuration.Issuer);
            // @ID
            writer.WriteAttributeString(Attributes.Id, entityDescriptorId);

            WriteSecurityTokenServiceTypeRoleDescriptor(configuration, writer);

            // </EntityDescriptor>
            writer.WriteEndElement();

            writer.WriteEndDocument();
        }

        private static void WriteSecurityTokenServiceTypeRoleDescriptor(WsFederationConfiguration configuration, XmlWriter writer)
        {
            // <RoleDescriptorr>
            writer.WriteStartElement(Elements.RoleDescriptor);
            writer.WriteAttributeString(IdentityServer4.WsFederation.WsFederationConstants.Xmlns, IdentityServer4.WsFederation.WsFederationConstants.Prefixes.Xsi, null, XmlSignatureConstants.XmlSchemaNamespace);
            writer.WriteAttributeString(IdentityServer4.WsFederation.WsFederationConstants.Xmlns, PreferredPrefix, null, Namespace);
            writer.WriteAttributeString(IdentityServer4.WsFederation.WsFederationConstants.Attributes.ProtocolSupportEnumeration, Namespace);
            writer.WriteStartAttribute(Attributes.Type, XmlSignatureConstants.XmlSchemaNamespace);
            writer.WriteQualifiedName(Types.SecurityTokenServiceType, Namespace);
            writer.WriteEndAttribute();

            WriteKeyDescriptorForSigning(configuration, writer);

            var supportedTokenTypeUris = new[] {
                "http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV1.1",
                "http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV2.0"
            };

            writer.WriteStartElement(IdentityServer4.WsFederation.WsFederationConstants.Attributes.TokenTypesOffered, Namespace);
            foreach (string tokenTypeUri in supportedTokenTypeUris)
            {
                // <TokenType>
                writer.WriteStartElement(IdentityServer4.WsFederation.WsFederationConstants.Attributes.TokenType, Namespace);
                writer.WriteAttributeString(IdentityServer4.WsFederation.WsFederationConstants.Attributes.Uri, tokenTypeUri);
                // </TokenType>
                writer.WriteEndElement();
            }
            // </TokenTypesOffered>
            writer.WriteEndElement();

            WriteSecurityTokenEndpoint(configuration, writer);
            WritePassiveRequestorEndpoint(configuration, writer);

            // </RoleDescriptorr>
            writer.WriteEndElement();
        }

        private static void WriteSecurityTokenEndpoint(WsFederationConfiguration configuration, XmlWriter writer)
        {
            // <SecurityTokenServiceEndpoint>
            writer.WriteStartElement(IdentityServer4.WsFederation.WsFederationConstants.Elements.SecurityTokenServiceEndpoint, Namespace);

            // <EndpointReference>
            writer.WriteStartElement(WsAddressing.PreferredPrefix, WsAddressing.Elements.EndpointReference, WsAddressing.Namespace);  // EndpointReference

            // <Address>
            writer.WriteStartElement(WsAddressing.Elements.Address, WsAddressing.Namespace);
            writer.WriteString(configuration.TokenEndpoint);
            // </Address>
            writer.WriteEndElement();

            // </EndpointReference>
            writer.WriteEndElement();

            // </SecurityTokenServiceEndpoint>
            writer.WriteEndElement();
        }

        private static void WritePassiveRequestorEndpoint(WsFederationConfiguration configuration, XmlWriter writer)
        {
            // <PassiveRequestorEndpoint>
            writer.WriteStartElement(IdentityServer4.WsFederation.WsFederationConstants.Elements.PassiveRequestorEndpoint, Namespace);

            // <EndpointReference>
            writer.WriteStartElement(WsAddressing.PreferredPrefix, WsAddressing.Elements.EndpointReference, WsAddressing.Namespace);

            // <Address>
            writer.WriteStartElement(WsAddressing.Elements.Address, WsAddressing.Namespace);
            writer.WriteString(configuration.TokenEndpoint);
            // </Address>
            writer.WriteEndElement();

            // </EndpointReference>
            writer.WriteEndElement();

            // </PassiveRequestorEndpoint>
            writer.WriteEndElement();
        }

        private static void WriteKeyDescriptorForSigning(WsFederationConfiguration configuration, XmlWriter writer)
        {
            // <KeyDescriptor>
            writer.WriteStartElement(Elements.KeyDescriptor, MetadataNamespace);
            writer.WriteAttributeString(Attributes.Use, KeyUse.Signing);

            var dsigSerializer = new DSigSerializer();
            foreach (var keyInfo in configuration.KeyInfos)
            {
                dsigSerializer.WriteKeyInfo(writer, keyInfo);
            }

            // </KeyDescriptor>
            writer.WriteEndElement();
        }
    }
}
