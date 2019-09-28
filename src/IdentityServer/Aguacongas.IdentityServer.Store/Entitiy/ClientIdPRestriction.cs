using System;
using System.ComponentModel.DataAnnotations;

namespace Aguacongas.IdentityServer.Store.Entitiy
{
    public class ClientIdPRestriction<TKey> : IAuditable where TKey : IEquatable<TKey>
    {
        public TKey Id { get; set; }
        public TKey ClientId { get; set; }

        [MaxLength(200)]
        public string Provider { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string CreateBy { get; set; }
        public string ModifiedBy { get; set; }

    }
}
