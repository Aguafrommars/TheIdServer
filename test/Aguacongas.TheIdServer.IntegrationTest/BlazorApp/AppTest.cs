using Aguacongas.TheIdServer.Blazor.Oidc;
using Aguacongas.TheIdServer.Data;
using Aguacongas.TheIdServer.Models;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Moq;
using RichardSzalay.MockHttp;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using blazorApp = Aguacongas.TheIdServer.BlazorApp;

namespace Aguacongas.TheIdServer.IntegrationTest.BlazorApp
{
    public class AppTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public AppTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task FullLoginTest()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            var connectionApp = new SqliteConnection("DataSource=:memory:");
            connectionApp.Open();
            var testLoggerProvider = new TestLoggerProvider(_testOutputHelper);
            var server = TestUtils.CreateTestServer(
                // We use Sqlite in memory mode for tests. https://docs.microsoft.com/en-us/ef/core/miscellaneous/testing/sqlite
                services =>
                {
                    services.AddLogging(configure => configure.AddProvider(testLoggerProvider))
                    .AddDbContext<ApplicationDbContext>(options =>
                        options.UseSqlite(connectionApp))
                    .AddIdentityServer4EntityFrameworkStores<ApplicationUser, ApplicationDbContext>(options =>
                        options.UseSqlite(connection))
                    .AddIdentityProviderStore();
                });

            using var scope = server.Host.Services.CreateScope();
            SeedData.SeedUsers(scope);
            SeedData.SeedConfiguration(scope);

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
                        configure.AddProvider(testLoggerProvider);
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

            var document = new HtmlDocument();
            document.LoadHtml(loginPageContent);
            var form = document.DocumentNode.SelectSingleNode("//body//form");
            Assert.NotNull(form);
            var inputs = form.ParentNode.SelectNodes("input");
            Assert.NotEmpty(inputs);

            var redirectUrl = inputs.FirstOrDefault(n => n.Attributes.Any(a => a.Value == "ReturnUrl"))?
                .Attributes?
                .FirstOrDefault(a => a.Name == "value")?
                .Value;

            Assert.NotNull(redirectUri);
            var validationToken = inputs.FirstOrDefault(n => n.Attributes.Any(a => a.Value == "__RequestVerificationToken"))?
                .Attributes?
                .FirstOrDefault(a => a.Name == "value")?
                .Value;

            Assert.NotNull(validationToken);

            var postContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["Username"] = "Alice",
                ["Password"] = "Password123",
                ["ReturnUrl"] = redirectUrl,
                ["__RequestVerificationToken"] = validationToken,
                ["RememberLogin"] = "false"
            });

            var postLoginResponse = await httpClient.PostAsync(redirectResponse.Headers.Location, postContent);
            var postLoginContent = await postLoginResponse.Content.ReadAsStringAsync();
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
