// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// Modifications copyright (c) 2021 @Olivier Lefebvre

// This file is a copy of https://github.com/dotnet/aspnetcore/blob/master/src/DataProtection/DataProtection/src/KeyManagement/KeyRingProvider.cs
// with:
// namespace change from original Microsoft.AspNetCore.DataProtection.KeyManagement
// implementation of IKeyRingProvider declaration removed
// options change from original KeyManagementOptions
// add property KeyManager
// original CryptoUtil.Fail call replaced
// original ILogger extensions calls replaced
// next key expiration date rule change to use the default key expiration date and not now
// explicit implemenation of ICacheableKeyRingProvider added

using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Threading;

// namespace change from original Microsoft.AspNetCore.DataProtection.KeyManagement
namespace Aguacongas.IdentityServer.KeysRotation
{
    internal sealed class KeyRingProvider<TC, TE> : ICacheableKeyRingProvider<TC, TE> // implementation of IKeyRingProvider declaration removed
        where TC : SigningAlgorithmConfiguration
        where TE : ISigningAlgortithmEncryptor
    {
        private CacheableKeyRing<TC, TE> _cacheableKeyRing;
        private readonly object _cacheableKeyRingLockObj = new();
        private readonly IDefaultKeyResolver _defaultKeyResolver;
        private readonly KeyRotationOptions _keyManagementOptions; // options change from original KeyManagementOptions
        private readonly IKeyManager _keyManager;
        private readonly ILogger _logger;

        public KeyRingProvider(
            IKeyManager keyManager,
            IOptions<KeyRotationOptions> keyManagementOptions, // options change from original KeyManagementOptions
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
            IOptions<KeyRotationOptions> keyManagementOptions, // options change from original KeyManagementOptions
            IDefaultKeyResolver defaultKeyResolver,
            ILoggerFactory loggerFactory)
        {
            _keyManagementOptions = new KeyRotationOptions(keyManagementOptions.Value); // clone so new instance is immutable
            _keyManager = keyManager;
            CacheableKeyRingProvider = this;
            _defaultKeyResolver = defaultKeyResolver;
            _logger = loggerFactory.CreateLogger<KeyRingProvider<TC, TE>>();

            // We will automatically refresh any unknown keys for 2 minutes see https://github.com/aspnet/AspNetCore/issues/3975
            AutoRefreshWindowEnd = DateTime.UtcNow.AddMinutes(2);
        }

        public IKeyManager KeyManager => _keyManager; // add property KeyManager

        public string Algorithm => ((SigningAlgorithmConfiguration) _keyManagementOptions.AuthenticatedEncryptorConfiguration).SigningAlgorithm; // add property Algorithm

        public IKeyRing GetCurrentKeyRing()
        {
            return GetCurrentKeyRingCore(DateTime.UtcNow);
        }

        internal ICacheableKeyRingProvider<TC, TE> CacheableKeyRingProvider { get; set; }

        internal DateTime AutoRefreshWindowEnd { get; set; }

        internal bool InAutoRefreshWindow() => DateTime.UtcNow < AutoRefreshWindowEnd;

        internal IKeyRing RefreshCurrentKeyRing()
        {
            return GetCurrentKeyRingCore(DateTime.UtcNow, forceRefresh: true);
        }

        internal IKeyRing GetCurrentKeyRingCore(DateTime utcNow, bool forceRefresh = false)
        {
            Debug.Assert(utcNow.Kind == DateTimeKind.Utc);

            // Can we return the cached keyring to the caller?
            CacheableKeyRing<TC, TE> existingCacheableKeyRing = null;
            if (!forceRefresh)
            {
                existingCacheableKeyRing = Volatile.Read(ref _cacheableKeyRing);
                if (CacheableKeyRing<TC, TE>.IsValid(existingCacheableKeyRing, utcNow))
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
                    return GetCurrentKeyRingSync(utcNow, forceRefresh, ref existingCacheableKeyRing);
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

        private CacheableKeyRing<TC, TE> CreateCacheableKeyRingCore(DateTimeOffset now, IKey keyJustAdded)
        {
            // Refresh the list of all keys
            var cacheExpirationToken = KeyManager.GetCacheExpirationToken();
            var allKeys = GetAllKeys();

            // Fetch the current default key from the list of all keys
            var defaultKeyPolicy = _defaultKeyResolver.ResolveDefaultKeyPolicy(now, allKeys);
            if (!defaultKeyPolicy.ShouldGenerateNewKey)
            {
                if (defaultKeyPolicy.DefaultKey == null)
                {
                    throw new CryptographicException("Assertion failed: Expected to see a default key."); // original CryptoUtil.Fail call replaced
                }

                return CreateCacheableKeyRingCoreStep2(now, defaultKeyPolicy.DefaultKey, allKeys, cacheExpirationToken);
            }

            _logger.LogDebug("Policy resolution states that a new key should be added to the key ring."); // original ILogger extensions calls replaced

            // We shouldn't call CreateKey more than once, else we risk stack diving. This code path shouldn't
            // get hit unless there was an ineligible key with an activation date slightly later than the one we
            // just added. If this does happen, then we'll just use whatever key we can instead of creating
            // new keys endlessly, eventually falling back to the one we just added if all else fails.
            if (keyJustAdded != null)
            {
                var keyToUse = defaultKeyPolicy.DefaultKey ?? defaultKeyPolicy.FallbackKey ?? keyJustAdded;
                return CreateCacheableKeyRingCoreStep2(now, keyToUse, allKeys, cacheExpirationToken);
            }

            // At this point, we know we need to generate a new key.

            // We have been asked to generate a new key, but auto-generation of keys has been disabled.
            // We need to use the fallback key or fail.
            if (!_keyManagementOptions.AutoGenerateKeys)
            {
                var keyToUse = defaultKeyPolicy.DefaultKey ?? defaultKeyPolicy.FallbackKey;
                if (keyToUse == null)
                {
                    _logger.LogError("The key ring does not contain a valid default key, and the key manager is configured with auto-generation of keys disabled."); // original ILogger extensions calls replaced
                    throw new InvalidOperationException("The key ring does not contain a valid default key, and the key manager is configured with auto-generation of keys disabled.");
                }
                else
                {
                    _logger.LogWarning("Policy resolution states that a new key should be added to the key ring, but automatic generation of keys is disabled. Using fallback key {KeyId:B} with expiration {ExpirationDate:u} as default key.",
                        keyToUse.KeyId,
                        keyToUse.ExpirationDate); // original ILogger extensions calls replaced
                    return CreateCacheableKeyRingCoreStep2(now, keyToUse, allKeys, cacheExpirationToken);
                }
            }

            if (defaultKeyPolicy.DefaultKey == null)
            {
                // The case where there's no default key is the easiest scenario, since it
                // means that we need to create a new key with immediate activation.
                var newKey = KeyManager.CreateNewKey(activationDate: now, expirationDate: now + _keyManagementOptions.NewKeyLifetime);
                return CreateCacheableKeyRingCore(now, keyJustAdded: newKey); // recursively call
            }
            else
            {
                // If there is a default key, then the new key we generate should become active upon
                // expiration of the default key.
                var newActivationDate = defaultKeyPolicy.DefaultKey.ExpirationDate;
                var newExpirationDate = defaultKeyPolicy.DefaultKey.ExpirationDate + _keyManagementOptions.NewKeyLifetime;
                var keyAlreadyCreated = allKeys.FirstOrDefault(k => k.ActivationDate == newActivationDate && k.ExpirationDate == newExpirationDate && !k.IsRevoked);
                if (keyAlreadyCreated == null)
                {
                    var newKey = KeyManager.CreateNewKey(activationDate: newActivationDate, expirationDate: newExpirationDate); // next key expiration date rule change to use the default key expiration date and not now
                    return CreateCacheableKeyRingCore(now, keyJustAdded: newKey); // recursively call
                }
                // The next key already exists. Don't need to create a new one. It can occur when the NewKeyLifetime < KeyPropagationWindow
                return CreateCacheableKeyRingCore(now, keyJustAdded: keyAlreadyCreated); // recursively call
            }
        }

        private IReadOnlyCollection<IKey> GetAllKeys()
        {
            var allKeys = KeyManager.GetAllKeys();
            if (_keyManagementOptions.AuthenticatedEncryptorConfiguration is RsaEncryptorConfiguration rsaConfiguration)
            {
                allKeys = allKeys.Where(k => k.Descriptor is RsaEncryptorDescriptor descriptor &&
                    descriptor.Configuration.SigningAlgorithm == rsaConfiguration.SigningAlgorithm).ToArray();
            }
            if (_keyManagementOptions.AuthenticatedEncryptorConfiguration is ECDsaEncryptorConfiguration eCDsaEncryptorConfiguration)
            {
                allKeys = allKeys.Where(k => k.Descriptor is ECDsaEncryptorDescriptor descriptor &&
                    descriptor.Configuration.SigningAlgorithm == eCDsaEncryptorConfiguration.SigningAlgorithm).ToArray();
            }

            return allKeys;
        }

        private CacheableKeyRing<TC, TE> CreateCacheableKeyRingCoreStep2(DateTimeOffset now, IKey defaultKey, IEnumerable<IKey> allKeys, CancellationToken cacheExpirationToken)
        {
            Debug.Assert(defaultKey != null);

            // Invariant: our caller ensures that CreateEncryptorInstance succeeded at least once
            Debug.Assert(defaultKey.CreateEncryptor() != null);

            _logger.LogDebug("Using key {KeyId:B} as the default key.", defaultKey.KeyId); // original ILogger extensions calls replaced

            var nextAutoRefreshTime = now + GetRefreshPeriodWithJitter(_keyManagementOptions.KeyRingRefreshPeriod);

            // The cached keyring should expire at the earliest of (default key expiration, next auto-refresh time).
            // Since the refresh period and safety window are not user-settable, we can guarantee that there's at
            // least one auto-refresh between the start of the safety window and the key's expiration date.
            // This gives us an opportunity to update the key ring before expiration, and it prevents multiple
            // servers in a cluster from trying to update the key ring simultaneously. Special case: if the default
            // key's expiration date is in the past, then we know we're using a fallback key and should disregard
            // its expiration date in favor of the next auto-refresh time.
            return new CacheableKeyRing<TC, TE>(
                expirationToken: cacheExpirationToken,
                expirationTime: (defaultKey.ExpirationDate <= now) ? nextAutoRefreshTime : Min(defaultKey.ExpirationDate, nextAutoRefreshTime),
                defaultKey: defaultKey,
                allKeys: allKeys,
                configuration: _keyManagementOptions.AuthenticatedEncryptorConfiguration as TC);
        }

        private IKeyRing GetCurrentKeyRingSync(DateTime utcNow, bool forceRefresh, ref CacheableKeyRing<TC, TE> existingCacheableKeyRing)
        {
            if (!forceRefresh)
            {
                // This thread acquired the critical section and is responsible for updating the
                // cached keyring. But first, let's make sure that somebody didn't sneak in before
                // us and update the keyring on our behalf.
                existingCacheableKeyRing = Volatile.Read(ref _cacheableKeyRing);
                if (CacheableKeyRing<TC, TE>.IsValid(existingCacheableKeyRing, utcNow))
                {
                    return existingCacheableKeyRing.KeyRing;
                }

                if (existingCacheableKeyRing != null)
                {
                    _logger.LogDebug("Existing cached key ring is expired. Refreshing."); // original ILogger extensions calls replaced
                }
            }

            // It's up to us to refresh the cached keyring.
            // This call is performed *under lock*.
            CacheableKeyRing<TC, TE> newCacheableKeyRing;

            try
            {
                newCacheableKeyRing = CacheableKeyRingProvider.GetCacheableKeyRing(utcNow);
            }
            catch (Exception ex)
            {
                if (existingCacheableKeyRing != null)
                {
                    _logger.LogError(ex, "An error occurred while refreshing the key ring. Will try again in 2 minutes."); // original ILogger extensions calls replaced
                }
                else
                {
                    _logger.LogError(ex, "An error occurred while reading the key ring."); // original ILogger extensions calls replaced
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

        CacheableKeyRing<TC, TE> ICacheableKeyRingProvider<TC, TE>.GetCacheableKeyRing(DateTimeOffset now)
        {
            // the entry point allows one recursive call
            return CreateCacheableKeyRingCore(now, keyJustAdded: null);
        }

        IKeyRing ICacheableKeyRingProvider.RefreshCurrentKeyRing() // explicit implemenation of ICacheableKeyRingProvider added
        {
            return RefreshCurrentKeyRing();
        }
    }
}
