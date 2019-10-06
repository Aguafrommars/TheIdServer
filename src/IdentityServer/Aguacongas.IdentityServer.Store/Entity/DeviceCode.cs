using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Aguacongas.IdentityServer.Store.Entity
{
    public class DeviceCode<TKey> : IAuditable where TKey : IEquatable<TKey>
    {
        public TKey Id { get; set; }

        public TKey ClientId { get; set; }

        [MaxLength(200)]
        public string Code { get; set; }
        [MaxLength(200)]
        public string SubjectId { get; set; }

        public DateTime Expiration { get; set; }

        public string Data { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string CreateBy { get; set; }
        public string ModifiedBy { get; set; }

    }
}
