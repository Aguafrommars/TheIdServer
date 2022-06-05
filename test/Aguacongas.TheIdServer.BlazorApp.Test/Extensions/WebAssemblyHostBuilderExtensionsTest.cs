using Aguacongas.IdentityServer.Store;
using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Moq;
using System.Net.Http;
using Xunit;

namespace Aguacongas.TheIdServer.BlazorApp.Test.Extensions
{
    public class WebAssemblyHostBuilderExtensionsTest
    {
        [Fact]
        public void ConfigureServices_should_register_di_services()
        {
            var jsRuntimeMock = new Mock<IJSRuntime>();

            var configuration = new ConfigurationManager();
            var services = new ServiceCollection()
                .AddTransient<NavigationManager>(p => new NavManager())
                .AddTransient(p => jsRuntimeMock.Object);

            WebAssemblyHostBuilderExtensions.ConfigureServices(services, configuration, new Models.Settings
            {
                ApiBaseUrl = "https://exemple.com/"
            });

            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateAsyncScope();
            
            var provider = scope.ServiceProvider;

            Assert.NotNull(provider.GetRequiredService<IAuthorizationService>());

            Assert.NotNull(provider.GetRequiredService<IExternalProviderKindStore>());

            Assert.NotNull(provider.GetRequiredService<IReadOnlyLocalizedResourceStore>());

            Assert.NotNull(provider.GetRequiredService<IReadOnlyCultureStore>());

            var factory = provider.GetRequiredService<IHttpClientFactory>();

            Assert.NotNull(factory.CreateClient("oidc"));
            Assert.NotNull(factory.CreateClient("localizer"));
        }

        class NavManager : NavigationManager
        {
            public NavManager()
            {
                Initialize("https://exemple.com/", "https://exemple.com/");
            }
        }
    }
}
