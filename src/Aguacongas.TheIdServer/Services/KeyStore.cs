// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.IdentityServer.Admin.Services;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.Services
{
    public class KeyStore<T1, T2> : IKeyStore<T1> where T2 : IAuthenticatedEncryptorDescriptor
    {
        private readonly KeyManagerWrapper<T2> _wrapper;

        public KeyStore(KeyManagerWrapper<T2> wrapper)
        {
            _wrapper = wrapper ?? throw new ArgumentNullException(nameof(wrapper));
        }

        public Task<PageResponse<Key>> GetAllKeysAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(_wrapper.GetAllKeys());

        public Task RevokeKeyAsync(string id, string reason, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();
    }
}
