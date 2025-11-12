using Aguacongas.IdentityServer.Store;
using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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

            services.ConfigureServices(configuration, new Models.Settings
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

        [Fact]
        public void ConfigureLogging_should_add_filter_and_set_minimum_level()
        {
            // Arrange
            var services = new ServiceCollection();
            var mockLoggingBuilder = new Mock<ILoggingBuilder>();
            mockLoggingBuilder.SetupGet(x => x.Services).Returns(services);

            var settings = new Models.Settings
            {
                LoggingOptions = new Models.LoggingOptions
                {
                    Minimum = LogLevel.Warning,
                    Filters =
                    [
                        new Models.LoggingFilter
                        {
                            Category = "A",
                            Level = LogLevel.Error
                        }
                    ]
                }
            };

            // Act
            mockLoggingBuilder.Object.ConfigureLogging(settings);

            // Assert
            Assert.Contains(services, s => s.Lifetime == ServiceLifetime.Singleton);
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
