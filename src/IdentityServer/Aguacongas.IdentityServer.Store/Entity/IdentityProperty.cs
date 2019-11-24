using System;
using System.ComponentModel.DataAnnotations;

namespace Aguacongas.IdentityServer.Store.Entity
{
    /// <summary>
    /// Define an identity resource property
    /// </summary>
    /// <seealso cref="IAuditable" />
    public class IdentityProperty : IAuditable, IIdentitySubEntity
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
        /// Gets or sets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        [MaxLength(250)]
        [Required]
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        [MaxLength(2000)]
        public string Value { get; set; }

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
