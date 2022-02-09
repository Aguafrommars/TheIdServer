// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre

// This file is a copy of https://github.com/dotnet/aspnetcore/blob/v3.1.8/src/DataProtection/DataProtection/src/KeyManagement/KeyRing.cs
// with:
// namespace change from original Microsoft.AspNetCore.DataProtection.KeyManagement
// interface implementation chance from orignal IKeyRing
// add RsaEncryptorConfiguration instance in constructor
// use discard intead of unused instance
// use inline declaration
// use object locker instead of this
#if DUENDE
using Duende.IdentityServer.Models;
#else
using IdentityServer4.Models;
#endif
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

// namespace change from original Microsoft.AspNetCore.DataProtection.KeyManagement
namespace Aguacongas.IdentityServer.KeysRotation
{
    /// <summary>
    /// A basic implementation of <see cref="IKeyRingStores"/>.
    /// </summary>
    internal sealed class KeyRing : IKeyRingStores // interface implementation chance from orignal IKeyRing
    {
        private readonly KeyHolder _defaultKeyHolder;
        private readonly RsaEncryptorConfiguration _configuration;
        private readonly Dictionary<Guid, KeyHolder> _keyIdToKeyHolderMap;

        public KeyRing(IKey defaultKey, IEnumerable<IKey> allKeys, RsaEncryptorConfiguration configuration) // add RsaEncryptorConfiguration intance from orignal
        {
            _keyIdToKeyHolderMap = new Dictionary<Guid, KeyHolder>();
            foreach (IKey key in allKeys)
            {
                _keyIdToKeyHolderMap.Add(key.KeyId, new KeyHolder(key));
            }

            // It's possible under some circumstances that the default key won't be part of 'allKeys',
            // such as if the key manager is forced to use the key it just generated even if such key
            // wasn't in the underlying repository. In this case, we just add it now.
            if (!_keyIdToKeyHolderMap.ContainsKey(defaultKey.KeyId))
            {
                _keyIdToKeyHolderMap.Add(defaultKey.KeyId, new KeyHolder(defaultKey));
            }

            DefaultKeyId = defaultKey.KeyId;
            _defaultKeyHolder = _keyIdToKeyHolderMap[DefaultKeyId];
            _configuration = configuration; // add RsaEncryptorConfiguration instance from orignal
        }

        public IAuthenticatedEncryptor DefaultAuthenticatedEncryptor
        {
            get
            {
                return _defaultKeyHolder.GetEncryptorInstance(out _); // use discard of unused instance
            }
        }

        public Guid DefaultKeyId { get; }

        public IKey DefaultKey => _defaultKeyHolder.Key;

        public IAuthenticatedEncryptor GetAuthenticatedEncryptorByKeyId(Guid keyId, out bool isRevoked) // interface implementation chance from orignal IKeyRing
        {
            isRevoked = false;
            _keyIdToKeyHolderMap.TryGetValue(keyId, out KeyHolder holder); // use inline declaration
            return holder?.GetEncryptorInstance(out isRevoked);
        }

        public Task<SigningCredentials> GetSigningCredentialsAsync() // interface implementation chance from orignal IKeyRing
        {
            if (!(_defaultKeyHolder.GetEncryptorInstance(out bool isRevoked) is RsaEncryptor encryptor))
            {
                throw new InvalidOperationException($"The default key is not an Rsa key. Revoked : {isRevoked}");
            }
            return Task.FromResult(encryptor.GetSigningCredentials(_configuration.RsaSigningAlgorithm));
        }

        public Task<IEnumerable<SecurityKeyInfo>> GetValidationKeysAsync()
        {
            var signingCredentialsList = _keyIdToKeyHolderMap.Values
                .Where(h => h.GetEncryptorInstance(out bool isRevoked) is RsaEncryptor && !isRevoked && h.Key.ExpirationDate <= DateTimeOffset.UtcNow + _configuration.KeyRetirement)
                .Select(h => h.GetEncryptorInstance(out _) as RsaEncryptor)
                .Where(e => e != null)
                .Select((RsaEncryptor e) => e.GetSecurityKeyInfo(_configuration.RsaSigningAlgorithm));
            return Task.FromResult(signingCredentialsList);
        }

        // used for providing lazy activation of the authenticated encryptor instance
        private sealed class KeyHolder
        {
            private readonly object _locker = new object(); // use object locker instead of this
            private readonly IKey _key;
            private IAuthenticatedEncryptor _encryptor;

            internal KeyHolder(IKey key)
            {
                _key = key;
            }

            public IKey Key => _key; // change to implement IKeyRingStores

            internal IAuthenticatedEncryptor GetEncryptorInstance(out bool isRevoked)
            {
                // simple double-check lock pattern
                // we can't use LazyInitializer<T> because we don't have a simple value factory
                IAuthenticatedEncryptor encryptor = Volatile.Read(ref _encryptor);
                if (encryptor == null)
                {
                    lock (_locker) // use object locker instead of this
                    {
                        encryptor = Volatile.Read(ref _encryptor);
                        if (encryptor == null)
                        {
                            encryptor = Key.CreateEncryptor();
                            Volatile.Write(ref _encryptor, encryptor);
                        }
                    }
                }
                isRevoked = Key.IsRevoked; // change to implement IKeyRingStores
                return encryptor;
            }
        }
    }
}
