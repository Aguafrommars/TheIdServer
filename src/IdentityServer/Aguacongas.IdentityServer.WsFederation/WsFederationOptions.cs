// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using System.Collections.Generic;

namespace Aguacongas.IdentityServer.WsFederation
{
    /// <summary>
    /// WS-Federation options
    /// </summary>
    public class WsFederationOptions
    {
        /// <summary>
        /// Gets or sets the collection of claim type requested
        /// </summary>
        public IEnumerable<ClaimType> ClaimTypesRequested { get; set; }
        /// <summary>
        /// Gets or sets the collection of claim type offered
        /// </summary>
        public IEnumerable<ClaimType> ClaimTypesOffered { get; set; }
    }
}
