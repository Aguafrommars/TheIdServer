using System;
using System.Collections.Generic;
using System.Text;

namespace Aguacongas.IdentityServer.Store.Entitiy
{
    public class ApiScopeClaim<TKey> : IAuditable where TKey : IEquatable<TKey>
    {
        public TKey Id { get; set; }

        public TKey ApiId { get; set; }

        public TKey ApiScopeId { get; set; }

        public string Type { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string CreateBy { get; set; }
        public string ModifiedBy { get; set; }

    }
}
