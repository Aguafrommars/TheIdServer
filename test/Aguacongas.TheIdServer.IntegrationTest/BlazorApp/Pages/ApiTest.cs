using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.Blazor.Oidc;
using Aguacongas.TheIdServer.IntegrationTest;
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
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.TheIdServer.BlazorApp.Test.Pages
{
    [Collection("api collection")]
    public class ApiTest
    {
        private readonly ApiFixture _fixture;

        public ApiTest(ApiFixture fixture)
        {
            _fixture = fixture;
        }


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
        public async Task OnFilterChanged_should_filter_properties_scopes_scopeClaims_and_secret()
        {
            var apiId = GenerateId();
            await _fixture.DbActionAsync<IdentityServerDbContext>(context =>
            {
                context.Apis.Add(new ProtectResource
                {
                    Id = apiId,
                    DisplayName = apiId,
                    ApiClaims = new List<ApiClaim>
                    {
                        new ApiClaim { Id = GenerateId(), Type = "filtered" }
                    },
                    Properties = new List<ApiProperty>
                    {
                        new ApiProperty { Id = GenerateId(), Key = "filtered", Value = "filtered" }
                    },
                    Scopes = new List<ApiScope>
                    {
                       new ApiScope
                       {
                           Id = GenerateId(),
                           Scope = apiId,
                           DisplayName = "test",
                           ApiScopeClaims = new List<ApiScopeClaim>
                           {
                               new ApiScopeClaim { Id = GenerateId(), Type = "filtered" }
                           }
                       },
                       new ApiScope
                       {
                           Id = GenerateId(),
                           Scope = "filtered",
                           DisplayName = "filtered",
                           ApiScopeClaims = new List<ApiScopeClaim>()
                       }
                    },
                    Secrets = new List<ApiSecret>
                    {
                        new ApiSecret { Id = GenerateId(), Type="SHA256", Value = "filtered" }
                    }
                });

                return context.SaveChangesAsync();
            });

            CreateTestHost("Alice Smith",
                apiId,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler mockHttp);

            host.WaitForNextRender(() => 
            {
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
                Value = apiId
            }));

            markup = component.GetMarkup();

            Assert.DoesNotContain("filtered", markup);
        }

        private void CreateTestHost(string userName, 
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
                var sut = _fixture.Sut;
                var httpClient = sut.CreateClient();
                httpClient.BaseAddress = new Uri(httpClient.BaseAddress, "api");
                sut.Services.GetRequiredService<TestUserService>()
                    .SetTestUser(true, new Claim[] { new Claim("name", userName) });

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

        private static string GenerateId() => Guid.NewGuid().ToString();
    }
}
