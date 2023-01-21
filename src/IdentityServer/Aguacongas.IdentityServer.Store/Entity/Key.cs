// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using System;

namespace Aguacongas.IdentityServer.Store.Entity
{
    /// <summary>
    /// Represent a key
    /// </summary>
    /// <seealso cref="IEntityId" />
    public class Key : IEntityId
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the activation date.
        /// </summary>
        /// <value>
        /// The activation date.
        /// </value>
        public DateTimeOffset ActivationDate { get; set; }

        /// <summary>
        /// Gets or sets the creation date.
        /// </summary>
        /// <value>
        /// The creation date.
        /// </value>
        public DateTimeOffset CreationDate { get; set; }

        /// <summary>
        /// Gets or sets the expiration date.
        /// </summary>
        /// <value>
        /// The expiration date.
        /// </value>
        public DateTimeOffset ExpirationDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is revoked.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is revoked; otherwise, <c>false</c>.
        /// </value>
        public bool IsRevoked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is default.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is default; otherwise, <c>false</c>.
        /// </value>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Gets or sets the kind.
        /// </summary>
        /// <value>
        /// The kind.
        /// </value>
        public string Kind { get; set; }
    }
}
