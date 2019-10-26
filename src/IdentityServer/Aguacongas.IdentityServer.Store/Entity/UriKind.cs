using System;
using System.Collections.Generic;
using System.Text;

namespace Aguacongas.IdentityServer.Store.Entity
{
    /// <summary>
    /// Define uri kind
    /// </summary>
    [Flags]
    public enum UriKind
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
        Cors = 4
    }
}
