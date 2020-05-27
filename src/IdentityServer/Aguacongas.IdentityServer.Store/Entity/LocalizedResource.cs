using System;
using System.ComponentModel.DataAnnotations;

namespace Aguacongas.IdentityServer.Store.Entity
{
    /// <summary>
    /// Defined a localized resource
    /// </summary>
    /// <seealso cref="IAuditable" />
    public class LocalizedResource : IAuditable
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        [Required]
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the name of the base.
        /// </summary>
        /// <value>
        /// The name of the base.
        /// </value>
        public string BaseName { get; set; }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>
        /// The location.
        /// </value>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the culture identifier.
        /// </summary>
        /// <value>
        /// The culture identifier.
        /// </value>
        [Required]
        public string CultureId { get; set; }
 
        /// <summary>
        /// Gets or sets the culture.
        /// </summary>
        /// <value>
        /// The culture.
        /// </value>
        public virtual Culture Culture { get; set; }

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
