using Microsoft.Extensions.DependencyInjection;

namespace Aguacongas.IdentityServer.KeysRotation
{
    internal class KeyRotationBuilder : IKeyRotationBuilder
    {
        public IServiceCollection Services { get; set; }
    }
}
