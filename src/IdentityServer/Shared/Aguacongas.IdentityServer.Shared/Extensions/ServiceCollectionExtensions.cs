// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer;
using Aguacongas.IdentityServer.Abstractions;
using Aguacongas.IdentityServer.Store;
using Aguacongas.TheIdServer.Authentication;
#if DUENDE
using Aguacongas.IdentityServer.Services;
using Aguacongas.IdentityServer.Validators;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using Duende.IdentityServer.Stores.Serialization;
using Duende.IdentityServer.Validation;
#else
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Stores.Serialization;
#endif
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Service collection extensions
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the identity provider store in DI container.
        /// </summary>
        /// <param name="services">The collection of service.</param>
        /// <returns></returns>
        public static IServiceCollection AddIdentityProviderStore(this IServiceCollection services)
        {
            return services
                .AddSingleton(p => new OAuthTokenManager(p.GetRequiredService<HttpClient>(), p.GetRequiredService<IOptions<IdentityServerOptions>>()))
                .AddTransient<OAuthDelegatingHandler>()
                .AddTransient(p => new HttpClient(p.GetRequiredService<HttpClientHandler>()));
        }

        public static IServiceCollection AddConfigurationStores(this IServiceCollection services)
        {
            services.TryAddScoped(typeof(IFlushableCache<>), typeof(FlushableCache<>));

            return services.AddTransient<ClientStore>()
                .AddTransient<IClientStore, ValidatingClientStore<ClientStore>>()
                .AddTransient<IResourceStore, ResourceStore>()
                .AddTransient<ICorsPolicyService, CorsPolicyService>()
                .AddTransient<IExternalProviderKindStore, ExternalProviderKindStore<SchemeDefinition>>();
        }

        public static IServiceCollection AddOperationalStores(this IServiceCollection services)
        {
            return services.AddTransient<AuthorizationCodeStore>()
                .AddTransient<RefreshTokenStore>()
                .AddTransient<ReferenceTokenStore>()
                .AddTransient<UserConsentStore>()
                .AddTransient<DeviceFlowStore>()
                .AddTransient<IPersistentGrantSerializer, PersistentGrantSerializer>()
                .AddTransient<IAuthorizationCodeStore>(p => p.GetRequiredService<AuthorizationCodeStore>())
                .AddTransient<IRefreshTokenStore>(p => p.GetRequiredService<RefreshTokenStore>())
                .AddTransient<IReferenceTokenStore>(p => p.GetRequiredService<ReferenceTokenStore>())
                .AddTransient<IUserConsentStore>(p => p.GetRequiredService<UserConsentStore>())
#if DUENDE
                .AddTransient<IBackChannelAuthenticationRequestStore, BackChannelAuthenticationRequestStore>()
#endif
                .AddTransient<IDeviceFlowStore>(p => p.GetRequiredService<DeviceFlowStore>());
        }

#if DUENDE
        public static IServiceCollection AddCibaServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<BackchannelAuthenticationUserNotificationServiceOptions>(configuration)
                .AddSingleton<OAuthTokenManager<BackchannelAuthenticationUserNotificationServiceOptions>>()                
                .AddTransient(p =>
                {
                    var settings = p.GetRequiredService<IOptions<BackchannelAuthenticationUserNotificationServiceOptions>>().Value;

                    var serviceType = GetBackchannelAuthenticationUserNotificationServiceType(settings);
                    var constructor = serviceType.GetConstructors()[0];
                    var parameters = constructor.GetParameters();
                    var arguments = new object[parameters.Length];
                    for (var i = 0; i < parameters.Length; i++)
                    {
                        var parameterType = parameters[i].ParameterType;
                        if (parameterType == typeof(HttpClient))
                        {
                            var factory = p.GetRequiredService<IHttpClientFactory>();
                            arguments[i] = factory.CreateClient(settings.HttpClientName);
                            continue;
                        }
                        arguments[i] = p.GetRequiredService(parameterType);
                    }

                    return constructor.Invoke(arguments) as IBackchannelAuthenticationUserNotificationService;
                })
                .AddTransient<IBackchannelAuthenticationUserValidator, BackchannelAuthenticationUserValidator>()
                .AddTransient<OAuthDelegatingHandler<BackchannelAuthenticationUserNotificationServiceOptions>>()
                .AddHttpClient(configuration.GetValue<string>("HttpClientName"))
                .ConfigurePrimaryHttpMessageHandler(p => p.GetRequiredService<HttpClientHandler>())
                .AddHttpMessageHandler<OAuthDelegatingHandler<BackchannelAuthenticationUserNotificationServiceOptions>>();

            return services;
        }

        private static Type GetBackchannelAuthenticationUserNotificationServiceType(BackchannelAuthenticationUserNotificationServiceOptions settings)
        {
            Type serviceType;
            if (!string.IsNullOrEmpty(settings.AssemblyPath))
            {
#pragma warning disable S3885 // "Assembly.Load" should be used
                var assembly = Assembly.LoadFrom(settings.AssemblyPath);
#pragma warning restore S3885 // "Assembly.Load" should be used
                serviceType = assembly.GetType(settings.ServiceType, true);
            }
            else
            {
                serviceType = Type.GetType(settings.ServiceType, true);
            }

            return serviceType;
        }
#endif
    }
}
