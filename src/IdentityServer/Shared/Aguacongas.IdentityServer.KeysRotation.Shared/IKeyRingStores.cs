// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
#if DUENDE
using Duende.IdentityServer.Stores;
#else
using IdentityServer4.Stores;
#endif
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;

namespace Aguacongas.IdentityServer.KeysRotation
{
    public interface IKeyRingStores: IKeyRing, IValidationKeysStore, ISigningCredentialStore
    {
        IKey DefaultKey { get;  }
    }
}
