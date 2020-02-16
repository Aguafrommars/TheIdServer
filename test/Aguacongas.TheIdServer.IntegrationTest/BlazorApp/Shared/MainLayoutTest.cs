using Aguacongas.IdentityServer.Store;
using Aguacongas.TheIdServer.Blazor.Oidc;
using Aguacongas.TheIdServer.BlazorApp;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Xunit;
using Xunit.Abstractions;
using blazorApp = Aguacongas.TheIdServer.BlazorApp;

namespace Aguacongas.TheIdServer.IntegrationTest.BlazorApp.Shared
{
    public class MainLayoutTest : IDisposable
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public MainLayoutTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void WhenNoUser_should_authomaticaly_signin()
        {
            var jsRuntimeMock = new Mock<IJSRuntime>();
            var navigationInterceptionMock = new Mock<INavigationInterception>();
            using var host = new TestHost();
            host.ConfigureServices(services =>
            {
                blazorApp.Program.ConfigureServices(services);                
                services
                    .AddLogging(configure =>
                    {
                        configure.AddProvider(new TestLoggerProvider(_testOutputHelper));
                    })
                    .AddSingleton<NavigationManager, TestNavigationManager>()
                    .AddSingleton(p => jsRuntimeMock.Object)
                    .AddSingleton(p => navigationInterceptionMock.Object);
            });

            var httpMock = host.AddMockHttp();
            var settingsRequest = httpMock.Capture("/settings.json");
            var discoveryRequest = httpMock.Capture("https://exemple.com/.well-known/openid-configuration");
            
            var component = host.AddComponent<App>();

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
                discoveryRequest.SetResult(new
                {
                    issuer = "https://exemple.com",
                });
            });

            markup = component.GetMarkup();
            Assert.Contains("You are redirecting to the login page. please wait", markup);
        }

        [Fact]
        public void WhenNoAuthorized_should_display_message()
        {
            var component = CreateComponent("test", new List<SerializableClaim>());

            var markup = component.GetMarkup();
            Assert.Contains("You're not authorize to use this application.", markup);
        }

        [Fact]
        public void WhenAuthorized_should_display_welcome_message()
        {
            var expected = "Bob Smith";
            var component = CreateComponent(expected, new List<SerializableClaim>
            {
                new SerializableClaim
                {
                    Type = "role",
                    Value = SharedConstants.READER
                }
            });

            var markup = component.GetMarkup();
            Assert.Contains(expected, markup);
        }

        private RenderedComponent<App> CreateComponent(string userName, List<SerializableClaim> claims)
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
            var host = new TestHost();
            _host = host;
            host.ConfigureServices(services =>
            {
                blazorApp.Program.ConfigureServices(services);
                services.AddLogging(configure =>
                {
                    configure.AddProvider(new TestLoggerProvider(_testOutputHelper));
                })
                    .AddSingleton<NavigationManager, TestNavigationManager>()
                    .AddSingleton(p => jsRuntimeMock.Object)
                    .AddSingleton(p => navigationInterceptionMock.Object);
            });

            var httpMock = host.AddMockHttp();
            var settingsRequest = httpMock.Capture("/settings.json");

            var component = host.AddComponent<App>();
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

            return component;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls
        private TestHost _host;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _host?.Dispose();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
