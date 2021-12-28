using System.Collections.Generic;

namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    /// <summary>
    /// Options class to configure discovery endpoint
    /// </summary>
    public class DiscoveryOptions
    {
        /// <summary>
        /// Show endpoints
        /// </summary>
        public bool ShowEndpoints { get; set; }

        /// <summary>
        /// Show signing keys
        /// </summary>
        public bool ShowKeySet { get; set; }

        /// <summary>
        /// Show identity scopes
        /// </summary>
        public bool ShowIdentityScopes { get; set; }

        /// <summary>
        /// Show API scopes
        /// </summary>
        public bool ShowApiScopes { get; set; }

        /// <summary>
        /// Show identity claims
        /// </summary>
        public bool ShowClaims { get; set; }

        /// <summary>
        /// Show response types
        /// </summary>
        public bool ShowResponseTypes { get; set; }

        /// <summary>
        /// Show response modes
        /// </summary>
        public bool ShowResponseModes { get; set; }

        /// <summary>
        /// Show standard grant types
        /// </summary>
        public bool ShowGrantTypes { get; set; }

        /// <summary>
        /// Show custom grant types
        /// </summary>
        public bool ShowExtensionGrantTypes { get; set; }

        /// <summary>
        /// Show token endpoint authentication methods
        /// </summary>
        public bool ShowTokenEndpointAuthenticationMethods { get; set; }

        /// <summary>
        /// Turns relative paths that start with ~/ into absolute paths
        /// </summary>
        public bool ExpandRelativePathsInCustomEntries { get; set; }

        /// <summary>
        /// Sets the maxage value of the cache control header (in seconds) of the HTTP response. This gives clients a hint how often they should refresh their cached copy of the discovery document. If set to 0 no-cache headers will be set. Defaults to null, which does not set the header.
        /// </summary>
        /// <value>
        /// The cache interval in seconds.
        /// </value>
        public int? ResponseCacheInterval { get; set; }

        /// <summary>
        /// Adds custom entries to the discovery document
        /// </summary>
        public IDictionary<string, object> CustomEntries { get; set; }
    }
}