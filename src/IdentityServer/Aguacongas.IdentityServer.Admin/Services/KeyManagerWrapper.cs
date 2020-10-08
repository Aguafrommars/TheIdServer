// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.IdentityServer.Abstractions;
using Aguacongas.IdentityServer.Admin.Extensions;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using System;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Admin.Services
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class KeyManagerWrapper<T> where T : IAuthenticatedEncryptorDescriptor
    {
        private readonly IKeyManager _keyManager;
        private readonly IDefaultKeyResolver _defaultKeyResolver;
        private readonly IProviderClient _providerClient;

        /// <summary>
        /// Gets the key manager.
        /// </summary>
        /// <value>
        /// The key manager.
        /// </value>
        public IKeyManager Manager => _keyManager;


        /// <summary>
        /// Initializes a new instance of the <see cref="KeyManagerWrapper{T}" /> class.
        /// </summary>
        /// <param name="keyManager">The key manager.</param>
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
        public KeyManagerWrapper(IKeyManager keyManager, IDefaultKeyResolver defaultKeyResolver, IProviderClient providerClient)
        {
            _keyManager = keyManager ?? throw new ArgumentNullException(nameof(keyManager));
            _defaultKeyResolver = defaultKeyResolver ?? throw new ArgumentNullException(nameof(defaultKeyResolver));
            _providerClient = providerClient ?? throw new ArgumentNullException(nameof(providerClient));
        }

        /// <summary>
        /// Gets all keys.
        /// </summary>
        /// <returns></returns>
        public PageResponse<Key> GetAllKeys()
        {
            return _keyManager.GetAllKeys().ToPageResponse(_defaultKeyResolver);
        }

        /// <summary>
        /// Revokes the key.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="reason">The reason.</param>
        public Task RevokeKey(Guid id, string reason)
        {
            _keyManager.RevokeKey(id, reason);
            return _providerClient.KeyRevokedAsync(typeof(T).Name, id);
        }

    }
}
