// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Duende.IdentityServer.Stores;
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
