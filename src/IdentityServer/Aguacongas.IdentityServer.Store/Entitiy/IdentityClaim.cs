using System;

namespace Aguacongas.IdentityServer.Store.Entitiy
{
    public class IdentityClaim<TKey> : IAuditable where TKey : IEquatable<TKey>
    {
        public TKey Id { get; set; }

        public Identity<TKey> IdentityId { get; set; }
        public ClaimType<TKey> Type { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string CreateBy { get; set; }
        public string ModifiedBy { get; set; }

    }
}
