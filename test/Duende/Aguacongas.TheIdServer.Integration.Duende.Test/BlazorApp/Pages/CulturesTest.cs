// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using CulturesPage = Aguacongas.TheIdServer.BlazorApp.Pages.Cultures.Cultures;

namespace Aguacongas.TheIdServer.IntegrationTest.BlazorApp.Pages
{
    [Collection(BlazorAppCollection.Name)]
    public class CulturesTest : EntitiesPageTestBase<Culture, CulturesPage>
    {
        public override string Entities => "cultures";

        protected override string FilteredString => CultureInfo.CurrentCulture.Name;

        public CulturesTest(TheIdServerFactory factory)
            : base(factory)
        {
        }

        protected override void AssertExportResponse(HttpResponseMessage response)
        {
            Assert.True(response.IsSuccessStatusCode);
        }

        protected override Task PopulateList()
        {
            return DbActionAsync<ConfigurationDbContext>(async context =>
            {
                if (await context.Cultures.AnyAsync(c => c.Id == CultureInfo.CurrentCulture.Name))
                {
                    return;
                }

                await context.Cultures.AddAsync(new Culture
                {
                    Id = CultureInfo.CurrentCulture.Name
                });

                await context.SaveChangesAsync();
            });
        }
    }
}
