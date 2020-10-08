// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Aguacongas.IdentityServer.Admin.Services
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [SuppressMessage("Major Code Smell", "S2326:Unused type parameters should be removed", Justification = "Used by DI")]
    public class KeyManagerWrapper<T> where T : IAuthenticatedEncryptorDescriptor
    {
        private readonly IKeyManager _keyManager;
        private readonly IDefaultKeyResolver _defaultKeyResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyManagerWrapper{T}"/> class.
        /// </summary>
        /// <param name="keyManager">The key manager.</param>
        /// <param name="defaultKeyResolver">The default key resolver.</param>
        /// <exception cref="ArgumentNullException">
        /// keyManager
        /// or
        /// defaultKeyResolver
        /// </exception>
        public KeyManagerWrapper(IKeyManager keyManager, IDefaultKeyResolver defaultKeyResolver)
        {
            _keyManager = keyManager ?? throw new ArgumentNullException(nameof(keyManager));
            _defaultKeyResolver = defaultKeyResolver ?? throw new ArgumentNullException(nameof(defaultKeyResolver));
        }

        /// <summary>
        /// Gets the key manager.
        /// </summary>
        /// <value>
        /// The key manager.
        /// </value>
        public IKeyManager Manager => _keyManager;

        /// <summary>
        /// Gets the default key resolver.
        /// </summary>
        /// <value>
        /// The default key resolver.
        /// </value>
        public IDefaultKeyResolver DefaultKeyResolver => _defaultKeyResolver;
    }
}
