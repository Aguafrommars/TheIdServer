// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using ApiScopePage = Aguacongas.TheIdServer.BlazorApp.Pages.ApiScope.ApiScope;

namespace Aguacongas.TheIdServer.IntegrationTest.BlazorApp.Pages
{
    [Collection(BlazorAppCollection.Name)]
    public class ApiScopeTest : EntityPageTestBase<ApiScopePage>
    {
        public override string Entity => "apiscope";
        public ApiScopeTest(TheIdServerFactory factory):base(factory)
        {
        }

        [Fact]
        public async Task OnAddTranslation_should_validate_resource()
        {
            string apiScopeId = await CreateEntity();

            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY,
                apiScopeId);

            var addButton = component.Find("#btnAddDisplayName");

            Assert.NotNull(addButton);

            await addButton.ClickAsync(new MouseEventArgs()).ConfigureAwait(false);

            var cultureInputs = component.FindAll("input[placeholder=\"culture\"]");

            Assert.NotNull(cultureInputs);

            var cultureInput = cultureInputs[cultureInputs.Count - 1];

            await cultureInput.TriggerEventAsync("oninput", new ChangeEventArgs { Value = "en" }).ConfigureAwait(false);

            var dropDownItem = component.Find("button.dropdown-item");
            Assert.NotNull(dropDownItem);

            await dropDownItem.ClickAsync(new MouseEventArgs()).ConfigureAwait(false);

            var addDescriptionButton = component.Find("#btnAddDescription");

            Assert.NotNull(addButton);

            await addDescriptionButton.ClickAsync(new MouseEventArgs()).ConfigureAwait(false);

            cultureInputs = component.FindAll("input[placeholder=\"culture\"]");

            Assert.NotNull(cultureInputs);

            cultureInput = cultureInputs[cultureInputs.Count - 1];

            await cultureInput.TriggerEventAsync("oninput", new ChangeEventArgs { Value = "fr-FR" }).ConfigureAwait(false);

            var items = component.FindAll("button.dropdown-item");
            dropDownItem = items[items.Count - 1];
            Assert.NotNull(dropDownItem);

            await dropDownItem.ClickAsync(new MouseEventArgs()).ConfigureAwait(false);

            cultureInputs = component.FindAll("input[placeholder=\"culture\"]");

            Assert.NotNull(cultureInputs);

            cultureInput = cultureInputs[cultureInputs.Count - 1];

            await cultureInput.TriggerEventAsync("oninput", new ChangeEventArgs { Value = "fr-FR" }).ConfigureAwait(false);

            items = component.FindAll("button.dropdown-item");
            dropDownItem = items[items.Count - 1];
            Assert.NotNull(dropDownItem);

            await dropDownItem.ClickAsync(new MouseEventArgs()).ConfigureAwait(false);

            var form = component.Find("form");

            Assert.NotNull(form);

            await form.SubmitAsync().ConfigureAwait(false);
        }


        [Fact]
        public async Task OnFilterChanged_should_filter_properties_and_claims()
        {
            string apiScopeId = await CreateEntity();

            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY,
                apiScopeId);

            var filterInput = component.Find("input[placeholder=\"filter\"]");

            Assert.NotNull(filterInput);

            await filterInput.TriggerEventAsync("oninput", new ChangeEventArgs
            {
                Value = apiScopeId
            }).ConfigureAwait(false);

            Assert.DoesNotContain("filtered", component.Markup);
        }

        [Fact]
        public async Task SaveClick_should_update_entity()
        {
            string apiScopeId = await CreateEntity();

            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY,
                apiScopeId);

            var input = component.Find("#displayName");

            Assert.NotNull(input);

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
                var apiScope = await context.ApiScopes.FirstOrDefaultAsync(a => a.Id == apiScopeId);
                Assert.Equal(expected, apiScope?.DisplayName);
            });
        }

        [Fact]
        public async Task WhenWriter_should_be_able_to_clone_entity()
        {
            string apiScopeId = await CreateEntity();

            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY,
                apiScopeId, 
                true);

            var input = component.Find("#displayName");

            Assert.Contains(input.Attributes, a => a.Value == $"Clone of {apiScopeId}");
        }

        private async Task<string> CreateEntity()
        {
            var apiScopeId = GenerateId();
            await DbActionAsync<ConfigurationDbContext>(context =>
            {
                context.ApiScopes.Add(new ApiScope
                {
                    Id = apiScopeId,
                    DisplayName = apiScopeId,
                    ApiScopeClaims = new List<ApiScopeClaim>
                    {
                        new ApiScopeClaim { Id = GenerateId(), Type = "filtered" }
                    },
                    Properties = new List<ApiScopeProperty>
                    {
                        new ApiScopeProperty { Id = GenerateId(), Key = "filtered", Value = "filtered" }
                    },
                    Resources = new List<ApiScopeLocalizedResource>
                    {
                        new ApiScopeLocalizedResource
                        {
                            Id = GenerateId(),
                            ResourceKind = EntityResourceKind.DisplayName,
                            CultureId = "en",
                            Value = GenerateId()
                        }
                    }
                });

                return context.SaveChangesAsync();
            });
            return apiScopeId;
        }
    }
}
