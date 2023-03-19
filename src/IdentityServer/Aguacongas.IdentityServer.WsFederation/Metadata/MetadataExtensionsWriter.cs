// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Microsoft.IdentityModel.Xml;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Aguacongas.IdentityServer.WsFederation
{
    /// <summary>
    /// Write supported claim list
    /// </summary>
    public class MetadataExtensionsWriter : DelegatingXmlDictionaryWriter
    {
        private static readonly string AUTH_NS = "http://docs.oasis-open.org/wsfed/authorization/200706";
        private readonly WsFederationConfiguration _configuration;
        
        /// <summary>
        /// Initialize a new instance of <see cref="MetadataExtensionsWriter"/>
        /// </summary>
        /// <param name="xmlWriter"></param>
        /// <param name="configuration"></param>
        public MetadataExtensionsWriter(XmlDictionaryWriter xmlWriter, WsFederationConfiguration configuration)
        {
            InnerWriter = xmlWriter;
            _configuration = configuration;
        }

        /// <inheritdoc/>
        public override void WriteStartElement(string prefix, string localName, string @namespace)
        {
            if (localName == "PassiveRequestorEndpoint")
            {
                WriteClaimCollection(_configuration.ClaimTypesOffered, prefix, nameof(WsFederationConfiguration.ClaimTypesOffered), @namespace);
                WriteClaimCollection(_configuration.ClaimTypesRequested, prefix, nameof(WsFederationConfiguration.ClaimTypesRequested), @namespace);
                if (_configuration.TokenTypesOffered is not null)
                {
                    InnerWriter.WriteStartElement(prefix, nameof(WsFederationConfiguration.TokenTypesOffered), @namespace);
                    foreach (var tokenType in _configuration.TokenTypesOffered)
                    {
                        InnerWriter.WriteStartElement(prefix, nameof(TokenType), @namespace);
                        InnerWriter.WriteAttributeString("Uri", tokenType.Uri);
                        InnerWriter.WriteEndElement();
                    }
                    InnerWriter.WriteEndElement();
                }
            }

            base.WriteStartElement(prefix, localName, @namespace);
        }

        private void WriteClaimCollection(IEnumerable<ClaimType> collection, string prefix, string name, string @namespace)
        {
            if (collection is null)
            {
                return;
            }

            InnerWriter.WriteStartElement(prefix, name, @namespace);
            foreach (var clainType in collection)
            {
                WriteClaimType(clainType);
            }
            InnerWriter.WriteEndElement();
        }

        private void WriteClaimType(ClaimType clainType)
        {
            InnerWriter.WriteStartElement("auth", nameof(ClaimType), AUTH_NS);
            InnerWriter.WriteAttributeString(nameof(ClaimType.Uri), clainType.Uri);
            InnerWriter.WriteAttributeString(nameof(ClaimType.Optional), clainType.Optional.ToString().ToLower());
            if (clainType.DisplayName is not null)
            {
                InnerWriter.WriteElementString(nameof(ClaimType.DisplayName), AUTH_NS, clainType.DisplayName);
            }
            if (clainType.Description is not null)
            {
                InnerWriter.WriteElementString(nameof(ClaimType.Description), AUTH_NS, clainType.Description);
            }
            InnerWriter.WriteEndElement();
        }

    }
}
