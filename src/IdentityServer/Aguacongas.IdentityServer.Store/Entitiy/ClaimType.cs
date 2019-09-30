using System;

namespace Aguacongas.IdentityServer.Store.Entitiy
{
    public class ClaimType<TKey> : IAuditable where TKey : IEquatable<TKey>
    {
        public TKey Id { get; set; }

        public string Type { get; set; }

        public string ValueType { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string CreateBy { get; set; }
        public string ModifiedBy { get; set; }

    }
}
