using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.TheIdServer.Blazor.Oidc;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Moq;
using RichardSzalay.MockHttp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using blazorApp = Aguacongas.TheIdServer.BlazorApp;

namespace Aguacongas.TheIdServer.IntegrationTest.BlazorApp
{
    [Collection("api collection")]
    public class AppTest
    {
        private readonly ApiFixture _fixture;

        public AppTest(ApiFixture fixture, ITestOutputHelper testOutputHelper)
        {
            _fixture = fixture;
            _fixture.TestOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task FullLoginTest()
        {
            var server = TestUtils.CreateTestServer();
            using var scope = server.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IdentityServerDbContext>();
            context.Database.EnsureDeleted();

            var config = server.Services.GetRequiredService<IConfiguration>();
            var connectionString = config.GetConnectionString("DefaultConnection");
            SeedData.EnsureSeedData(connectionString);

            var host = new TestHost();
            var options = new AuthorizationOptions();
            var jsRuntimeMock = new Mock<IJSRuntime>();
            var navigationInterceptionMock = new Mock<INavigationInterception>();
            var navigationManager = new TestNavigationManager(uri: "http://exemple.com");

            var httpClient = server.CreateClient();
            options.Authority = httpClient.BaseAddress.ToString();

            host.ConfigureServices(services =>
            {
                new blazorApp.Startup().ConfigureServices(services);
                services
                    .AddLogging(configure =>
                    {
                        configure.AddProvider(new TestLoggerProvider(_fixture.TestOutputHelper));
                    })
                    .AddIdentityServer4HttpStores(p => Task.FromResult(httpClient))
                    .AddSingleton(p => navigationManager)
                    .AddSingleton<NavigationManager>(p => p.GetRequiredService<TestNavigationManager>())
                    .AddSingleton(p => jsRuntimeMock.Object)
                    .AddSingleton(p => navigationInterceptionMock.Object);
            });

            var httpMock = host.AddMockHttp();
            var waitHandle = new ManualResetEvent(false);
            httpMock.Fallback.Respond(httpClient);
            jsRuntimeMock.Setup(m => m.InvokeAsync<object>("sessionStorage.setItem", It.IsAny<object[]>()))
                .ReturnsAsync(null);

            string redirectUri = null;
            navigationManager.OnNavigateToCore = (uri, f) =>
            {
                redirectUri = uri;
                waitHandle.Set();
            };

            var settingsRequest = httpMock.Capture("/settings.json");
            var component = host.AddComponent<blazorApp.App>();

            var markup = component.GetMarkup();
            Assert.Contains("Authentication in progress", markup);

            host.WaitForNextRender(() =>
            {
                settingsRequest.SetResult(new
                {
                    ApiBaseUrl = $"{options.Authority}api",
                    options.Authority,
                    ClientId = "theidserveradmin",
                    Scope = "openid profile theidserveradminapi"
                });
            });

            host.WaitForContains(component, "You are redirecting to the login page. please wait");

            WaitForHttpResponse(waitHandle);

            var redirectResponse = await httpClient.GetAsync(redirectUri);
            Assert.Equal(HttpStatusCode.Redirect, redirectResponse.StatusCode);


            var loginPageResponse = await httpClient.GetAsync(redirectResponse.Headers.Location);
            var loginPageContent = await loginPageResponse.Content.ReadAsStringAsync();

            Assert.Contains("<form", loginPageContent);
        }

        private static void WaitForHttpResponse(ManualResetEvent waitHandle)
        {
            if (Debugger.IsAttached)
            {
                waitHandle.WaitOne();
            }
            else
            {
                waitHandle.WaitOne(5000);
            }
        }
    }
}
