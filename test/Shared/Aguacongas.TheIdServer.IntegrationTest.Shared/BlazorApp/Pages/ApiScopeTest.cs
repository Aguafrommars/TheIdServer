// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
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
using page = Aguacongas.TheIdServer.BlazorApp.Pages.ApiScope.ApiScope;

namespace Aguacongas.TheIdServer.IntegrationTest.BlazorApp.Pages
{
    [Collection(BlazorAppCollection.Name)]
    public class ApiScopeTest : EntityPageTestBase<page>
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

            addButton.Click(new MouseEventArgs());

            var cultureInputs = component.FindAll("input[placeholder=\"culture\"]");

            Assert.NotNull(cultureInputs);

            var cultureInput = cultureInputs.Last();

            cultureInput.TriggerEvent("oninput", new ChangeEventArgs { Value = "en" });

            var dropDownItem = component.Find("button.dropdown-item");
            Assert.NotNull(dropDownItem);

            dropDownItem.Click(new MouseEventArgs());

            var addDescriptionButton = component.Find("#btnAddDescription");

            Assert.NotNull(addButton);

            addDescriptionButton.Click(new MouseEventArgs());

            cultureInputs = component.FindAll("input[placeholder=\"culture\"]");

            Assert.NotNull(cultureInputs);

            cultureInput = cultureInputs.Last();

            cultureInput.TriggerEvent("oninput", new ChangeEventArgs { Value = "fr-FR" });

            var items = component.FindAll("button.dropdown-item");
            dropDownItem = items.Last();
            Assert.NotNull(dropDownItem);

            dropDownItem.Click(new MouseEventArgs());

            cultureInputs = component.FindAll("input[placeholder=\"culture\"]");

            Assert.NotNull(cultureInputs);

            cultureInput = cultureInputs.Last();

            cultureInput.TriggerEvent("oninput", new ChangeEventArgs { Value = "fr-FR" });

            items = component.FindAll("button.dropdown-item");
            dropDownItem = items.Last();
            Assert.NotNull(dropDownItem);

            dropDownItem.Click(new MouseEventArgs());

            var form = component.Find("form");

            Assert.NotNull(form);

            form.Submit();
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

            filterInput.TriggerEvent("oninput", new ChangeEventArgs
            {
                Value = apiScopeId
            });

            Assert.DoesNotContain("filtered", component.Markup);
        }

        [Fact]
        public async Task SaveClick_should_update_entity()
        {
            string identityId = await CreateEntity();

            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY,
                identityId);

            var input = component.Find("#displayName");

            Assert.NotNull(input);

            var expected = GenerateId();
            input.Change(new ChangeEventArgs
            {
                Value = expected
            });

            Assert.Contains(expected, component.Markup);

            var form = component.Find("form");

            Assert.NotNull(form);

            form.Submit();

            await DbActionAsync<ConfigurationDbContext>(async context =>
            {
                var apiScope = await context.ApiScopes.FirstOrDefaultAsync(a => a.Id == identityId);
                Assert.Equal(expected, apiScope.DisplayName);
            });
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
