// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using IdentityServer4.Stores;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;

namespace Aguacongas.IdentityServer.KeysRotation
{
    public interface IKeyRingStores: IKeyRing, IValidationKeysStore, ISigningCredentialStore
    {
        IKey DefaultKey { get;  }
    }
}
