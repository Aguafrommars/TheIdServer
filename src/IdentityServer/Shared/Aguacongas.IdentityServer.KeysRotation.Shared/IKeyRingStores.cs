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
    public interface IKeyRingStore: IKeyRing, IValidationKeysStore, ISigningCredentialStore
    {
        IKey DefaultKey { get;  }
    }

    public interface IKeyRingStore<TC, TE> : IKeyRingStore
        where TC : SigningAlgorithmConfiguration
        where TE : ISigningAlgortithmEncryptor
    {
        string Algorithm { get; }
    }
}
