using System;

namespace Aguacongas.IdentityServer.Store.Entitiy
{
    public class ApiScope<TKey> : IAuditable where TKey : IEquatable<TKey>
    {
        public TKey Id { get; set; }

        public TKey ApiId { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public bool Required { get; set; }

        public bool Emphasize { get; set; }

        public bool ShowInDiscoveryDocument { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string CreateBy { get; set; }
        public string ModifiedBy { get; set; }

    }
}
