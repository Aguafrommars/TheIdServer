using System;
using System.ComponentModel.DataAnnotations;

namespace Aguacongas.IdentityServer.Store.Entity
{
    /// <summary>
    /// Define a claim needed by an API scope
    /// </summary>
    /// <seealso cref="IAuditable" />
    public class ApiScopeClaim : IAuditable, IClaimType
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the API scpope identifier.
        /// </summary>
        /// <value>
        /// The API scpope.
        /// </value>
        [Required]
        public string ApiScpopeId { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        [Required]
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
        /// Gets or sets the API scpope.
        /// </summary>
        /// <value>
        /// The API scpope.
        /// </value>
        public virtual ApiScope ApiScpope { get; set; }

    }
}
