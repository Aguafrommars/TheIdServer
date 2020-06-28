using System;
using System.ComponentModel.DataAnnotations;

namespace Aguacongas.IdentityServer.Store.Entity
{
    /// <summary>
    /// Api/ApiScope relation
    /// </summary>
    /// <seealso cref="IApiSubEntity" />
    /// <seealso cref="IApiScopeSubEntity" />
    /// <seealso cref="IAuditable" />
    public class ApiApiScope: IApiSubEntity, IApiScopeSubEntity, IAuditable
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the API identifier.
        /// </summary>
        /// <value>
        /// The API.
        /// </value>
        [Required]
        public string ApiId { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        [Required]
        public string ApiScopeId { get; set; }

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
        /// Gets or sets the API.
        /// </summary>
        /// <value>
        /// The API.
        /// </value>
        public virtual ProtectResource Api { get; set; }

        /// <summary>
        /// Gets or sets the API scope.
        /// </summary>
        /// <value>
        /// The API.
        /// </value>
        public virtual ApiScope ApiScope { get; set; }
    }
}
