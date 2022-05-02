using System.Collections.Generic;

namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    /// <summary>
    /// WS-Federation options
    /// </summary>
    public class WsFederationConfiguration
    {
        /// <summary>
        /// Gets or sets the collection of claim type requested
        /// </summary>
        public IEnumerable<ClaimType> ClaimTypesRequested { get; set; }
        /// <summary>
        /// Gets or sets the collection of claim type offered
        /// </summary>
        public IEnumerable<ClaimType> ClaimTypesOffered { get; set; }
        /// <summary>
        /// Gets or set the token type offered collection
        /// </summary>
        public IEnumerable<TokenType> TokenTypesOffered { get; set; }
    }
}