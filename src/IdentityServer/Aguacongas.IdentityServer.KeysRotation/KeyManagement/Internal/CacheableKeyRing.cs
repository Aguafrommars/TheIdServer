// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// Modifications copyright (c) 2020 @Olivier Lefebvre

// This file is a copy of https://github.com/dotnet/aspnetcore/blob/v3.1.8/src/DataProtection/DataProtection/src/KeyManagement/Internal/CacheableKeyRing.cs
// with :
// namespace change from original Microsoft.AspNetCore.DataProtection.KeyManagement.Internal
// constructor change to add a RsaEncryptorConfiguration instance
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using System;
using System.Collections.Generic;
using System.Threading;

// namespace change from original Microsoft.AspNetCore.DataProtection.KeyManagement.Internal
namespace Aguacongas.IdentityServer.KeysRotation
{
    /// <summary>
    /// Wraps both a keyring and its expiration policy.
    /// </summary>
    public sealed class CacheableKeyRing
    {
        private readonly CancellationToken _expirationToken;

        internal CacheableKeyRing(
            DateTimeOffset expirationTime, 
            IKey defaultKey, 
            IEnumerable<IKey> allKeys,
            RsaEncryptorConfiguration configuration,
            CancellationToken expirationToken) // constructor change to add a RsaEncryptorConfiguration instance
            : this(expirationTime, keyRing: new KeyRing(defaultKey, allKeys, configuration), expirationToken)
        {
        }

        internal CacheableKeyRing(DateTimeOffset expirationTime, IKeyRing keyRing, CancellationToken expirationToken)
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
            return new CacheableKeyRing(now + extension, KeyRing, CancellationToken.None);
        }
    }
}
