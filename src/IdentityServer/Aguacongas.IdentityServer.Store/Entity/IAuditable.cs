using System;
using System.Collections.Generic;
using System.Text;

namespace Aguacongas.IdentityServer.Store.Entity
{
    public interface IAuditable
    {
        DateTime CreatedAt { get; set; }
        DateTime ModifiedAt { get; set; }

        string CreateBy { get; set; }

        string ModifiedBy { get; set; }
    }
}
