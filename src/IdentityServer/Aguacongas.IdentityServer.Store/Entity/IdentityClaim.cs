// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using System;
using System.ComponentModel.DataAnnotations;

namespace Aguacongas.IdentityServer.Store.Entity
{
    /// <summary>
    /// Define a claim returned by an identity resource
    /// </summary>
    /// <seealso cref="IAuditable" />
    /// <seealso cref="IClaimType" />
    public class IdentityClaim : IAuditable, IClaimType, IIdentitySubEntity
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the identity identifier.
        /// </summary>
        /// <value>
        /// The client.
        /// </value>
        [Required]
        public string IdentityId { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        [MaxLength(250)]
        public string Type { get; set; }

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

        /// <summary>
        /// Gets or sets the identity resource.
        /// </summary>
        /// <value>
        /// The client.
        /// </value>
        public virtual IdentityResource Identity { get; set; }
    }
}
