using Aguacongas.IdentityServer.KeysRotation;
using Microsoft.AspNetCore.DataProtection.Internal;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddKeysRotation(this IServiceCollection services)
        {
            return services.AddTransient<ICacheableKeyRingProvider>(p =>
            {
                var options = p.GetRequiredService<IOptions<KeyManagementOptions>>();
                var keyManager = new AspNetCore.DataProtection.KeyManagement.XmlKeyManager(options, p.GetRequiredService<IActivator>());
                var resolver = p.GetRequiredService<AspNetCore.DataProtection.KeyManagement.Internal.IDefaultKeyResolver>();
                return new KeyRingProvider(keyManager, options, resolver);
            });
        }
    }
}
