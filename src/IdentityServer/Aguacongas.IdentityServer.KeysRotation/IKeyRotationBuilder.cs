using Microsoft.Extensions.DependencyInjection;

namespace Aguacongas.IdentityServer.KeysRotation
{
    public interface IKeyRotationBuilder
    {
        IServiceCollection Services { get; set; }
    }
}