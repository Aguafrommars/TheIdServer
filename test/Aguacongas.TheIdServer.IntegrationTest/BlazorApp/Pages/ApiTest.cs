using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.BlazorApp;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Testing;
using Microsoft.EntityFrameworkCore;
using RichardSzalay.MockHttp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Aguacongas.TheIdServer.IntegrationTest.BlazorApp.Pages
{
    [Collection("api collection")]
    public class ApiTest : EntityPageTestBase
    {
        public override string Entity => "protectresource";
        public ApiTest(ApiFixture fixture, ITestOutputHelper testOutputHelper) : base(fixture, testOutputHelper)
        {
        }

        [Fact]
        public async Task OnFilterChanged_should_filter_properties_scopes_scopeClaims_and_secret()
        {
            string apiId = await CreateApi();

            CreateTestHost("Alice Smith",
                SharedConstants.WRITER,
                apiId,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler mockHttp);

            string markup = WaitForLoaded(host, component);

            WaitForContains(host, component, "filtered");

            var filterInput = component.Find("input[placeholder=\"filter\"]");

            Assert.NotNull(filterInput);

            await host.WaitForNextRenderAsync(() => filterInput.TriggerEventAsync("oninput", new ChangeEventArgs
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
                SharedConstants.WRITER,
                apiId,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler mockHttp);

            WaitForLoaded(host, component);

            var input = host.WaitForNode(component, "#displayName");

            var expected = GenerateId();
            await host.WaitForNextRenderAsync(() => input.ChangeAsync(expected));

            var markup = component.GetMarkup();

            Assert.Contains(expected, markup);

            var form = component.Find("form");

            Assert.NotNull(form);

            await host.WaitForNextRenderAsync(() => form.SubmitAsync());

            WaitForSavedToast(host, component);

            await DbActionAsync<IdentityServerDbContext>(async context =>
            {
                var api = await context.Apis.FirstOrDefaultAsync(a => a.Id == apiId);
                Assert.Equal(expected, api.DisplayName);
            });
        }

        [Fact]
        public async Task SaveClicked_should_create_api()
        {
            var apiId = GenerateId();

            CreateTestHost("Alice Smith",
                SharedConstants.WRITER,
                null,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler mockHttp);

            WaitForLoaded(host, component);

            var input = WaitForNode(host, component, "#name");

            await host.WaitForNextRenderAsync(() => input.ChangeAsync(apiId));

            input = component.Find("#displayName");

            Assert.NotNull(input);

            await host.WaitForNextRenderAsync(() => input.ChangeAsync(apiId));

            input = component.Find("#scopes #scope");

            Assert.NotNull(input);

            await host.WaitForNextRenderAsync(() => input.ChangeAsync(apiId));

            input = component.Find("#scopes #displayName");

            Assert.NotNull(input);

            await host.WaitForNextRenderAsync(() => input.ChangeAsync(apiId));

            var form = component.Find("form");

            Assert.NotNull(form);

            await host.WaitForNextRenderAsync(() => form.SubmitAsync());

            WaitForSavedToast(host, component);

            await DbActionAsync<IdentityServerDbContext>(async context =>
            {
                var api = await context.Apis.FirstOrDefaultAsync(a => a.Id == apiId);
                Assert.Equal(apiId, api.DisplayName);
            });
        }

        [Fact]
        public async Task DeleteClicked_should_delete_api()
        {
            string apiId = await CreateApi();

            CreateTestHost("Alice Smith",
                SharedConstants.WRITER,
                apiId,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler mockHttp);

            WaitForLoaded(host, component);

            var input = WaitForNode(host, component, "#delete-entity input");

            await host.WaitForNextRenderAsync(() => input.ChangeAsync(apiId));

            var confirm = component.Find("#delete-entity button.btn-danger");

            await host.WaitForNextRenderAsync(() => confirm.ClickAsync());

            WaitForDeletedToast(host, component);

            await DbActionAsync<IdentityServerDbContext>(async context =>
            {
                var api = await context.Apis.FirstOrDefaultAsync(a => a.Id == apiId);
                Assert.Null(api);
            });
        }

        [Fact]
        public void DisposeTest()
        {
            CreateTestHost("Alice Smith",
                SharedConstants.WRITER,
                null,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler mockHttp);

            WaitForLoaded(host, component);

            host.Dispose();

            Assert.Throws<ArgumentException>(() => 
            {
                component.GetMarkup();
            });
        }

        [Fact]
        public async Task ClickSecretsButtons_should_not_throw()
        {
            var apiId = await CreateApi();
            CreateTestHost("Alice Smith",
                         SharedConstants.WRITER,
                         apiId,
                         out TestHost host,
                         out RenderedComponent<App> component,
                         out MockHttpMessageHandler mockHttp);

            WaitForLoaded(host, component);

            var buttons = WaitForAllNodes(host, component, "#secrets button");            

            buttons = buttons.Where(b => b.Attributes.Any(a => a.Name == "onclick")).ToList();

            var expected = buttons.Count;
            await host.WaitForNextRenderAsync(() => buttons.First().ClickAsync());

            buttons = component.FindAll("#secrets button")
                .Where(b => b.Attributes.Any(a => a.Name == "onclick")).ToList();

            Assert.NotEqual(expected, buttons.Count);

            await host.WaitForNextRenderAsync(() => buttons.Last().ClickAsync());

            buttons = component.FindAll("#secrets button")
                .Where(b => b.Attributes.Any(a => a.Name == "onclick")).ToList();

            Assert.Equal(expected, buttons.Count);
        }

        [Fact]
        public async Task ClickScopesButtons_should_not_throw()
        {
            var apiId = await CreateApi();
            CreateTestHost("Alice Smith",
                         SharedConstants.WRITER,
                         apiId,
                         out TestHost host,
                         out RenderedComponent<App> component,
                         out MockHttpMessageHandler mockHttp);

            WaitForLoaded(host, component);

            var buttons = WaitForAllNodes(host, component, "#scopes button");

            buttons = buttons.Where(b => b.Attributes.Any(a => a.Name == "onclick")).ToList();

            var expected = buttons.Count;
            await host.WaitForNextRenderAsync(() => buttons.First().ClickAsync());

            buttons = component.FindAll("#scopes button")
                .Where(b => b.Attributes.Any(a => a.Name == "onclick")).ToList();

            Assert.NotEqual(expected, buttons.Count);

            await host.WaitForNextRenderAsync(() => buttons.Last().ClickAsync());

            buttons = component.FindAll("#scopes button")
                .Where(b => b.Attributes.Any(a => a.Name == "onclick")).ToList();

            Assert.Equal(expected, buttons.Count);
        }

        [Fact]
        public async Task ClickPropertiesButtons_should_not_throw()
        {
            var apiId = await CreateApi();
            CreateTestHost("Alice Smith",
                         SharedConstants.WRITER,
                         apiId,
                         out TestHost host,
                         out RenderedComponent<App> component,
                         out MockHttpMessageHandler mockHttp);

            WaitForLoaded(host, component);

            var buttons = WaitForAllNodes(host, component, "#properties button");

            buttons = buttons.Where(b => b.Attributes.Any(a => a.Name == "onclick")).ToList();

            var expected = buttons.Count;
            await host.WaitForNextRenderAsync(() => buttons.First().ClickAsync());

            buttons = component.FindAll("#properties button")
                .Where(b => b.Attributes.Any(a => a.Name == "onclick")).ToList();

            Assert.NotEqual(expected, buttons.Count);

            await host.WaitForNextRenderAsync(() => buttons.Last().ClickAsync());

            buttons = component.FindAll("#properties button")
                .Where(b => b.Attributes.Any(a => a.Name == "onclick")).ToList();

            Assert.Equal(expected, buttons.Count);
        }

        [Fact]
        public async Task ClickAddRemoveClaims_should_not_throw()
        {
            var apiId = await CreateApi();
            CreateTestHost("Alice Smith",
                         SharedConstants.WRITER,
                         apiId,
                         out TestHost host,
                         out RenderedComponent<App> component,
                         out MockHttpMessageHandler mockHttp);

            WaitForLoaded(host, component);

            var input = WaitForNode(host, component, "#claims input.new-claim");

            await host.WaitForNextRenderAsync(() => input.TriggerEventAsync("oninput", new ChangeEventArgs { Value = "name" }));

            var button = component.Find("#claims button.dropdown-item");
            while (button == null)
            {
                host.WaitForNextRender();
                button = component.Find("#claims button.dropdown-item");
            }

            Assert.NotNull(button);

            await host.WaitForNextRenderAsync(() => button.ClickAsync());

            var divs = component.FindAll("#claims div.select");

            Assert.NotEmpty(divs);

            await host.WaitForNextRenderAsync(() => divs.Last().ClickAsync());
        }

        [Fact]
        public async Task DeleteScopeClaimsClick_should_delete_scope_claim()
        {
            var apiId = await CreateApi();
            CreateTestHost("Alice Smith",
                         SharedConstants.WRITER,
                         apiId,
                         out TestHost host,
                         out RenderedComponent<App> component,
                         out MockHttpMessageHandler mockHttp);

            WaitForLoaded(host, component);

            var div = WaitForNode(host, component, "#scopes div.select");

            await host.WaitForNextRenderAsync(() => div.ClickAsync());

            var form = component.Find("form");

            await host.WaitForNextRenderAsync(() => form.SubmitAsync());

            WaitForSavedToast(host, component);

            await DbActionAsync<IdentityServerDbContext>(async context =>
            {
                var scope = await context.ApiScopes.FirstAsync(s => s.ApiId == apiId);
                Assert.False(await context.ApiScopeClaims.AnyAsync(c => c.ApiScpopeId == scope.Id));
            });
        }

        [Fact]
        public async Task AddScopeClaims_should_validate_claim()
        {
            var apiId = await CreateApi();
            ApiScope scope = null;
            int expected = 0;
            await DbActionAsync<IdentityServerDbContext>(async context =>
            {
                scope = await context.ApiScopes.FirstAsync(s => s.ApiId == apiId);
                expected = await context.ApiScopeClaims.CountAsync(c => c.ApiScpopeId == scope.Id);
            });

            CreateTestHost("Alice Smith",
                         SharedConstants.WRITER,
                         apiId,
                         out TestHost host,
                         out RenderedComponent<App> component,
                         out MockHttpMessageHandler mockHttp);

            WaitForLoaded(host, component);

            var input = WaitForNode(host, component, "#scopes input.new-claim");

            await host.WaitForNextRenderAsync(() => input.TriggerEventAsync("oninput", new ChangeEventArgs { Value = "name" }));

            var button = component.Find("#scopes button.dropdown-item");
            while (button == null)
            {
                host.WaitForNextRender();
                button = component.Find("#scopes button.dropdown-item");
            }

            await host.WaitForNextRenderAsync(() => button.ClickAsync());

            var form = component.Find("form");

            await host.WaitForNextRenderAsync(() => form.SubmitAsync());

            WaitForSavedToast(host, component);

            await DbActionAsync<IdentityServerDbContext>(async context =>
            {
                var count = await context.ApiScopeClaims.CountAsync(c => c.ApiScpopeId == scope.Id);
                Assert.True(expected <= count);
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
                if (!context.IdentityClaims.Any(c => c.Type == "name"))
                {
                    context.Identities.Add(new IdentityResource
                    {
                        Id = GenerateId(),
                        DisplayName = GenerateId(),
                        IdentityClaims = new List<IdentityClaim>
                    {
                        new IdentityClaim
                        {
                            Id = GenerateId(),
                            Type = "name"
                        }
                    }
                    });
                }
                return context.SaveChangesAsync();
            });
            return apiId;
        }
    }
}
