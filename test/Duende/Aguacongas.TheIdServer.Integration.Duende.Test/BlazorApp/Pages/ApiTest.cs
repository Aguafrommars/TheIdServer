// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
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
using ApiPage = Aguacongas.TheIdServer.BlazorApp.Pages.Api.Api;

namespace Aguacongas.TheIdServer.IntegrationTest.BlazorApp.Pages
{
    [Collection(BlazorAppCollection.Name)]
    public class ApiTest : EntityPageTestBase<ApiPage>
    {
        public override string Entity => "protectresource";
        public ApiTest(TheIdServerFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task OnFilterChanged_should_filter_properties_scopes_scopeClaims_and_secret()
        {
            string apiId = await CreateApi();

            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY,
                apiId);

            var filterInput = component.Find("input[placeholder=\"filter\"]");

            Assert.NotNull(filterInput);

            await filterInput.TriggerEventAsync("oninput", new ChangeEventArgs
            {
                Value = apiId
            });

            Assert.DoesNotContain("filtered", component.Markup);
        }

        [Fact]
        public async Task SaveClicked_should_update_api()
        {
            string apiId = await CreateApi();

            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY,
                apiId);

            var input = component.Find("#displayName");

            var expected = GenerateId();
            await input.ChangeAsync(new ChangeEventArgs
            {
                Value = expected
            });

            Assert.Contains(expected, component.Markup);

            var form = component.Find("form");

            Assert.NotNull(form);

            await form.SubmitAsync();

            await DbActionAsync<ConfigurationDbContext>(async context =>
            {
                var api = await context.Apis.FirstOrDefaultAsync(a => a.Id == apiId);
                Assert.Equal(expected, api?.DisplayName);
            });
        }

        [Fact]
        public async Task SaveClicked_should_create_api()
        {
            var apiId = GenerateId();

            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY,
                null);

            var input = component.Find("#name");

            await input.ChangeAsync(new ChangeEventArgs
            {
                Value = apiId
            });

            input = component.Find("#displayName");

            Assert.NotNull(input);

            await input.ChangeAsync(new ChangeEventArgs
            {
                Value = apiId
            });

            var form = component.Find("form");

            Assert.NotNull(form);

            await form.SubmitAsync();

            await DbActionAsync<ConfigurationDbContext>(async context =>
            {
                var api = await context.Apis.FirstOrDefaultAsync(a => a.Id == apiId);
                Assert.Equal(apiId, api?.DisplayName);
            });
        }

        [Fact]
        public async Task DeleteClicked_should_delete_api()
        {
            string apiId = await CreateApi();

            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY,
                apiId);

            var input = component.Find("#delete-entity input");

            await input.ChangeAsync(new ChangeEventArgs
            {
                Value = apiId
            });

            var confirm = component.Find("#delete-entity button.btn-danger");

            await confirm.ClickAsync(new MouseEventArgs());

            await DbActionAsync<ConfigurationDbContext>(async context =>
            {
                var api = await context.Apis.FirstOrDefaultAsync(a => a.Id == apiId);
                Assert.Null(api);
            });
        }

        [Fact]
        public void DisposeTest()
        {
            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY,
                null);

            component.Dispose();

            Assert.Throws<ComponentDisposedException>(()=> component.Markup);
        }

        [Fact]
        public async Task ClickSecretsButtons_should_not_throw()
        {
            var apiId = await CreateApi();
            var component = CreateComponent("Alice Smith",
                         SharedConstants.WRITERPOLICY,
                         apiId);

            component.WaitForElements("#secrets button");
            var buttons = component.FindAll("#secrets button").ToList();            

            buttons = buttons.Where(b => b.Attributes.Any(a => a.Name == "blazor:onclick")).ToList();

            var expected = buttons.Count;
            await buttons.First().ClickAsync(new MouseEventArgs());

            buttons = component.FindAll("#secrets button")
                .Where(b => b.Attributes.Any(a => a.Name == "blazor:onclick")).ToList();

            Assert.NotEqual(expected, buttons.Count);

            await buttons.Last().ClickAsync(new MouseEventArgs());
        }

        [Fact]
        public async Task ClickPropertiesButtons_should_not_throw()
        {
            var apiId = await CreateApi();
            var component = CreateComponent("Alice Smith",
                         SharedConstants.WRITERPOLICY,
                         apiId);

            component.WaitForState(() => component.Markup.Contains("filtered"));

            var buttons = component.WaitForElements("#properties button")
                .Where(b => b.Attributes.Any(a => a.Name == "blazor:onclick")).ToList();

            var notExpected = buttons.Count;
            await buttons.First().ClickAsync(new MouseEventArgs());

            buttons = component.FindAll("#properties button")
                .Where(b => b.Attributes.Any(a => a.Name == "blazor:onclick")).ToList();

            Assert.NotEqual(notExpected, buttons.Count);

            var expected = buttons.Count - 1;

            await buttons.Last().ClickAsync(new MouseEventArgs());

            buttons = component.FindAll("#properties button")
                .Where(b => b.Attributes.Any(a => a.Name == "blazor:onclick")).ToList();

            Assert.Equal(expected, buttons.Count);
        }

        [Fact]
        public async Task ClickAddRemoveClaims_should_not_throw()
        {
            var apiId = await CreateApi();
            var component = CreateComponent("Alice Smith",
                         SharedConstants.WRITERPOLICY,
                         apiId);

            var input = component.Find("#claims input.new-claim");

            await input.TriggerEventAsync("oninput", new ChangeEventArgs { Value = "name" });

            component.WaitForElement("#claims button.dropdown-item");
            var button = component.Find("#claims button.dropdown-item");

            Assert.NotNull(button);

            await button.ClickAsync(new MouseEventArgs());

            var divs = component.FindAll("#claims div.select");

            Assert.NotEmpty(divs);

            await divs[divs.Count - 1].ClickAsync(new MouseEventArgs());
        }

        [Fact]
        public async Task WhenWriter_should_be_able_to_clone_entity()
        {
            var apiId = await CreateApi();

            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY,
                apiId,
                true);

            var input = component.Find("#displayName");

            Assert.Contains(input.Attributes, a => a.Value == $"Clone of {apiId}");
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
