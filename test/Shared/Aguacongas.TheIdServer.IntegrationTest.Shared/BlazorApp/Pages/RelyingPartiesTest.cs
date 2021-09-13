// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.IdentityServer.Store.Entity;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Aguacongas.TheIdServer.IntegrationTest.BlazorApp.Pages
{
    [Collection("api collection")]
    public class RelyingPartiesTest : EntitiesPageTestBase<RelyingParty>
    {
        public override string Entities => "relyingparties";
        public RelyingPartiesTest(ApiFixture fixture, ITestOutputHelper testOutputHelper)
            : base (fixture, testOutputHelper)
        {
        }

        protected override Task PopulateList()
        {
            return DbActionAsync<ConfigurationDbContext>(context =>
            {
                context.RelyingParties.Add(new RelyingParty
                {
                    Id = GenerateId(),
                    Description = "filtered"
                });

                return context.SaveChangesAsync();
            });
        }
    }
}
