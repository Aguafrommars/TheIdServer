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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Aguacongas.TheIdServer.IntegrationTest.BlazorApp.Pages
{
    [Collection("api collection")]
    public class ApiScopeTest : EntityPageTestBase
    {
        public override string Entity => "apiscope";
        public ApiScopeTest(ApiFixture fixture, ITestOutputHelper testOutputHelper):base(fixture, testOutputHelper)
        {
        }

        [Fact]
        public async Task OnAddTranslation_should_validate_resource()
        {
            string apiScopeId = await CreateEntity();

            CreateTestHost("Alice Smith",
                SharedConstants.WRITER,
                apiScopeId,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler mockHttp);

            WaitForLoaded(host, component);

            var addButton = component.Find("#btnAddDisplayName");

            Assert.NotNull(addButton);

            await host.WaitForNextRenderAsync(() => addButton.ClickAsync());

            var cultureInputs = component.FindAll("input[placeholder=\"culture\"]");

            Assert.NotNull(cultureInputs);

            var cultureInput = cultureInputs.Last();

            await host.WaitForNextRenderAsync(() => cultureInput.TriggerEventAsync("oninput", new ChangeEventArgs { Value = "en" }));

            var dropDownItem = WaitForNode(host, component, "button.dropdown-item");
            Assert.NotNull(dropDownItem);

            await host.WaitForNextRenderAsync(() => dropDownItem.ClickAsync());

            var addDescriptionButton = component.Find("#btnAddDescription");

            Assert.NotNull(addButton);

            await host.WaitForNextRenderAsync(() => addDescriptionButton.ClickAsync());

            cultureInputs = component.FindAll("input[placeholder=\"culture\"]");

            Assert.NotNull(cultureInputs);

            cultureInput = cultureInputs.Last();

            await host.WaitForNextRenderAsync(() => cultureInput.TriggerEventAsync("oninput", new ChangeEventArgs { Value = "fr-FR" }));

            var items = component.FindAll("button.dropdown-item");
            dropDownItem = items.Last();
            Assert.NotNull(dropDownItem);

            await host.WaitForNextRenderAsync(() => dropDownItem.ClickAsync());

            cultureInputs = component.FindAll("input[placeholder=\"culture\"]");

            Assert.NotNull(cultureInputs);

            cultureInput = cultureInputs.Last();

            await host.WaitForNextRenderAsync(() => cultureInput.TriggerEventAsync("oninput", new ChangeEventArgs { Value = "fr-FR" }));

            items = component.FindAll("button.dropdown-item");
            dropDownItem = items.Last();
            Assert.NotNull(dropDownItem);

            await host.WaitForNextRenderAsync(() => dropDownItem.ClickAsync());

            var form = component.Find("form");

            Assert.NotNull(form);

            await host.WaitForNextRenderAsync(() => form.SubmitAsync());

            WaitForSavedToast(host, component);
        }


        [Fact]
        public async Task OnFilterChanged_should_filter_properties_and_claims()
        {
            string apiScopeId = await CreateEntity();

            CreateTestHost("Alice Smith",
                SharedConstants.WRITER,
                apiScopeId,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler mockHttp);

            WaitForLoaded(host, component);

            WaitForContains(host, component, "filtered");

            var filterInput = component.Find("input[placeholder=\"filter\"]");

            Assert.NotNull(filterInput);

            await host.WaitForNextRenderAsync(() => filterInput.TriggerEventAsync("oninput", new ChangeEventArgs
            {
                Value = apiScopeId
            }));

            string markup = component.GetMarkup();

            Assert.DoesNotContain("filtered", markup);
        }

        [Fact]
        public async Task SaveClick_should_update_entity()
        {
            string identityId = await CreateEntity();

            CreateTestHost("Alice Smith",
                SharedConstants.WRITER,
                identityId,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler mockHttp);

            WaitForLoaded(host, component);

            var input = component.Find("#displayName");

            Assert.NotNull(input);

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
