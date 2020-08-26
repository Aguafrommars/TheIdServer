// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using System.ComponentModel.DataAnnotations;

namespace Aguacongas.IdentityServer.Store.Entity
{
    /// <summary>
    /// User claim
    /// </summary>
    /// <seealso cref="IEntityId" />
    /// <seealso cref="IUserSubEntity" />
    public class UserClaim : IEntityId, IUserSubEntity
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>
        /// The client.
        /// </value>
        [Required]
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        [Required]
        [MaxLength(250)]
        public string ClaimType { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        [Required]
        public string ClaimValue { get; set; }

        /// <summary>
        /// Gets or sets the issuer.
        /// </summary>
        /// <value>
        /// The issuer.
        /// </value>
        public string Issuer { get; set; }

        /// <summary>
        /// Gets or sets the original value.
        /// </summary>
        /// <value>
        /// The original value.
        /// </value>
        public string OriginalType { get; set; }

        /// <summary>
        /// Gets or sets the user claim.
        /// </summary>
        /// <value>
        /// The user claim.
        /// </value>
        public virtual User User { get; set; }
    }
}
