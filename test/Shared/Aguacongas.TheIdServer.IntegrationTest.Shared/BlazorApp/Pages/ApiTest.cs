// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.BlazorApp;
using Bunit;
using Bunit.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using page = Aguacongas.TheIdServer.BlazorApp.Pages.Api.Api;

namespace Aguacongas.TheIdServer.IntegrationTest.BlazorApp.Pages
{
    [Collection("api collection")]
    public class ApiTest : EntityPageTestBase<page>
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
                SharedConstants.WRITERPOLICY,
                apiId,
                out IRenderedComponent<page> component);

            var filterInput = component.Find("input[placeholder=\"filter\"]");

            Assert.NotNull(filterInput);

            await filterInput.TriggerEventAsync("oninput", new ChangeEventArgs
            {
                Value = apiId
            }).ConfigureAwait(false);

            Assert.DoesNotContain("filtered", component.Markup);
        }

        [Fact]
        public async Task SaveClicked_should_update_api()
        {
            string apiId = await CreateApi();

            CreateTestHost("Alice Smith",
                SharedConstants.WRITERPOLICY,
                apiId,
                out IRenderedComponent<page> component);

            var input = component.Find("#displayName");

            var expected = GenerateId();
            await input.ChangeAsync(new ChangeEventArgs
            {
                Value = expected
            }).ConfigureAwait(false);

            Assert.Contains(expected, component.Markup);

            var form = component.Find("form");

            Assert.NotNull(form);

            await form.SubmitAsync().ConfigureAwait(false);

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
                SharedConstants.WRITERPOLICY,
                null,
                out IRenderedComponent<page> component);

            var input = component.Find("#name");

            await input.ChangeAsync(new ChangeEventArgs
            {
                Value = apiId
            }).ConfigureAwait(false);

            input = component.Find("#displayName");

            Assert.NotNull(input);

            await input.ChangeAsync(new ChangeEventArgs
            {
                Value = apiId
            }).ConfigureAwait(false);

            var form = component.Find("form");

            Assert.NotNull(form);

            await form.SubmitAsync().ConfigureAwait(false);

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
                SharedConstants.WRITERPOLICY,
                apiId,
                out IRenderedComponent<page> component);

            var input = component.Find("#delete-entity input");

            await input.ChangeAsync(new ChangeEventArgs
            {
                Value = apiId
            }).ConfigureAwait(false);

            var confirm = component.Find("#delete-entity button.btn-danger");

            await confirm.ClickAsync(new MouseEventArgs()).ConfigureAwait(false);

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
                SharedConstants.WRITERPOLICY,
                null,
                out IRenderedComponent<page> component);

            component.Dispose();

            Assert.Throws<ComponentDisposedException>(()=> component.Markup);
        }

        [Fact(Skip = "fail")]
        public async Task ClickSecretsButtons_should_not_throw()
        {
            var apiId = await CreateApi();
            CreateTestHost("Alice Smith",
                         SharedConstants.WRITERPOLICY,
                         apiId,
                         out IRenderedComponent<page> component);

            component.WaitForElements("#secrets button");
            var buttons = component.FindAll("#secrets button").ToList();            

            buttons = buttons.Where(b => b.Attributes.Any(a => a.Name == "blazor:onclick")).ToList();

            var expected = buttons.Count;
            await buttons.First().ClickAsync(new MouseEventArgs()).ConfigureAwait(false);

            buttons = component.FindAll("#secrets button")
                .Where(b => b.Attributes.Any(a => a.Name == "blazor:onclick")).ToList();

            while (buttons.Count == expected)
            {
                component.WaitForElements("#secrets button");
                buttons = component.FindAll("#secrets button")
                    .Where(b => b.Attributes.Any(a => a.Name == "blazor:onclick")).ToList();
            }

            Assert.NotEqual(expected, buttons.Count);

            await buttons.Last().ClickAsync(new MouseEventArgs()).ConfigureAwait(false);

            buttons = component.FindAll("#secrets button")
                .Where(b => b.Attributes.Any(a => a.Name == "blazor:onclick")).ToList();

            while(buttons.Count != expected)
            {
                component.WaitForElements("#secrets button");
                buttons = component.FindAll("#secrets button")
                    .Where(b => b.Attributes.Any(a => a.Name == "blazor:onclick")).ToList();
            }
            Assert.Equal(expected, buttons.Count);
        }

        [Fact(Skip = "fail")]
        public async Task ClickPropertiesButtons_should_not_throw()
        {
            var apiId = await CreateApi();
            CreateTestHost("Alice Smith",
                         SharedConstants.WRITERPOLICY,
                         apiId,
                         out IRenderedComponent<page> component);

            var buttons = component.WaitForElements("#properties button").ToList();

            buttons = buttons.Where(b => b.Attributes.Any(a => a.Name == "blazor:onclick")).ToList();

            var expected = buttons.Count;
            await buttons.First().ClickAsync(new MouseEventArgs()).ConfigureAwait(false);

            buttons = component.FindAll("#properties button")
                .Where(b => b.Attributes.Any(a => a.Name == "blazor:onclick")).ToList();

            Assert.NotEqual(expected, buttons.Count);

            await buttons.Last().ClickAsync(new MouseEventArgs()).ConfigureAwait(false);

            buttons = component.FindAll("#properties button")
                .Where(b => b.Attributes.Any(a => a.Name == "blazor:onclick")).ToList();

            Assert.Equal(expected, buttons.Count);
        }

        [Fact]
        public async Task ClickAddRemoveClaims_should_not_throw()
        {
            var apiId = await CreateApi();
            CreateTestHost("Alice Smith",
                         SharedConstants.WRITERPOLICY,
                         apiId,
                         out IRenderedComponent<page> component);

            var input = component.Find("#claims input.new-claim");

            await input.TriggerEventAsync("oninput", new ChangeEventArgs { Value = "name" }).ConfigureAwait(false);

            component.WaitForElement("#claims button.dropdown-item");
            var button = component.Find("#claims button.dropdown-item");

            Assert.NotNull(button);

            await button.ClickAsync(new MouseEventArgs()).ConfigureAwait(false);

            var divs = component.FindAll("#claims div.select");

            Assert.NotEmpty(divs);

            await divs.Last().ClickAsync(new MouseEventArgs()).ConfigureAwait(false);
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
