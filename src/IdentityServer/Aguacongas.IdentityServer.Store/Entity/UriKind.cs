using System;
using System.Collections.Generic;
using System.Text;

namespace Aguacongas.IdentityServer.Store.Entity
{
    [Flags]
    public enum UriKind
    {
        Redirect = 1,
        PostLogout = 2,
        Cors = 4
    }
}
