using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Aguacongas.IdentityServer.Store.Entitiy
{
    public class Api<TKey>: IAuditable where TKey : IEquatable<TKey>
    {
        public TKey Id { get; set; }
        public bool Enabled { get; set; }

        [MaxLength(200)]
        public string Name { get; set; }
        [MaxLength(200)] 
        public string DisplayName { get; set; }
        [MaxLength(1000)] 
        public string Description { get; set; }
        public DateTime? LastAccessed { get; set; }
        public bool NonEditable { get; set; }

        public virtual ICollection<ApiSecret<TKey>> Secrets { get; set; }
        public virtual ICollection<ApiScope<TKey>> Scopes { get; set; }
        public virtual ICollection<ApiClaim<TKey>> UserClaims { get; set; }
        public virtual ICollection<ApiProperty<TKey>> Properties { get; set; }

        public DateTime CreatedAt { get ; set; }
        public DateTime ModifiedAt { get; set; }
        public string CreateBy { get; set; }
        public string ModifiedBy { get; set; }
    }
}
