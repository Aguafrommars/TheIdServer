using System;
using System.Collections.Generic;
using System.Text;

namespace Aguacongas.IdentityServer.Store.Entitiy
{
    public class DeviceCodes<TKey> : IAuditable where TKey : IEquatable<TKey>
    {
        public TKey Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string CreateBy { get; set; }
        public string ModifiedBy { get; set; }

    }
}
