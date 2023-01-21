// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre

namespace Aguacongas.IdentityServer.WsFederation
{
    /// <summary>
    /// Defines a claim type
    /// </summary>
    public class ClaimType
    {
        /// <summary>
        /// Gets or sets the claim type
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// Gets or sets the display name
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets  or sets the optional flag
        /// </summary>
        public bool Optional { get; set; } = true;
    }
}