using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.Blazor.Oidc;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Moq;
using RichardSzalay.MockHttp;
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
            CreateTestHost("Bob Smith",
                null,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler mockHttp);

            host.WaitForNextRender(() => { });

            var inputs = component.FindAll("input")
                .Where(i => !i.Attributes.Any(a => a.Name == "class" && a.Value.Contains("new-claim")));
            Assert.All(inputs, input => input.Attributes.Any(a => a.Name == "disabled"));
        }

        [Fact]
        public void WhenAdmin_should_enable_inputs()
        {
            CreateTestHost("Alice Smith",
                null,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler mockHttp);

            host.WaitForNextRender(() => { });

            var inputs = component.FindAll("input")
                .Where(i => !i.Attributes.Any(a => a.Name == "class" && a.Value.Contains("new-claim")));
            Assert.DoesNotContain(inputs, input => input.Attributes.Any(a => a.Name == "disabled"));
        }

        [Fact]
        public void OnFilterChanged_should_filter_properties_scopes_scopeClaims_and_secret()
        {
            CreateTestHost("Alice Smith",
                "test",
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler mockHttp);

            var apiRequest = mockHttp.Capture("http://exemple.com/api/protectresource/test");

            host.WaitForNextRender(() => 
            {
                apiRequest.SetResult(new ProtectResource
                {
                    ApiClaims = new List<ApiClaim>
                    {
                        new ApiClaim { Type = "filtered" }
                    },                    
                    Id = "test",
                    Properties = new List<ApiProperty>
                    {
                        new ApiProperty { Key = "filtered", Value = "filtered" }
                    },
                    Scopes = new List<ApiScope>
                    {
                       new ApiScope
                       {
                           Scope = "test",
                           ApiScopeClaims = new List<ApiScopeClaim>
                           {
                               new ApiScopeClaim { Type = "filtered" }
                           }
                       },
                       new ApiScope
                       {
                           Scope = "filtered",
                           ApiScopeClaims = new List<ApiScopeClaim>()
                       }
                    },
                    Secrets = new List<ApiSecret>
                    {
                        new ApiSecret { Value = "filtered" }
                    }
                });
            });

            var markup = component.GetMarkup();

            Assert.Contains("Loading...", markup);

            host.WaitForNextRender(() => { });

            markup = component.GetMarkup();

            Assert.Contains("filtered", markup);

            var filterInput = component.Find("input[placeholder=\"filter\"]");

            Assert.NotNull(filterInput);

            host.WaitForNextRender(() => filterInput.TriggerEventAsync("oninput", new ChangeEventArgs
            {
                Value = "test"
            }));

            markup = component.GetMarkup();

            Assert.DoesNotContain("filtered", markup);
        }

        private static void CreateTestHost(string userName, 
            string id,
            out TestHost host,
            out RenderedComponent<App> component,
            out MockHttpMessageHandler mockHttp)
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
                        Value = userName
                    }
                }));

            var navigationInterceptionMock = new Mock<INavigationInterception>();

            host = new TestHost();
            var httpMock = host.AddMockHttp();
            mockHttp = httpMock;
            var settingsRequest = httpMock.Capture("/settings.json");
            host.ConfigureServices(services =>
            {
                new Startup().ConfigureServices(services);
                var httpClient = httpMock.ToHttpClient();
                httpClient.BaseAddress = new Uri("http://exemple.com/api");
                services.AddIdentityServer4HttpStores(p => Task.FromResult(httpClient))
                    .AddSingleton<NavigationManager>(p => new TestNavigationManager(uri: $"http://exemple.com/protectresource/{id}"))
                    .AddSingleton(p => jsRuntimeMock.Object)
                    .AddSingleton(p => navigationInterceptionMock.Object);
            });

            component = host.AddComponent<App>();

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
