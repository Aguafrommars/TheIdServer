using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Aguacongas.IdentityServer.Admin.Models
{
    /// <summary>
    /// Create a personal access token
    /// </summary>
    public class CreatePersonalAccessToken
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is reference token.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is reference token; otherwise, <c>false</c>.
        /// </value>
        public bool IsReferenceToken { get; set; }

        /// <summary>
        /// Gets or sets the lifetime days.
        /// </summary>
        /// <value>
        /// The lifetime days.
        /// </value>
        public int LifetimeDays { get; set; }

        /// <summary>
        /// Gets or sets the apis.
        /// </summary>
        /// <value>
        /// The apis.
        /// </value>
        [Required]
        public IEnumerable<string> Apis { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<string> ClaimTypes { get; set; }

        /// <summary>
        /// Gets or sets the scopes.
        /// </summary>
        /// <value>
        /// The scopes.
        /// </value>
        public IEnumerable<string> Scopes { get; set; }
    }
}
