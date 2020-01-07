using Aguacongas.TheIdServer.Blazor.Oidc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Moq;
using RichardSzalay.MockHttp;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using blazorApp = Aguacongas.TheIdServer.BlazorApp;

namespace Aguacongas.TheIdServer.IntegrationTest
{
    public static class TestUtils
    {

        public static TestServer CreateTestServer(
                    Action<IServiceCollection> configureServices = null,
                    IEnumerable<KeyValuePair<string, string>> configurationOverrides = null)
        {
            Startup startup = null;
            var webHostBuilder = new WebHostBuilder()
                .UseEnvironment("Development")
                .UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration
                    .ReadFrom.Configuration(hostingContext.Configuration))
                .ConfigureAppConfiguration(builder =>
                {
                    builder.AddJsonFile(Path.Combine(Environment.CurrentDirectory, @"..\..\..\..\..\src\Aguacongas.TheIdServer\appsettings.json"));
                    builder.AddJsonFile(Path.Combine(Environment.CurrentDirectory, @"..\..\..\appsettings.Test.json"), true);
                    if (configurationOverrides != null)
                    {
                        builder.AddInMemoryCollection(configurationOverrides);
                    }
                })
                .ConfigureServices((context, services) =>
                {
                    startup = new Startup(context.Configuration, context.HostingEnvironment);
                    configureServices?.Invoke(services);
                    startup.ConfigureServices(services);
                    services.AddSingleton<TestUserService>()
                        .AddMvc().AddApplicationPart(startup.GetType().Assembly);
                    configureServices?.Invoke(services);
                })
                .Configure(builder =>
                {
                    builder.Use(async (context, next) =>
                    {
                        var testService = context.RequestServices.GetRequiredService<TestUserService>();
                        context.User = testService.User;
                        await next();
                    });
                    startup.Configure(builder);
                });

            var testServer = new TestServer(webHostBuilder);

            return testServer;
        }

        public static void CreateTestHost(string userName,
            List<SerializableClaim> claims,
            string url,
            TestServer sut,
            ITestOutputHelper testOutputHelper,
            out TestHost host,
            out RenderedComponent<blazorApp.App> component,
            out MockHttpMessageHandler mockHttp)
        {
            var options = new AuthorizationOptions();
            var jsRuntimeMock = new Mock<IJSRuntime>();
            jsRuntimeMock.Setup(m => m.InvokeAsync<string>("sessionStorage.getItem", new object[] { options.ExpireAtStorageKey }))
                .ReturnsAsync(DateTime.UtcNow.AddHours(1).ToString());
            jsRuntimeMock.Setup(m => m.InvokeAsync<string>("sessionStorage.getItem", new object[] { options.TokensStorageKey }))
                .ReturnsAsync(JsonSerializer.Serialize(new Tokens
                {
                    AccessToken = "test",
                    TokenType = "Bearer"
                }));
            
            if (!claims.Any(c => c.Type == "name"))
            {
                claims.Add(new SerializableClaim
                {
                    Type = "name",
                    Value = userName
                });
            }
            jsRuntimeMock.Setup(m => m.InvokeAsync<string>("sessionStorage.getItem", new object[] { options.ClaimsStorageKey }))
                .ReturnsAsync(JsonSerializer.Serialize(claims));

            var navigationInterceptionMock = new Mock<INavigationInterception>();

            host = new TestHost();
            var httpMock = host.AddMockHttp();
            mockHttp = httpMock;
            var settingsRequest = httpMock.Capture("/settings.json");
            host.ConfigureServices(services =>
            {
                new blazorApp.Startup().ConfigureServices(services);
                var httpClient = sut.CreateClient();
                httpClient.BaseAddress = new Uri(httpClient.BaseAddress, "api");
                sut.Services.GetRequiredService<TestUserService>()
                    .SetTestUser(true, claims.Select(c => new Claim(c.Type, c.Value)));

                services
                    .AddLogging(configure =>
                    {
                        configure.AddProvider(new TestLoggerProvider(testOutputHelper));
                    })
                    .AddIdentityServer4HttpStores(p => Task.FromResult(httpClient))
                    .AddSingleton(p => new TestNavigationManager(uri: url))
                    .AddSingleton<NavigationManager>(p => p.GetRequiredService<TestNavigationManager>())
                    .AddSingleton(p => jsRuntimeMock.Object)
                    .AddSingleton(p => navigationInterceptionMock.Object);
            });

            component = host.AddComponent<blazorApp.App>();

            var markup = component.GetMarkup();
            Assert.Contains("Authentication in progress", markup);

            host.WaitForNextRender(() =>
            {
                settingsRequest.SetResult(new AuthorizationOptions
                {
                    Authority = "https://exemple.com",
                    ClientId = "test",
                    Scope = "openid profile apitest"
                });
            });
        }
    }

}
