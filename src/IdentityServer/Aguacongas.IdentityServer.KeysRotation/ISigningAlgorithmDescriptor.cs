// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
namespace Aguacongas.IdentityServer.KeysRotation
{
    public interface ISigningAlgorithmDescriptor
    {
        SigningAlgorithmConfiguration Configuration { get; }
    }
}
