using Aguacongas.TheIdServer.Blazor.Oidc;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.TheIdServer.BlazorApp.Test.Pages
{
    public class ApiTest
    {
        [Fact]
        public void WhenNonAdmin_should_disable_inputs()
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
            var httpMock = host.AddMockHttp();
            var settingsRequest = httpMock.Capture("/settings.json");

            host.ConfigureServices(services =>
            {
                new Startup().ConfigureServices(services);
                services.AddIdentityServer4HttpStores(p => Task.FromResult(httpMock.ToHttpClient()))
                    .AddSingleton<NavigationManager>(p => new TestNavigationManager(uri: "http://exemple.com/protectresource"))
                    .AddSingleton(p => jsRuntimeMock.Object)
                    .AddSingleton(p => navigationInterceptionMock.Object);
            });


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

            host.WaitForNextRender(() => { });

            var inputs = component.FindAll("input")
                .Where(i => !i.Attributes.Any(a => a.Name == "class" && a.Value.Contains("new-claim")));
            Assert.All(inputs, input => input.Attributes.Any(a => a.Name == "disabled"));
        }
    }
}
