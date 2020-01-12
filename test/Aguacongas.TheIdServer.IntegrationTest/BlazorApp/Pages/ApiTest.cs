using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.BlazorApp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Testing;
using Microsoft.EntityFrameworkCore;
using RichardSzalay.MockHttp;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Aguacongas.TheIdServer.IntegrationTest.BlazorApp.Pages
{
    [Collection("api collection")]
    public class ApiTest : EntityPageTestBase
    {
        public override string Entity => "protectresource";
        public ApiTest(ApiFixture fixture, ITestOutputHelper testOutputHelper):base(fixture, testOutputHelper)
        {
        }



        [Fact]
        public async Task OnFilterChanged_should_filter_properties_scopes_scopeClaims_and_secret()
        {
            string apiId = await CreateApi();

            CreateTestHost("Alice Smith",
                AuthorizationOptionsExtensions.WRITER,
                apiId,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler mockHttp);

            string markup = WaitForLoaded(host, component);

            Assert.Contains("filtered", markup);

            var filterInput = component.Find("input[placeholder=\"filter\"]");

            Assert.NotNull(filterInput);

            host.WaitForNextRender(async () => await filterInput.TriggerEventAsync("oninput", new ChangeEventArgs
            {
                Value = apiId
            }));

            markup = component.GetMarkup();

            Assert.DoesNotContain("filtered", markup);
        }

        [Fact]
        public async Task SaveClicked_should_update_api()
        {
            string apiId = await CreateApi();

            CreateTestHost("Alice Smith",
                AuthorizationOptionsExtensions.WRITER,
                apiId,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler mockHttp);

            WaitForLoaded(host, component);

            var input = component.Find("#displayName");

            Assert.NotNull(input);

            var expected = GenerateId();
            host.WaitForNextRender(() => input.Change(expected));

            var markup = component.GetMarkup();

            Assert.Contains(expected, markup);

            var form = component.Find("form");

            Assert.NotNull(form);

            host.WaitForNextRender(() => form.Submit());

            WaitForSavedToast(host, component);

            await DbActionAsync<IdentityServerDbContext>(async context =>
            {
                var api = await context.Apis.FirstOrDefaultAsync(a => a.Id == apiId);
                Assert.Equal(expected, api.DisplayName);
            });

        }

        private async Task<string> CreateApi()
        {
            var apiId = GenerateId();
            await DbActionAsync<IdentityServerDbContext>(context =>
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
            return apiId;
        }

        private static string WaitForLoaded(TestHost host, RenderedComponent<App> component)
        {
            host.WaitForNextRender();

            var markup = component.GetMarkup();

            while (markup.Contains("Loading..."))
            {
                host.WaitForNextRender();
                markup = component.GetMarkup();
            }

            return markup;
        }
    }
}
