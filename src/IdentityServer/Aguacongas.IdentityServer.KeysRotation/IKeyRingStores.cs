// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using IdentityServer4.Stores;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;

namespace Aguacongas.IdentityServer.KeysRotation
{
    interface IKeyRingStores: IKeyRing, IValidationKeysStore, ISigningCredentialStore
    {
    }
}
