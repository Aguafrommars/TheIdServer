using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Aguacongas.IdentityServer.Abstractions
{
    public interface ISetupClaimsProvider
    {
        IServiceCollection SetupClaimsProvider(IServiceCollection services, IConfiguration configuration);
    }
}
