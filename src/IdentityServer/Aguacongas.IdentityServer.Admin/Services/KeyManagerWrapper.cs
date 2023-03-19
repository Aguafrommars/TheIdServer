// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Aguacongas.IdentityServer.Abstractions;
using Aguacongas.IdentityServer.Admin.Extensions;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Admin.Services
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class KeyManagerWrapper<T> where T : IAuthenticatedEncryptorDescriptor
    {
        private readonly IEnumerable<Tuple<IKeyManager, string, IEnumerable<IKey>>> _keyManagerList;
        private readonly IDefaultKeyResolver _defaultKeyResolver;
        private readonly IProviderClient _providerClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyManagerWrapper{T}" /> class.
        /// </summary>
        /// <param name="keyManagerList">The list of key manager.</param>
        /// <param name="defaultKeyResolver">The default key resolver.</param>
        /// <param name="providerClient">The provider client.</param>
        /// <exception cref="ArgumentNullException">
        /// keyManager
        /// or
        /// defaultKeyResolver
        /// or
        /// providerClient
        /// </exception>
        /// <exception cref="ArgumentNullException">keyManager
        /// or
        /// defaultKeyResolver</exception>
        public KeyManagerWrapper(IEnumerable<Tuple<IKeyManager, string, IEnumerable<IKey>>> keyManagerList, IDefaultKeyResolver defaultKeyResolver, IProviderClient providerClient)
        {
            _keyManagerList = keyManagerList ?? throw new ArgumentNullException(nameof(keyManagerList));
            _defaultKeyResolver = defaultKeyResolver ?? throw new ArgumentNullException(nameof(defaultKeyResolver));
            _providerClient = providerClient ?? throw new ArgumentNullException(nameof(providerClient));
        }

        /// <summary>
        /// Gets all keys.
        /// </summary>
        /// <returns></returns>
        public PageResponse<Key> GetAllKeys()
        {
            return new PageResponse<Key>
            {
                Items = _keyManagerList.SelectMany(t => t.Item3.Select(k => new Tuple<IKey, string>(k, t.Item2)).ToPageResponse(_defaultKeyResolver).Items),
                Count = _keyManagerList.Sum(t => t.Item3.Select(k => new Tuple<IKey, string>(k, t.Item2)).ToPageResponse(_defaultKeyResolver).Count)
            };
        }

        /// <summary>
        /// Revokes the key.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="reason">The reason.</param>
        public Task RevokeKey(Guid id, string reason)
        {
            foreach (var manager in _keyManagerList
                .Where(t => t.Item1.GetAllKeys().Any(k => k.KeyId == id))
                .Select(t => t.Item1))
            {
                manager.RevokeKey(id, reason);
            }
            return _providerClient.KeyRevokedAsync(typeof(T).Name, id.ToString());
        }
    }
}
