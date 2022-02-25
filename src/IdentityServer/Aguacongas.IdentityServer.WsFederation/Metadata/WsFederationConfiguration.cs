// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
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

    }
}
