// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.BlazorApp;
using AngleSharp.Dom;
using Bunit;
using Bunit.Extensions.WaitForHelpers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using CulturePage = Aguacongas.TheIdServer.BlazorApp.Pages.Culture.Culture;

namespace Aguacongas.TheIdServer.IntegrationTest.BlazorApp.Pages
{
    [Collection(BlazorAppCollection.Name)]
    public class CultureTest : EntityPageTestBase<CulturePage>
    {
        public override string Entity => "culture";
        public CultureTest(TheIdServerFactory factory) : base(factory)
        {
        }


        [Fact]
        public async Task OnFilterChanged_should_filter_resources()
        {
            string cultureId = await CreateCulture();

            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY,
                cultureId);

            var filterInput = component.Find("input[placeholder=\"filter\"]");

            Assert.NotNull(filterInput);

            await filterInput.TriggerEventAsync("oninput", new ChangeEventArgs
            {
                Value = cultureId
            }).ConfigureAwait(false);

            Assert.DoesNotContain("filtered", component.Markup);
        }

        [Fact]
        public async Task WhenWriter_should_be_able_to_clone_entity()
        {
            string cultureId = await CreateCulture();

            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY,
                cultureId,
                true);

            var input = WaitForNode(component, "input[placeholder=culture]");

            Assert.NotNull(input);
        }

        private async Task<string> CreateCulture()
        {
            var cultureId = CultureInfo.CurrentCulture.Name;
            await DbActionAsync<ConfigurationDbContext>(async context =>
            {
                if (await context.Cultures.AnyAsync(c => c.Id == cultureId))
                {
                    return;
                }

                await context.Cultures.AddAsync(new Culture
                {
                    Id = cultureId,
                    Resources = new[]
                    {
                        new LocalizedResource
                        {
                            Id = Guid.NewGuid().ToString(),
                            Key = "filtered",
                            Value = "filtered"
                        }
                    },
                });

                await context.SaveChangesAsync();
            });
            return cultureId;
        }
    }
}
