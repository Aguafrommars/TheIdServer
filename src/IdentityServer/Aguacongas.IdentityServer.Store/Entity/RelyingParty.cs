// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace Aguacongas.IdentityServer.Store.Entity
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="IAuditable" />
    public class RelyingParty : IAuditable
    {
        /// <summary>
        /// Gets or sets the realm.
        /// </summary>
        /// <value>
        /// The realm.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the type of the token.
        /// </summary>
        /// <value>
        /// The type of the token.
        /// </value>
        [Required]
        public string TokenType { get; set; }
        /// <summary>
        /// Gets or sets the digest algorithm.
        /// </summary>
        /// <value>
        /// The digest algorithm.
        /// </value>
        [Required]
        public string DigestAlgorithm { get; set; }
        /// <summary>
        /// Gets or sets the signature algorithm.
        /// </summary>
        /// <value>
        /// The signature algorithm.
        /// </value>
        [Required]
        public string SignatureAlgorithm { get; set; }
        /// <summary>
        /// Gets or sets the saml name identifier format.
        /// </summary>
        /// <value>
        /// The saml name identifier format.
        /// </value>
        public string SamlNameIdentifierFormat { get; set; }
        /// <summary>
        /// Gets or sets the encryption certificate.
        /// </summary>
        /// <value>
        /// The encryption certificate.
        /// </value>
        public byte[] EncryptionCertificate { get; set; }

        /// <summary>
        /// Gets or sets the claim mappings.
        /// </summary>
        /// <value>
        /// The claim mappings.
        /// </value>
        public virtual ICollection<RelyingPartyClaimMapping> ClaimMappings { get; set; }

        /// <summary>
        /// Gets or sets the clients.
        /// </summary>
        /// <value>
        /// The clients.
        /// </value>
        public virtual ICollection<Client> Clients { get; set; }

        /// <summary>
        /// Gets or sets the created at.
        /// </summary>
        /// <value>
        /// The created at.
        /// </value>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the modified at.
        /// </summary>
        /// <value>
        /// The modified at.
        /// </value>
        public DateTime? ModifiedAt { get; set; }
    }
}
