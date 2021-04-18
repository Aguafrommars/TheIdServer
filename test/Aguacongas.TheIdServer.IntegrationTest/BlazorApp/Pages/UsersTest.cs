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
    public class UsersTest : EntitiesPageTestBase<User>
    {
        public override string Entities => "users";

        public UsersTest(ApiFixture fixture, ITestOutputHelper testOutputHelper)
            :base(fixture, testOutputHelper)
        {
        }

        protected override Task PopulateList()
        {
            return DbActionAsync<ApplicationDbContext>(context =>
            {
                context.Users.Add(new User
                {
                    Id = GenerateId(),
                    UserName = "filtered",
                });

                return context.SaveChangesAsync();
            });
        }
    }
}
