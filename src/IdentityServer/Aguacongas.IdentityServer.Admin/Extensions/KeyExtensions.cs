// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.IdentityServer.KeysRotation;
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
        public static PageResponse<Key> ToPageResponse(this IEnumerable<Tuple<IKey, string>> keys, IDefaultKeyResolver defaultKeyResolver)
        {
            var defaultKeyId = defaultKeyResolver.ResolveDefaultKeyPolicy(DateTimeOffset.UtcNow, keys.Select(t => t.Item1)).DefaultKey?.KeyId;
            return new PageResponse<Key>
            {
                Count = keys.Count(),
                Items = keys.OrderBy(t => t.Item1.CreationDate)
                    .Select(t => new Key
                    {
                        ActivationDate = t.Item1.ActivationDate,
                        CreationDate = t.Item1.CreationDate,
                        ExpirationDate = t.Item1.ExpirationDate,
                        Id = t.Item1.KeyId.ToString(),
                        IsRevoked = t.Item1.IsRevoked,
                        IsDefault = t.Item1.KeyId == defaultKeyId,
                        Kind = t.Item2
                    })
            };
        }
    }
}
