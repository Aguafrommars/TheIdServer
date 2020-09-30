// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// This code is a copy of https://github.com/dotnet/aspnetcore/blob/master/src/DataProtection/DataProtection/src/KeyManagement/Internal/CacheableKeyRing.cs
// but adapted for our needs

using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Aguacongas.IdentityServer.KeysRotation
{
    /// <summary>
    /// Wraps both a keyring and its expiration policy.
    /// </summary>
    public sealed class CacheableKeyRing
    {
        private readonly CancellationToken _expirationToken;

        internal CacheableKeyRing(CancellationToken expirationToken,
            DateTimeOffset expirationTime, 
            IKey defaultKey, 
            IEnumerable<IKey> allKeys,
            RsaEncryptorConfiguration configuration)
            : this(expirationToken, expirationTime, keyRing: new KeyRing(defaultKey, allKeys, configuration))
        {
        }

        internal CacheableKeyRing(CancellationToken expirationToken, DateTimeOffset expirationTime, IKeyRing keyRing)
        {
            _expirationToken = expirationToken;
            ExpirationTimeUtc = expirationTime.UtcDateTime;
            KeyRing = keyRing;
        }

        internal DateTime ExpirationTimeUtc { get; }

        internal IKeyRing KeyRing { get; }

        internal static bool IsValid(CacheableKeyRing keyRing, DateTime utcNow)
        {
            return keyRing != null
                && !keyRing._expirationToken.IsCancellationRequested
                && keyRing.ExpirationTimeUtc > utcNow;
        }

        /// <summary>
        /// Returns a new <see cref="CacheableKeyRing"/> which is identical to 'this' but with a
        /// lifetime extended 2 minutes from <paramref name="now"/>. The inner cancellation token
        /// is also disconnected.
        /// </summary>
        internal CacheableKeyRing WithTemporaryExtendedLifetime(DateTimeOffset now)
        {
            var extension = TimeSpan.FromMinutes(2);
            return new CacheableKeyRing(CancellationToken.None, now + extension, KeyRing);
        }
    }
}
