// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Aguacongas.IdentityServer.Store.Entity
{
    /// <summary>
    /// Define a protected resources
    /// </summary>
    /// <seealso cref="IAuditable" />
    public class ProtectResource : IAuditable, ICloneable<ProtectResource>, ILocalizable<ApiLocalizedResource>
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [Required]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ProtectResource"/> is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>
        /// The display name.
        /// </value>
        [MaxLength(200)]
        [Required]
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        [MaxLength(1000)] 
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [non editable].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [non editable]; otherwise, <c>false</c>.
        /// </value>
        public bool NonEditable { get; set; }

        /// <summary>
        /// Indicates if this API resource requires the resource indicator to request it,
        /// and expects access tokens issued to it will only ever contain this API resource
        /// as the audience.
        /// </summary>
        public bool RequireResourceIndicator { get; set; }

        /// <summary>
        /// Gets or sets the secrets.
        /// </summary>
        /// <value>
        /// The secrets.
        /// </value>
        public virtual ICollection<ApiSecret> Secrets { get; set; }

        /// <summary>
        /// Gets or sets the user claims.
        /// </summary>
        /// <value>
        /// The user claims.
        /// </value>
        public virtual ICollection<ApiClaim> ApiClaims { get; set; }

        /// <summary>
        /// Gets or sets the properties.
        /// </summary>
        /// <value>
        /// The properties.
        /// </value>
        public virtual ICollection<ApiProperty> Properties { get; set; }

        /// <summary>
        /// Gets or sets the resources.
        /// </summary>
        /// <value>
        /// The resources.
        /// </value>
        public virtual ICollection<ApiLocalizedResource> Resources { get; set; }

        /// <summary>
        /// Gets or sets the API scopes.
        /// </summary>
        /// <value>
        /// The API scopes.
        /// </value>
        public virtual ICollection<ApiApiScope> ApiScopes { get; set; }

        /// <summary>
        /// Gets or sets the created at.
        /// </summary>
        /// <value>
        /// The created at.
        /// </value>
        public DateTime CreatedAt { get ; set; }

        /// <summary>
        /// Gets or sets the modified at.
        /// </summary>
        /// <value>
        /// The modified at.
        /// </value>
        public DateTime? ModifiedAt { get; set; }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public ProtectResource Clone()
        {
            var clone = MemberwiseClone() as ProtectResource;
            return clone;
        }
    }
}
