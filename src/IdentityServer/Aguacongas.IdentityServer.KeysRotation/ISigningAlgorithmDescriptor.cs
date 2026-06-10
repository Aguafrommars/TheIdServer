// Project: Aguafrommars/TheIdServer
// Copyright (c) 2026 @Olivier Lefebvre
namespace Aguacongas.IdentityServer.KeysRotation
{
    public interface ISigningAlgorithmDescriptor
    {
        SigningAlgorithmConfiguration Configuration { get; }
    }
}
