// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using System;
using System.ComponentModel.DataAnnotations;

namespace Aguacongas.IdentityServer.Store.Entity
{
    /// <summary>
    /// Configures an external claim transformation
    /// </summary>
    /// <seealso cref="IAuditable" />
    public class ExternalClaimTransformation : IAuditable
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the scheme.
        /// </summary>
        /// <value>
        /// The scheme.
        /// </value>
        [Required]
        public string Scheme { get; set; }

        /// <summary>
        /// Gets or sets the type of from claim.
        /// </summary>
        /// <value>
        /// The type of from claim.
        /// </value>
        [Required]
        public string FromClaimType { get; set; }

        /// <summary>
        /// Converts to claimtype.
        /// </summary>
        /// <value>
        /// The type of to claim.
        /// </value>
        [Required]
        public string ToClaimType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [as multiple values].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [as multiple values]; otherwise, <c>false</c>.
        /// </value>
        public bool AsMultipleValues { get; set; }

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
