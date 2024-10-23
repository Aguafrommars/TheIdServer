// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Bunit;
using System;
using System.Threading.Tasks;
using Xunit;
using ApisPage = Aguacongas.TheIdServer.BlazorApp.Pages.Apis.Apis;

namespace Aguacongas.TheIdServer.IntegrationTest.BlazorApp.Pages
{
    [Collection(BlazorAppCollection.Name)]
    public class ApisTest : EntitiesPageTestBase<ProtectResource, ApisPage>
    {
        public override string Entities => "apis";
        public ApisTest(TheIdServerFactory factory)
            : base(factory)
        {
        }

        [Fact]
        public async Task ScrollBottomReach_should_load_next_entities()
        {
            await DbActionAsync<ConfigurationDbContext>(context =>
            {
                for(var i = 0; i < 200; i++)
                context.Apis.Add(new ProtectResource
                {
                    Id = GenerateId(),
                    DisplayName = "filtered"
                });

                return context.SaveChangesAsync();
            });

            JSInterop.Setup<bool>("browserInteropt.isScrollable", 0).SetResult(true);

            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY);

            component.WaitForState(() => component.Markup.Contains(FilteredString), TimeSpan.FromMinutes(1));

            var tdList = component.FindAll(".table-hover tr td");

            component.WaitForState(() => component.Markup.Contains(FilteredString), TimeSpan.FromMinutes(1));

            Assert.NotEmpty(tdList);

            tdList = component.FindAll(".table-hover tr td");

            JSInterop.Setup<bool>("browserInteropt.isScrollable", 0).SetResult(false);
            await component.Instance.ScrollBottomReach();

            component.WaitForState(() => component.Markup.Contains(FilteredString), TimeSpan.FromMinutes(1));

            var newTdList = component.FindAll(".table-hover tr td");

            Assert.NotEqual(tdList.Count, newTdList.Count);
        }

        protected override Task PopulateList()
        {
            return DbActionAsync<ConfigurationDbContext>(context =>
            {
                context.Apis.Add(new ProtectResource
                {
                    Id = GenerateId(),
                    DisplayName = "filtered"
                });

                return context.SaveChangesAsync();
            });
        }
    }
}
