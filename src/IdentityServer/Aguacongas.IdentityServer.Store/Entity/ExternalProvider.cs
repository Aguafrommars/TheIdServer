// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using System;
using System.Collections.Generic;

namespace Aguacongas.IdentityServer.Store.Entity
{
    /// <summary>
    /// SchemeDefinition entity
    /// </summary>
    /// <seealso cref="IEntityId" />
    public class ExternalProvider : IAuditable
    {
        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>
        /// The display name.
        /// </value>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the kind of the provider.
        /// </summary>
        /// <value>
        /// The name of the provider.
        /// </value>
        public string KindName { get; set; }

        /// <summary>
        /// Gets or sets the serialized handler type.
        /// </summary>
        /// <value>
        /// The name of the serialized handler type.
        /// </value>
        public virtual string SerializedHandlerType { get; set; }

        /// <summary>
        /// Gets or sets the serialized options.
        /// </summary>
        /// <value>
        /// The serialized options.
        /// </value>
        public virtual string SerializedOptions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [store claims].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [store claims]; otherwise, <c>false</c>.
        /// </value>
        public bool StoreClaims { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [map default outbound claim type].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [map default outbound claim type]; otherwise, <c>false</c>.
        /// </value>
        public bool MapDefaultOutboundClaimType { get; set; }

        /// <summary>
        /// Gets or sets the claim transformations.
        /// </summary>
        /// <value>
        /// The claim transformations.
        /// </value>
        public virtual ICollection<ExternalClaimTransformation> ClaimTransformations { get; set; }

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
