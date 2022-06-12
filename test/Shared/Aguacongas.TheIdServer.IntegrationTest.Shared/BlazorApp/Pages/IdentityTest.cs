// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.BlazorApp;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using page = Aguacongas.TheIdServer.BlazorApp.Pages.Identity.Identity;

namespace Aguacongas.TheIdServer.IntegrationTest.BlazorApp.Pages
{
    [Collection(BlazorAppCollection.Name)]
    public class IdentityTest : EntityPageTestBase<page>
    {
        public override string Entity => "identityresource";
        public IdentityTest(TheIdServerFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task OnAddTranslation_should_validate_resource()
        {
            string identityId = await CreateEntity();

            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY,
                identityId);

            var addButton = component.Find("#btnAddDisplayName");

            Assert.NotNull(addButton);

            await addButton.ClickAsync(new MouseEventArgs()).ConfigureAwait(false);

            var cultureInputs = component.FindAll("input[placeholder=\"culture\"]");

            Assert.NotNull(cultureInputs);

            var cultureInput = cultureInputs.Last();

            await cultureInput.TriggerEventAsync("oninput", new ChangeEventArgs { Value = "en" }).ConfigureAwait(false);

            var dropDownItem = WaitForNode(component, "button.dropdown-item");
            Assert.NotNull(dropDownItem);

            await dropDownItem.ClickAsync(new MouseEventArgs()).ConfigureAwait(false);

            var addDescriptionButton = component.Find("#btnAddDescription");

            Assert.NotNull(addButton);

            await addDescriptionButton.ClickAsync(new MouseEventArgs()).ConfigureAwait(false);

            cultureInputs = component.FindAll("input[placeholder=\"culture\"]");

            Assert.NotNull(cultureInputs);

            cultureInput = cultureInputs.Last();

            await cultureInput.TriggerEventAsync("oninput", new ChangeEventArgs { Value = "fr-FR" }).ConfigureAwait(false);

            var items = component.FindAll("button.dropdown-item");
            dropDownItem = items.Last();
            Assert.NotNull(dropDownItem);

            await dropDownItem.ClickAsync(new MouseEventArgs()).ConfigureAwait(false);

            cultureInputs = component.FindAll("input[placeholder=\"culture\"]");

            Assert.NotNull(cultureInputs);

            cultureInput = cultureInputs.Last();

            await cultureInput.TriggerEventAsync("oninput", new ChangeEventArgs { Value = "fr-FR" }).ConfigureAwait(false);

            items = component.FindAll("button.dropdown-item");
            dropDownItem = items.Last();
            Assert.NotNull(dropDownItem);

            await dropDownItem.ClickAsync(new MouseEventArgs()).ConfigureAwait(false);

            var form = component.Find("form");

            Assert.NotNull(form);

            await form.SubmitAsync().ConfigureAwait(false);
        }


        [Fact]
        public async Task OnFilterChanged_should_filter_properties_and_claims()
        {
            string identityId = await CreateEntity();

            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY,
                identityId);

            var filterInput = component.Find("input[placeholder=\"filter\"]");

            Assert.NotNull(filterInput);

            await filterInput.TriggerEventAsync("oninput", new ChangeEventArgs
            {
                Value = identityId
            }).ConfigureAwait(false);

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
                var identity = await context.Identities.FirstOrDefaultAsync(a => a.Id == identityId);
                Assert.Equal(expected, identity?.DisplayName);
            });
        }

        private async Task<string> CreateEntity()
        {
            var identityId = GenerateId();
            await DbActionAsync<ConfigurationDbContext>(context =>
            {
                context.Identities.Add(new IdentityResource
                {
                    Id = identityId,
                    DisplayName = identityId,
                    IdentityClaims = new List<IdentityClaim>
                    {
                        new IdentityClaim { Id = GenerateId(), Type = "filtered" }
                    },
                    Properties = new List<IdentityProperty>
                    {
                        new IdentityProperty { Id = GenerateId(), Key = "filtered", Value = "filtered" }
                    },
                    Resources = new List<IdentityLocalizedResource>
                    {
                        new IdentityLocalizedResource
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
            return identityId;
        }
    }
}
