﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Threading;

namespace Aguacongas.IdentityServer.KeysRotation
{
    internal sealed class KeyRingProvider : ICacheableKeyRingProvider, IKeyRingProvider
    {
        private CacheableKeyRing _cacheableKeyRing;
        private readonly object _cacheableKeyRingLockObj = new object();
        private readonly IDefaultKeyResolver _defaultKeyResolver;
        private readonly KeyManagementOptions _keyManagementOptions;
        private readonly IKeyManager _keyManager;
        private readonly ILogger _logger;

        public KeyRingProvider(
            IKeyManager keyManager,
            IOptions<KeyManagementOptions> keyManagementOptions,
            IDefaultKeyResolver defaultKeyResolver)
            : this(
                  keyManager,
                  keyManagementOptions,
                  defaultKeyResolver,
                  NullLoggerFactory.Instance)
        {
        }

        public KeyRingProvider(
            IKeyManager keyManager,
            IOptions<KeyManagementOptions> keyManagementOptions,
            IDefaultKeyResolver defaultKeyResolver,
            ILoggerFactory loggerFactory)
        {
            _keyManagementOptions = new KeyManagementOptions(keyManagementOptions.Value); // clone so new instance is immutable
            _keyManager = keyManager;
            CacheableKeyRingProvider = this;
            _defaultKeyResolver = defaultKeyResolver;
            _logger = loggerFactory.CreateLogger<KeyRingProvider>();

            // We will automatically refresh any unknown keys for 2 minutes see https://github.com/aspnet/AspNetCore/issues/3975
            AutoRefreshWindowEnd = DateTime.UtcNow.AddMinutes(2);
        }

        // for testing
        internal ICacheableKeyRingProvider CacheableKeyRingProvider { get; set; }

        internal DateTime AutoRefreshWindowEnd { get; set; }

        internal bool InAutoRefreshWindow() => DateTime.UtcNow < AutoRefreshWindowEnd;

        private CacheableKeyRing CreateCacheableKeyRingCore(DateTimeOffset now, IKey keyJustAdded)
        {
            // Refresh the list of all keys
            var cacheExpirationToken = _keyManager.GetCacheExpirationToken();
            var allKeys = _keyManager.GetAllKeys();

            // Fetch the current default key from the list of all keys
            var defaultKeyPolicy = _defaultKeyResolver.ResolveDefaultKeyPolicy(now, allKeys);
            if (!defaultKeyPolicy.ShouldGenerateNewKey)
            {
                if (defaultKeyPolicy.DefaultKey == null)
                {
                    throw new CryptographicException("Assertion failed: Expected to see a default key.");
                }
                
                return CreateCacheableKeyRingCoreStep2(now, cacheExpirationToken, defaultKeyPolicy.DefaultKey, allKeys);
            }

            _logger.LogDebug("Policy resolution states that a new key should be added to the key ring.");

            // We shouldn't call CreateKey more than once, else we risk stack diving. This code path shouldn't
            // get hit unless there was an ineligible key with an activation date slightly later than the one we
            // just added. If this does happen, then we'll just use whatever key we can instead of creating
            // new keys endlessly, eventually falling back to the one we just added if all else fails.
            if (keyJustAdded != null)
            {
                var keyToUse = defaultKeyPolicy.DefaultKey ?? defaultKeyPolicy.FallbackKey ?? keyJustAdded;
                return CreateCacheableKeyRingCoreStep2(now, cacheExpirationToken, keyToUse, allKeys);
            }

            // At this point, we know we need to generate a new key.

            // We have been asked to generate a new key, but auto-generation of keys has been disabled.
            // We need to use the fallback key or fail.
            if (!_keyManagementOptions.AutoGenerateKeys)
            {
                var keyToUse = defaultKeyPolicy.DefaultKey ?? defaultKeyPolicy.FallbackKey;
                if (keyToUse == null)
                {
                    _logger.LogError("The key ring does not contain a valid default key, and the key manager is configured with auto-generation of keys disabled.");
                    throw new InvalidOperationException("The key ring does not contain a valid default key, and the key manager is configured with auto-generation of keys disabled.");
                }
                else
                {
                    _logger.LogWarning("Policy resolution states that a new key should be added to the key ring, but automatic generation of keys is disabled. Using fallback key {KeyId:B} with expiration {ExpirationDate:u} as default key.",
                        keyToUse.KeyId,
                        keyToUse.ExpirationDate);
                    return CreateCacheableKeyRingCoreStep2(now, cacheExpirationToken, keyToUse, allKeys);
                }
            }

            if (defaultKeyPolicy.DefaultKey == null)
            {
                // The case where there's no default key is the easiest scenario, since it
                // means that we need to create a new key with immediate activation.
                var newKey = _keyManager.CreateNewKey(activationDate: now, expirationDate: now + _keyManagementOptions.NewKeyLifetime);
                return CreateCacheableKeyRingCore(now, keyJustAdded: newKey); // recursively call
            }
            else
            {
                // If there is a default key, then the new key we generate should become active upon
                // expiration of the default key. The new key lifetime is measured from the creation
                // date (now), not the activation date.
                var newKey = _keyManager.CreateNewKey(activationDate: defaultKeyPolicy.DefaultKey.ExpirationDate, expirationDate: now + _keyManagementOptions.NewKeyLifetime);
                return CreateCacheableKeyRingCore(now, keyJustAdded: newKey); // recursively call
            }
        }

        private CacheableKeyRing CreateCacheableKeyRingCoreStep2(DateTimeOffset now, CancellationToken cacheExpirationToken, IKey defaultKey, IEnumerable<IKey> allKeys)
        {
            Debug.Assert(defaultKey != null);

            // Invariant: our caller ensures that CreateEncryptorInstance succeeded at least once
            Debug.Assert(defaultKey.CreateEncryptor() != null);

            _logger.LogDebug("Using key {KeyId:B} as the default key.", defaultKey.KeyId);

            var nextAutoRefreshTime = now + GetRefreshPeriodWithJitter(_keyManagementOptions.KeyRingRefreshPeriod);

            // The cached keyring should expire at the earliest of (default key expiration, next auto-refresh time).
            // Since the refresh period and safety window are not user-settable, we can guarantee that there's at
            // least one auto-refresh between the start of the safety window and the key's expiration date.
            // This gives us an opportunity to update the key ring before expiration, and it prevents multiple
            // servers in a cluster from trying to update the key ring simultaneously. Special case: if the default
            // key's expiration date is in the past, then we know we're using a fallback key and should disregard
            // its expiration date in favor of the next auto-refresh time.
            return new CacheableKeyRing(
                expirationToken: cacheExpirationToken,
                expirationTime: (defaultKey.ExpirationDate <= now) ? nextAutoRefreshTime : Min(defaultKey.ExpirationDate, nextAutoRefreshTime),
                defaultKey: defaultKey,
                allKeys: allKeys);
        }

        public IKeyRing GetCurrentKeyRing()
        {
            return GetCurrentKeyRingCore(DateTime.UtcNow);
        }

        internal IKeyRing RefreshCurrentKeyRing()
        {
            return GetCurrentKeyRingCore(DateTime.UtcNow, forceRefresh: true);
        }

        internal IKeyRing GetCurrentKeyRingCore(DateTime utcNow, bool forceRefresh = false)
        {
            Debug.Assert(utcNow.Kind == DateTimeKind.Utc);

            // Can we return the cached keyring to the caller?
            CacheableKeyRing existingCacheableKeyRing = null;
            if (!forceRefresh)
            {
                existingCacheableKeyRing = Volatile.Read(ref _cacheableKeyRing);
                if (CacheableKeyRing.IsValid(existingCacheableKeyRing, utcNow))
                {
                    return existingCacheableKeyRing.KeyRing;
                }
            }

            // The cached keyring hasn't been created or must be refreshed. We'll allow one thread to
            // update the keyring, and all other threads will continue to use the existing cached
            // keyring while the first thread performs the update. There is an exception: if there
            // is no usable existing cached keyring, all callers must block until the keyring exists.
            var acquiredLock = false;
            try
            {
                Monitor.TryEnter(_cacheableKeyRingLockObj, (existingCacheableKeyRing != null) ? 0 : Timeout.Infinite, ref acquiredLock);
                if (acquiredLock)
                {
                    if (!forceRefresh)
                    {
                        // This thread acquired the critical section and is responsible for updating the
                        // cached keyring. But first, let's make sure that somebody didn't sneak in before
                        // us and update the keyring on our behalf.
                        existingCacheableKeyRing = Volatile.Read(ref _cacheableKeyRing);
                        if (CacheableKeyRing.IsValid(existingCacheableKeyRing, utcNow))
                        {
                            return existingCacheableKeyRing.KeyRing;
                        }

                        if (existingCacheableKeyRing != null)
                        {
                            _logger.LogDebug("Existing cached key ring is expired. Refreshing.");
                        }
                    }

                    // It's up to us to refresh the cached keyring.
                    // This call is performed *under lock*.
                    CacheableKeyRing newCacheableKeyRing;

                    try
                    {
                        newCacheableKeyRing = CacheableKeyRingProvider.GetCacheableKeyRing(utcNow);
                    }
                    catch (Exception ex)
                    {
                        if (existingCacheableKeyRing != null)
                        {
                            _logger.LogError(ex, "An error occurred while refreshing the key ring. Will try again in 2 minutes.");
                        }
                        else
                        {
                            _logger.LogError(ex, "An error occurred while reading the key ring.");
                        }

                        // Failures that occur while refreshing the keyring are most likely transient, perhaps due to a
                        // temporary network outage. Since we don't want every subsequent call to result in failure, we'll
                        // create a new keyring object whose expiration is now + some short period of time (currently 2 min),
                        // and after this period has elapsed the next caller will try refreshing. If we don't have an
                        // existing keyring (perhaps because this is the first call), then there's nothing to extend, so
                        // each subsequent caller will keep going down this code path until one succeeds.
                        if (existingCacheableKeyRing != null)
                        {
                            Volatile.Write(ref _cacheableKeyRing, existingCacheableKeyRing.WithTemporaryExtendedLifetime(utcNow));
                        }

                        // The immediate caller should fail so that he can report the error up his chain. This makes it more likely
                        // that an administrator can see the error and react to it as appropriate. The caller can retry the operation
                        // and will probably have success as long as he falls within the temporary extension mentioned above.
                        throw;
                    }

                    Volatile.Write(ref _cacheableKeyRing, newCacheableKeyRing);
                    return newCacheableKeyRing.KeyRing;
                }
                else
                {
                    // We didn't acquire the critical section. This should only occur if we passed
                    // zero for the Monitor.TryEnter timeout, which implies that we had an existing
                    // (but outdated) keyring that we can use as a fallback.
                    Debug.Assert(existingCacheableKeyRing != null);
#pragma warning disable S2259 // Null pointers should not be dereferenced
                    return existingCacheableKeyRing.KeyRing;
#pragma warning restore S2259 // Null pointers should not be dereferenced
                }
            }
            finally
            {
                if (acquiredLock)
                {
                    Monitor.Exit(_cacheableKeyRingLockObj);
                }
            }
        }

        private static TimeSpan GetRefreshPeriodWithJitter(TimeSpan refreshPeriod)
        {
            // We'll fudge the refresh period up to -20% so that multiple applications don't try to
            // hit a single repository simultaneously. For instance, if the refresh period is 1 hour,
            // we'll return a value in the vicinity of 48 - 60 minutes. We use the Random class since
            // we don't need a secure PRNG for this.
            return TimeSpan.FromTicks((long)(refreshPeriod.Ticks * (1.0d - (new Random().NextDouble() / 5))));
        }

        private static DateTimeOffset Min(DateTimeOffset a, DateTimeOffset b)
        {
            return (a < b) ? a : b;
        }

        CacheableKeyRing ICacheableKeyRingProvider.GetCacheableKeyRing(DateTimeOffset now)
        {
            // the entry point allows one recursive call
            return CreateCacheableKeyRingCore(now, keyJustAdded: null);
        }
    }
}
