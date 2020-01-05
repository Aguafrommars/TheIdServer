using Aguacongas.TheIdServer.Data;
using Aguacongas.TheIdServer.Models;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Aguacongas.TheIdServer.IntegrationTest.BlazorApp.Pages
{
    [Collection("api collection")]
    public class UsersTest : EntitiesPageTestBase
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
                context.Users.Add(new ApplicationUser
                {
                    Id = GenerateId(),
                    UserName = "filtered",
                });

                return context.SaveChangesAsync();
            });
        }
    }
}
