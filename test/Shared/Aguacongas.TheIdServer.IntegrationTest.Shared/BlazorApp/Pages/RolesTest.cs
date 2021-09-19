// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.Data;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Aguacongas.TheIdServer.IntegrationTest.BlazorApp.Pages
{
    [Collection("api collection")]
    public class RolesTest : EntitiesPageTestBase<Role>
    {
        public override string Entities => "roles";

        public RolesTest(ApiFixture fixture, ITestOutputHelper testOutputHelper)
            :base(fixture, testOutputHelper)
        {
        }

        protected override Task PopulateList()
        {
            return DbActionAsync<ApplicationDbContext>(context =>
            {
                context.Roles.Add(new Role
                {
                    Id = GenerateId(),
                    Name = "filtered",
                });

                return context.SaveChangesAsync();
            });
        }
    }
}
