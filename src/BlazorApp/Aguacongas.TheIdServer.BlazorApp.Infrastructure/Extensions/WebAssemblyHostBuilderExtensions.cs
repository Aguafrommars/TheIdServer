// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
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
using System;
using System.Net.Http;
using System.Threading.Tasks;
using dn = Aguacongas.DynamicConfiguration.Razor.Services;
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
            ConfigureServices(builder.Services, configuration, settings);
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

        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration, Settings settings)
        {
            services.Configure((Action<RemoteAuthenticationApplicationPathsOptions>)(options => configuration.GetSection("AuthenticationPaths").Bind(options)))
                .AddOidcAuthentication(options =>
                {
                    SetDefaultAuthenticationOptions(options);
                    configuration.Bind(options);
                })
                .AddAccountClaimsPrincipalFactory<ClaimsPrincipalFactory>();

            services.Configure<MenuOptions>(options => configuration.GetSection(nameof(MenuOptions)).Bind(options))
                .AddScoped<dn.ConfigurationService>()
                .AddScoped<dn.IConfigurationService, ConfigurationService>()
                .AddConfigurationService(configuration.GetSection("settingsOptions"))
                .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            services.AddAuthorizationCore(options =>
            {
                options.AddIdentityServerPolicies(showSettings: configuration.GetValue<bool>($"{nameof(MenuOptions)}:{nameof(MenuOptions.ShowSettings)}"));
            });

            services.AddAdminHttpStores(p =>
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

        // Workaroun https://github.com/dotnet/aspnetcore/issues/41998#issuecomment-1144812047
        private static void SetDefaultAuthenticationOptions(RemoteAuthenticationOptions<OidcProviderOptions> options)
        {
            options.AuthenticationPaths.RemoteRegisterPath = "/identity/account/register";
            options.AuthenticationPaths.RemoteProfilePath = "/identity/account/manage";
            options.UserOptions.RoleClaim = "role";
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
