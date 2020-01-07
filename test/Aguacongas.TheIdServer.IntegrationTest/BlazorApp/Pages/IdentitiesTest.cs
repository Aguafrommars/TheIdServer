using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.IdentityServer.Store.Entity;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Aguacongas.TheIdServer.IntegrationTest.BlazorApp.Pages
{
    [Collection("api collection")]
    public class IdentitiesTest : EntitiesPageTestBase<IdentityResource>
    {
        public override string Entities => "identities";
        public IdentitiesTest(ApiFixture fixture, ITestOutputHelper testOutputHelper)
            : base (fixture, testOutputHelper)
        {
        }

        protected override Task PopulateList()
        {
            return DbActionAsync<IdentityServerDbContext>(context =>
            {
                context.Identities.Add(new IdentityResource
                {
                    Id = GenerateId(),
                    DisplayName = "filtered"
                });

                return context.SaveChangesAsync();
            });
        }
    }
}
