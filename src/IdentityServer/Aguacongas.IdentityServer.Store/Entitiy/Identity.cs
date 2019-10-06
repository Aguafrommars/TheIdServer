using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Aguacongas.IdentityServer.Store.Entitiy
{
    public class Identity : Identity<string>
    {

    }

    public class Identity<TKey> : IAuditable where TKey : IEquatable<TKey>
    {
        public TKey Id { get; set; }

        public bool Enabled { get; set; }

        [MaxLength(200)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string DisplayName { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }
        public bool Required { get; set; }
        public bool Emphasize { get; set; }
        public bool ShowInDiscoveryDocument { get; set; }
        public bool NonEditable { get; set; }

        public virtual ICollection<IdentityClaim<TKey>> UserClaims { get; set; }
        public virtual ICollection<IdentityProperty<TKey>> Properties { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string CreateBy { get; set; }
        public string ModifiedBy { get; set; }

    }
}
