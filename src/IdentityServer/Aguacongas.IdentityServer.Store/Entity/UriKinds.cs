// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using System;

namespace Aguacongas.IdentityServer.Store.Entity
{
    /// <summary>
    /// Define uri kind
    /// </summary>
    [Flags]
    public enum UriKinds
    {
        /// <summary>
        /// Redirect uri
        /// </summary>
        Redirect = 1,
        /// <summary>
        /// Post logout uri
        /// </summary>
        PostLogout = 2,
        /// <summary>
        /// Cors uri
        /// </summary>
        Cors = 4,
        /// <summary>
        /// Saml2P metadata uri
        /// </summary>
        Saml2Metadata = 16
    }
}
