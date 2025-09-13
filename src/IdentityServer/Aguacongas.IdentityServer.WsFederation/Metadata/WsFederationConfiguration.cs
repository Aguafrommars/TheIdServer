// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using System.Collections.Generic;

namespace Aguacongas.IdentityServer.WsFederation
{ 
    /// <summary>
    /// Contains WsFederation metadata that can be populated from a XML string.
    /// </summary>
    public class WsFederationConfiguration : Microsoft.IdentityModel.Protocols.WsFederation.WsFederationConfiguration
    {
        /// <summary>
        /// Gets or sets the claim type offered collection
        /// </summary>
        public IEnumerable<ClaimType> ClaimTypesOffered { get; set; }

        /// <summary>
        /// Gets or sets the claim type requested collection
        /// </summary>
        public IEnumerable<ClaimType> ClaimTypesRequested { get; set; }

        /// <summary>
        /// Gets or set the token type offered collection
        /// </summary>
        public IEnumerable<TokenType> TokenTypesOffered { get; set; } = new[]
        {
            new TokenType
            {
                Uri =  "urn:oasis:names:tc:SAML:1.0:assertion"
            },
            new TokenType
            {
                Uri =  "urn:oasis:names:tc:SAML:2.0:assertion"
            }
        };

    }
}
