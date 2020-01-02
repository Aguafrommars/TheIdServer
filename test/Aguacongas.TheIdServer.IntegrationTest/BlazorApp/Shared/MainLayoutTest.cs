using Aguacongas.TheIdServer.Blazor.Oidc;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Moq;
using System;
using System.Collections.Generic;
using System.Text.Json;
using Xunit;
using blazorApp = Aguacongas.TheIdServer.BlazorApp;

namespace Aguacongas.TheIdServer.IntegrationTest.BlazorApp.Shared
{
    public class MainLayoutTest
    {
        [Fact]
        public void WhenNoUser_should_authomaticaly_signin()
        {
            var jsRuntimeMock = new Mock<IJSRuntime>();
            var navigationInterceptionMock = new Mock<INavigationInterception>();
            var host = new TestHost();
            host.ConfigureServices(services =>
            {
                new blazorApp.Startup().ConfigureServices(services);
                services.AddSingleton<NavigationManager, TestNavigationManager>()
                    .AddSingleton(p => jsRuntimeMock.Object)
                    .AddSingleton(p => navigationInterceptionMock.Object);
            });

            var httpMock = host.AddMockHttp();
            var settingsRequest = httpMock.Capture("/settings.json");
            var discoveryRequest = httpMock.Capture("https://exemple.com/.well-known/openid-configuration");
            
            var component = host.AddComponent<blazorApp.App>();

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
            var options = new AuthorizationOptions();
            var jsRuntimeMock = new Mock<IJSRuntime>();
            jsRuntimeMock.Setup(m => m.InvokeAsync<string>("sessionStorage.getItem", new object[] { options.ExpireAtStorageKey }))
                .ReturnsAsync(DateTime.Now.ToString());
            jsRuntimeMock.Setup(m => m.InvokeAsync<string>("sessionStorage.getItem", new object[] { options.TokensStorageKey }))
                .ReturnsAsync(JsonSerializer.Serialize(new Tokens
                {
                    AccessToken = "test",
                    TokenType = "Bearer"
                }));
            jsRuntimeMock.Setup(m => m.InvokeAsync<string>("sessionStorage.getItem", new object[] { options.ClaimsStorageKey }))
                .ReturnsAsync(JsonSerializer.Serialize(new List<SerializableClaim>
                {
                    new SerializableClaim
                    {
                        Type = "name",
                        Value = "test"
                    }
                }));

            var navigationInterceptionMock = new Mock<INavigationInterception>();
            var host = new TestHost();
            host.ConfigureServices(services =>
            {
                new blazorApp.Startup().ConfigureServices(services);
                services.AddSingleton<NavigationManager, TestNavigationManager>()
                    .AddSingleton(p => jsRuntimeMock.Object)
                    .AddSingleton(p => navigationInterceptionMock.Object);
            });

            var httpMock = host.AddMockHttp();
            var settingsRequest = httpMock.Capture("/settings.json");

            var component = host.AddComponent<blazorApp.App>();

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

            markup = component.GetMarkup();
            Assert.Contains("You're not authorize to use this application.", markup);
        }

        [Fact]
        public void WhenAuthorized_should_display_welcome_message()
        {
            var options = new AuthorizationOptions();
            var jsRuntimeMock = new Mock<IJSRuntime>();
            jsRuntimeMock.Setup(m => m.InvokeAsync<string>("sessionStorage.getItem", new object[] { options.ExpireAtStorageKey }))
                .ReturnsAsync(DateTime.Now.ToString());
            jsRuntimeMock.Setup(m => m.InvokeAsync<string>("sessionStorage.getItem", new object[] { options.TokensStorageKey }))
                .ReturnsAsync(JsonSerializer.Serialize(new Tokens
                {
                    AccessToken = "test",
                    TokenType = "Bearer"
                }));
            jsRuntimeMock.Setup(m => m.InvokeAsync<string>("sessionStorage.getItem", new object[] { options.ClaimsStorageKey }))
                .ReturnsAsync(JsonSerializer.Serialize(new List<SerializableClaim>
                {
                    new SerializableClaim
                    {
                        Type = "name",
                        Value = "Bod Smith"
                    }
                }));

            var navigationInterceptionMock = new Mock<INavigationInterception>();
            var host = new TestHost();
            host.ConfigureServices(services =>
            {
                new blazorApp.Startup().ConfigureServices(services);
                services.AddSingleton<NavigationManager, TestNavigationManager>()
                    .AddSingleton(p => jsRuntimeMock.Object)
                    .AddSingleton(p => navigationInterceptionMock.Object);
            });

            var httpMock = host.AddMockHttp();
            var settingsRequest = httpMock.Capture("/settings.json");

            var component = host.AddComponent<blazorApp.App>();

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

            host.WaitForNextRender(() => { });

            markup = component.GetMarkup();
            Assert.Contains("Bod Smith", markup);
        }
    }
}
