// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.AspNetCore.Authentication;
using Aguacongas.IdentityServer.Admin.Http.Store;
using Aguacongas.IdentityServer.Store;
using Aguacongas.TheIdServer.BlazorApp.Infrastructure.Services;
using Aguacongas.TheIdServer.BlazorApp.Models;
using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Entity = Aguacongas.IdentityServer.Store.Entity;


namespace Microsoft.AspNetCore.Components.WebAssembly.Hosting
{
    public static class WebAssemblyHostBuilderExtensions
    {
        public static WebAssemblyHostBuilder AddTheIdServerApp(this WebAssemblyHostBuilder builder)
        {
            var configuration = builder.Configuration;
            var settings = configuration.Get<Settings>();
            ConfigureLogging(builder.Logging, settings);
            ConfigureServices(builder.Services, configuration, settings, builder.HostEnvironment.BaseAddress);
            return builder;
        }

        private static void ConfigureLogging(ILoggingBuilder logging, Settings settings)
        {
            var options = settings.LoggingOptions;
            var filters = options.Filters;
            if (filters != null)
            {
                foreach (var filter in filters)
                {
                    logging.AddFilter(filter.Category, filter.Level);
                }
            }
            logging.SetMinimumLevel(options.Minimum);
        }

        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration, Settings settings, string baseAddress)
        {
            services.Configure<RemoteAuthenticationApplicationPathsOptions>(options => configuration.GetSection("AuthenticationPaths").Bind(options))
                .AddOidcAuthentication(options =>
                {
                    configuration.GetSection("AuthenticationPaths").Bind(options.AuthenticationPaths);
                    configuration.GetSection("UserOptions").Bind(options.UserOptions);
                    configuration.Bind("ProviderOptions", options.ProviderOptions);
                })
                .AddAccountClaimsPrincipalFactory<RemoteAuthenticationState, RemoteUserAccount, ClaimsPrincipalFactory>();

            services.AddConfigurationService(configuration.GetSection("settingsOptions"))
                .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            var postConfigurationOidc = services.First(s => s.ServiceType == typeof(IPostConfigureOptions<RemoteAuthenticationOptions<OidcProviderOptions>>));
            
            services.Remove(postConfigurationOidc);
            services.Add(new ServiceDescriptor(postConfigurationOidc.ServiceType, postConfigurationOidc.ImplementationType, ServiceLifetime.Singleton));

            services.AddAuthorizationCore(options =>
                {
                    options.AddIdentityServerPolicies();
                });

            services.AddTransient(p => new HttpClient { BaseAddress = new Uri(baseAddress) })
                .AddAdminHttpStores(p =>
                {
                    return Task.FromResult(CreateApiHttpClient(p));
                })
                .AddAdminApplication(settings)
                .AddHttpClient("oidc")
                .ConfigureHttpClient(httpClient =>
                {
                    var apiUri = new Uri(settings.ApiBaseUrl);
                    httpClient.BaseAddress = apiUri;
                })
                .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            services.AddScoped<ISharedStringLocalizerAsync, StringLocalizer>()
                .AddTransient<IReadOnlyLocalizedResourceStore>(p =>
                {
                    var factory = p.GetRequiredService<IHttpClientFactory>();
                    return new ReadOnlyLocalizedResourceStore(new AdminStore<Entity.LocalizedResource>(Task.FromResult(factory.CreateClient("localizer")), p.GetRequiredService<ILogger<AdminStore<Entity.LocalizedResource>>>()));
                })
                .AddTransient<IReadOnlyCultureStore>(p =>
                {
                    var factory = p.GetRequiredService<IHttpClientFactory>();
                    return new ReadOnlyCultureStore(new AdminStore<Entity.Culture>(Task.FromResult(factory.CreateClient("localizer")), p.GetRequiredService<ILogger<AdminStore<Entity.Culture>>>()));
                })
                .AddHttpClient("localizer")
                .ConfigureHttpClient(httpClient =>
                {
                    var apiUri = new Uri(settings.ApiBaseUrl);
                    httpClient.BaseAddress = apiUri;
                });
        }

        public static IServiceCollection AddAdminApplication(this IServiceCollection services, Settings settings)
        {
            return services.AddScoped(p => settings)
                .AddScoped<Notifier>()
                .AddScoped<IAuthenticationSchemeOptionsSerializer, AuthenticationSchemeOptionsSerializer>()                
                .AddTransient<IAdminStore<User>, UserAdminStore>()
                .AddTransient<IAdminStore<Role>, RoleAdminStore>()
                .AddTransient<IAdminStore<ExternalProvider>, ExternalProviderStore>()
                .AddTransient(typeof(IStringLocalizerAsync<>), typeof(StringLocalizer<>))
                .AddTransient<OneTimeTokenService>();
        }

        private static HttpClient CreateApiHttpClient(IServiceProvider p)
        {
            return p.GetRequiredService<IHttpClientFactory>()
                                    .CreateClient("oidc");
        }

    }
}
