using System;

namespace Aguacongas.IdentityServer.Store.Entitiy
{
    public class ApiSecret<TKey> : IAuditable where TKey : IEquatable<TKey>
    {
        public TKey Id { get; set; }

        public TKey ApiId { get; set; }

        //
        // Summary:
        //     Gets or sets the description.
        public string Description { get; set; }
        //
        // Summary:
        //     Gets or sets the value.
        public string Value { get; set; }
        //
        // Summary:
        //     Gets or sets the expiration.
        public DateTime? Expiration { get; set; }
        //
        // Summary:
        //     Gets or sets the type of the client secret.
        public string Type { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string CreateBy { get; set; }
        public string ModifiedBy { get; set; }

    }
}
