// Project: Aguafrommars/TheIdServer
// Copyright (c) 2026 @Olivier Lefebvre
using System.Collections.Generic;

namespace Aguacongas.IdentityServer.Admin.Options
{
    /// <summary>
    /// Defines dynamic client registration options.
    /// </summary>
    public class DynamicClientRegistrationOptions
    {
        /// <summary>
        /// Gets or sets the allowed contacts.
        /// </summary>
        /// <value>
        /// The allowed contacts.
        /// </value>
        public IEnumerable<AllowedContact> AllowedContacts { get; set; }

        /// <summary>
        /// When true, only allowed contacts or authenticated user with Is4-Writer role can use the registration endpoint. Default is true.
        /// </summary>
        public bool Protected { get; set; } = true;

        /// <summary>
        /// Gets or sets when PKCE is required for clients using the authorization code flow. Default: Always.
        /// </summary>
        public PkceRequirement RequirePkce { get; set; } = PkceRequirement.Always;

        /// <summary>
        /// Gets or sets a value indicating whether clients must authenticate at the token endpoint.
        /// Only native applications restricted to the authorization code flow are public clients.
        /// Set to false to restore the legacy behavior (all clients are public). Default: true.
        /// </summary>
        public bool RequireClientAuthentication { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether web clients using the authorization code flow must register https redirect URIs. Default: true.
        /// </summary>
        public bool RequireHttpsRedirectUris { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether http loopback redirect URIs (localhost, 127.0.0.1, [::1]) are accepted
        /// when RequireHttpsRedirectUris is true. Default: true.
        /// </summary>
        public bool AllowLoopbackHttpRedirectUris { get; set; } = true;
    }

    /// <summary>
    /// Defines dynamic client registration allowed contact.
    /// </summary>
    public class AllowedContact
    {
        /// <summary>
        /// Gets or sets the contact.
        /// </summary>
        /// <value>
        /// The contact.
        /// </value>
        public string Contact { get; set; }

        /// <summary>
        /// Gets or sets the allowed hosts.
        /// </summary>
        /// <value>
        /// The allowed hosts.
        /// </value>
        public IEnumerable<string> AllowedHosts { get; set; }
    }
}
