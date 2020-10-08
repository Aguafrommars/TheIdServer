using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aguacongas.IdentityServer.Admin.Extensions
{
    /// <summary>
    /// <see cref="IKey"/> extensions
    /// </summary>
    public static class KeyExtensions
    {
        /// <summary>
        /// Converts to keycollection.
        /// </summary>
        /// <param name="keys">The keys.</param>
        /// <param name="defaultKeyResolver">The default key resolver.</param>
        /// <returns></returns>
        public static PageResponse<Key> ToPageResponse(this IEnumerable<IKey> keys, IDefaultKeyResolver defaultKeyResolver)
        {
            var defaultKeyId = defaultKeyResolver.ResolveDefaultKeyPolicy(DateTimeOffset.UtcNow, keys).DefaultKey?.KeyId;
            return new PageResponse<Key>
            {
                Count = keys.Count(),
                Items = keys.OrderBy(k => k.CreationDate)
                    .Select(k => new Key
                    {
                        ActivationDate = k.ActivationDate,
                        CreationDate = k.CreationDate,
                        ExpirationDate = k.ExpirationDate,
                        Id = k.KeyId.ToString(),
                        IsRevoked = k.IsRevoked,
                        IsDefault = k.KeyId == defaultKeyId
                    })
            };
        }
    }
}
