using Aguacongas.IdentityServer.KeysRotation;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.DataProtection.Internal;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IKeyRotationBuilder AddKeysRotation(this IServiceCollection services)
        {
            services.AddDataProtection();
            services.TryAddEnumerable(
                    ServiceDescriptor.Singleton<IConfigureOptions<KeyManagementOptions>, KeyManagementOptionsSetup>());

            return new KeyRotationBuilder
            {
                Services = services
                    .Configure<KeyManagementOptions>(options =>
                    {
                        options.AuthenticatedEncryptorConfiguration = new RsaEncryptorConfiguration();
                    })
                    .AddSingleton<ICacheableKeyRingProvider>(p =>
                    {
                        var options = p.GetRequiredService<IOptions<KeyManagementOptions>>();
                        var keyManager = new AspNetCore.DataProtection.KeyManagement.XmlKeyManager(options, p.GetRequiredService<IActivator>());
                        var resolver = new DefaultKeyResolver(options, p.GetService<ILoggerFactory>() ?? NullLoggerFactory.Instance);
                        return new KeyRingProvider(keyManager, options, resolver);
                    })
                    .AddTransient<IKeyRingStores>(p =>
                    {
                        var provider = p.GetRequiredService<ICacheableKeyRingProvider>();
                        return provider.GetCurrentKeyRing() as KeyRing;
                    })
                    .AddTransient(p => p.GetRequiredService<IKeyRingStores>() as IValidationKeysStore)
                    .AddTransient(p => p.GetRequiredService<IKeyRingStores>() as ISigningCredentialStore)
            };
        }
    }
}
