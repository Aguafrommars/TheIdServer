// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using IdentityServer4.WsFederation;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Aguacongas.IdentityServer.WsFederation.Stores
{

    /// <summary>
    /// 
    /// </summary>
    public class RelyingParty
    {
        /// <summary>
        /// Gets or sets the realm.
        /// </summary>
        /// <value>
        /// The realm.
        /// </value>
        public string Realm { get; set; }

        /// <summary>
        /// Gets or sets the type of the token.
        /// </summary>
        /// <value>
        /// The type of the token.
        /// </value>
        public string TokenType { get; set; } = WsFederationConstants.TokenTypes.Saml11TokenProfile11;
        /// <summary>
        /// Gets or sets the digest algorithm.
        /// </summary>
        /// <value>
        /// The digest algorithm.
        /// </value>
        public string DigestAlgorithm { get; set; } = SecurityAlgorithms.Sha256Digest;
        /// <summary>
        /// Gets or sets the signature algorithm.
        /// </summary>
        /// <value>
        /// The signature algorithm.
        /// </value>
        public string SignatureAlgorithm { get; set; } = SecurityAlgorithms.RsaSha256Signature;
        /// <summary>
        /// Gets or sets the saml name identifier format.
        /// </summary>
        /// <value>
        /// The saml name identifier format.
        /// </value>
        public string SamlNameIdentifierFormat { get; set; } = WsFederationConstants.SamlNameIdentifierFormats.UnspecifiedString;
        /// <summary>
        /// Gets or sets the encryption certificate.
        /// </summary>
        /// <value>
        /// The encryption certificate.
        /// </value>
        public X509Certificate2 EncryptionCertificate { get; set; }

        /// <summary>
        /// Gets or sets the claim mapping.
        /// </summary>
        /// <value>
        /// The claim mapping.
        /// </value>
        public IDictionary<string, string> ClaimMapping { get; set; } = new Dictionary<string, string>();
    }
}
