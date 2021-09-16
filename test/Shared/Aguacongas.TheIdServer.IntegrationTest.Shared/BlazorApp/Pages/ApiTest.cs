// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
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

#pragma warning disable S1854 // Unused assignments should be removed
            string markup = WaitForLoaded(host, component);
#pragma warning restore S1854 // Unused assignments should be removed

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

            await DbActionAsync<ConfigurationDbContext>(async context =>
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

            var form = component.Find("form");

            Assert.NotNull(form);

            await host.WaitForNextRenderAsync(() => form.SubmitAsync());

            WaitForSavedToast(host, component);

            await DbActionAsync<ConfigurationDbContext>(async context =>
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

            await DbActionAsync<ConfigurationDbContext>(async context =>
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

            string markup = null;
            try
            {
                markup = component.GetMarkup();
            }
            catch(ArgumentException)
            {
                // silent catch
            }

            Assert.Null(markup);
            
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

            while (buttons.Count == expected)
            {
                host.WaitForNextRender();
                buttons = component.FindAll("#secrets button")
                    .Where(b => b.Attributes.Any(a => a.Name == "onclick")).ToList();
            }

            Assert.NotEqual(expected, buttons.Count);

            await host.WaitForNextRenderAsync(() => buttons.Last().ClickAsync());

            buttons = component.FindAll("#secrets button")
                .Where(b => b.Attributes.Any(a => a.Name == "onclick")).ToList();

            while(buttons.Count != expected)
            {
                host.WaitForNextRender();
                buttons = component.FindAll("#secrets button")
                    .Where(b => b.Attributes.Any(a => a.Name == "onclick")).ToList();
            }
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

        private async Task<string> CreateApi()
        {
            var apiId = GenerateId();
            await DbActionAsync<ConfigurationDbContext>(context =>
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
                    ApiScopes = new List<ApiApiScope>
                    {
                       new ApiApiScope
                       {
                           Id = GenerateId(),
                           ApiId = apiId,
                           ApiScopeId = "filtered"
                       }
                    },
                    Secrets = new List<ApiSecret>
                    {
                        new ApiSecret { Id = GenerateId(), Type="SHA256", Value = "filtered" }
                    },
                    Resources = new List<ApiLocalizedResource>
                    {
                        new ApiLocalizedResource
                        {
                            Id = GenerateId(),
                            ApiId = apiId,
                            ResourceKind = EntityResourceKind.DisplayName,
                            CultureId = "en",
                            Value = GenerateId()
                        }
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
