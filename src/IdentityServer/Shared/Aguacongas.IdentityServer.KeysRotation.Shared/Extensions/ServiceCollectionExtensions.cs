// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.IdentityServer.KeysRotation;
#if DUENDE
using Duende.IdentityServer.Stores;
using static Duende.IdentityServer.IdentityServerConstants;
#else
using IdentityServer4.Stores;
using static IdentityServer4.IdentityServerConstants;
#endif
using Microsoft.AspNetCore.DataProtection.Internal;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// <see cref="IServiceCollection"/> extensions
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddECDsaAddValidationKeysStore(this IServiceCollection services, ECDsaSigningAlgorithm signingAlgorithm)
        {
            return services.AddValidattionKeysStore<ECDsaEncryptorConfiguration, ECDsaEncryptor>(signingAlgorithm.ToString());
        }

        public static IServiceCollection AddRsaAddValidationKeysStore(this IServiceCollection services, RsaSigningAlgorithm signingAlgorithm)
        {
            return services.AddValidattionKeysStore<RsaEncryptorConfiguration, RsaEncryptor>(signingAlgorithm.ToString());
        }

        /// <summary>
        /// Adds the keys rotation.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configureKeysRotation">Action to configure keys rotation.</param>
        /// <returns></returns>
        public static IKeyRotationBuilder AddKeysRotation(this IServiceCollection services, RsaSigningAlgorithm rsaSigningAlgorithm = RsaSigningAlgorithm.RS256, Action<KeyRotationOptions> configureKeysRotation = null)
        {
            services.AddDataProtection();
            services.TryAddEnumerable(
                    ServiceDescriptor.Singleton<IConfigureOptions<KeyRotationOptions>, KeyRotationOptionsSetup>());

            return new KeyRotationBuilder
            {
                Services = services
                    .AddKeysRotation<RsaEncryptorConfiguration, RsaEncryptor>(rsaSigningAlgorithm.ToString(), configureKeysRotation)
                    
            }.AddRsaEncryptorConfiguration(rsaSigningAlgorithm, options =>
            {
                options.SigningAlgorithm = rsaSigningAlgorithm.ToString();
                options.IsDefaultSigningAlgorithm = true;
            });
        }

        public static IServiceCollection AddKeysRotation<TC,TE>(this IServiceCollection services, string signingAlgorithm, Action<KeyRotationOptions> configureKeysRotation = null)
            where TC : SigningAlgorithmConfiguration, new()
            where TE : ISigningAlgortithmEncryptor
        {
            return services
                .Configure<KeyRotationOptions>(signingAlgorithm, options =>
                {
                    var configuration = new TC();
                    configuration.SigningAlgorithm = signingAlgorithm;
                    options.AuthenticatedEncryptorConfiguration = configuration;
                    configureKeysRotation?.Invoke(options);
                })
                .AddSingleton<ICacheableKeyRingProvider<TC, TE>>(p =>
                {
                    var optionsFactory = p.GetRequiredService<IOptionsFactory<KeyRotationOptions>>();
                    var configueOptionsList = p.GetRequiredService<IEnumerable<IConfigureOptions<KeyRotationOptions>>>();
                    var settings = optionsFactory.Create(signingAlgorithm);

                    foreach(var configure in configueOptionsList)
                    {
                        configure.Configure(settings);
                    }
                    var options = Options.Options.Create(settings);
                    var keyManager = new AspNetCore.DataProtection.KeyManagement.XmlKeyManager(options, p.GetRequiredService<IActivator>());
                    var resolver = new DefaultKeyResolver(options, p.GetService<ILoggerFactory>() ?? NullLoggerFactory.Instance);
                    return new KeyRingProvider<TC, TE>(keyManager, options, resolver);
                })
                .AddTransient<ICacheableKeyRingProvider>(p =>
                {
                    var providerList = p.GetRequiredService<IEnumerable<ICacheableKeyRingProvider<TC, TE>>>();
                    return providerList.First(p => p.Algorithm == signingAlgorithm);
                })
                .AddTransient<IKeyRingStore<TC, TE>>(p =>
                {
                    var providerList = p.GetRequiredService<IEnumerable<ICacheableKeyRingProvider<TC, TE>>>();
                    return providerList.First(p => p.Algorithm == signingAlgorithm).GetCurrentKeyRing() as KeyRing<TC, TE>;
                })
                .AddTransient(p =>
                {
                    var list = p.GetRequiredService<IEnumerable<IKeyRingStore<TC, TE>>>();
                    return list.First(s => s.Algorithm == signingAlgorithm) as IValidationKeysStore;
                })
                .AddTransient(p => {
                    var list = p.GetRequiredService<IEnumerable<IKeyRingStore<TC, TE>>>();
                    return list.First(s => s.Algorithm == signingAlgorithm) as ISigningCredentialStore;
                });
        }

        static IServiceCollection AddValidattionKeysStore<TC, TE>(this IServiceCollection services, string signingAlgorithm)
            where TC : SigningAlgorithmConfiguration, new()
            where TE : ISigningAlgortithmEncryptor
        {
            return services.AddTransient<IValidationKeysStore>(p =>
            {
                var providerList = p.GetRequiredService<IEnumerable<ICacheableKeyRingProvider<TC, TE>>>();
                return new ValidattionKeysStore(providerList.First(rp => rp.Algorithm == signingAlgorithm));
            });
        }
    }
}
