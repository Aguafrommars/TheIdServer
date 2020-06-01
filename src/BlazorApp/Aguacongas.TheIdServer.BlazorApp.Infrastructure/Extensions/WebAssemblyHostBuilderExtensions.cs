using Aguacongas.AspNetCore.Authentication;
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
            services
                .AddOptions()
                .Configure<RemoteAuthenticationApplicationPathsOptions>(options => configuration.GetSection("AuthenticationPaths").Bind(options))
                .AddOidcAuthentication<RemoteAuthenticationState, RemoteUserAccount>(options =>
                {
                    configuration.GetSection("AuthenticationPaths").Bind(options.AuthenticationPaths);
                    configuration.GetSection("UserOptions").Bind(options.UserOptions);
                    configuration.Bind("ProviderOptions", options.ProviderOptions);
                })
                .AddAccountClaimsPrincipalFactory<RemoteAuthenticationState, RemoteUserAccount, ClaimsPrincipalFactory>();

            services.AddAuthorizationCore(options =>
            {
                options.AddIdentityServerPolicies();
            });


            services
                .AddIdentityServer4AdminHttpStores(p =>
                {
                    return Task.FromResult(CreateApiHttpClient(p));
                })
                .AddSingleton(new HttpClient { BaseAddress = new Uri(baseAddress) })
                .AddSingleton(p => settings)
                .AddSingleton<Notifier>()
                .AddSingleton<IAuthenticationSchemeOptionsSerializer, AuthenticationSchemeOptionsSerializer>()
                .AddTransient<IAdminStore<User>, UserAdminStore>()
                .AddTransient<IAdminStore<Role>, RoleAdminStore>()
                .AddTransient<IAdminStore<ExternalProvider>, ExternalProviderStore>()
                .AddSingleton(typeof(SharedStringLocalizer<>))
                .AddTransient(typeof(IStringLocalizerAsync<>), typeof(StringLocalizer<>))
                .AddHttpClient("oidc")
                .ConfigureHttpClient(httpClient =>
                {
                    var apiUri = new Uri(settings.ApiBaseUrl);
                    httpClient.BaseAddress = apiUri;
                })
                .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();
            services.AddHttpClient("localizer").ConfigureHttpClient(httpClient =>
            {
                var apiUri = new Uri(settings.ApiBaseUrl);
                httpClient.BaseAddress = apiUri;
            });
        }

        private static HttpClient CreateApiHttpClient(IServiceProvider p)
        {
            return p.GetRequiredService<IHttpClientFactory>()
                                    .CreateClient("oidc");
        }

    }
}
