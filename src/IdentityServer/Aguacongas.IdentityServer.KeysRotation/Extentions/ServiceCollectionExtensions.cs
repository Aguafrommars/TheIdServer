// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.IdentityServer.KeysRotation;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.DataProtection.Internal;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// <see cref="IServiceCollection"/> extensions
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the keys rotation.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configureKeysRotation">Action to configure keys rotation.</param>
        /// <returns></returns>
        public static IKeyRotationBuilder AddKeysRotation(this IServiceCollection services, Action<KeyRotationOptions> configureKeysRotation = null)
        {
            services.AddDataProtection();
            services.TryAddEnumerable(
                    ServiceDescriptor.Singleton<IConfigureOptions<KeyRotationOptions>, KeyRotationOptionsSetup>());

            return new KeyRotationBuilder
            {
                Services = services
                    .Configure<KeyRotationOptions>(options =>
                    {
                        options.AuthenticatedEncryptorConfiguration = new RsaEncryptorConfiguration();
                        configureKeysRotation?.Invoke(options);
                    })
                    .AddSingleton<ICacheableKeyRingProvider>(p =>
                    {
                        var options = p.GetRequiredService<IOptions<KeyRotationOptions>>();
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
