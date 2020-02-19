using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.IdentityServer.Store.Entity;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Aguacongas.TheIdServer.IntegrationTest.BlazorApp.Pages
{
    [Collection("api collection")]
    public class CientsTest : EntitiesPageTestBase<Client>
    {
        public override string Entities => "clients";
        public CientsTest(ApiFixture fixture, ITestOutputHelper testOutputHelper)
            : base (fixture, testOutputHelper)
        {
        }

        protected override Task PopulateList()
        {
            return DbActionAsync<ConfigurationDbContext>(context =>
            {
                context.Clients.Add(new Client
                {
                    Id = GenerateId(),
                    ProtocolType = "oidc",
                    ClientName = "filtered"
                });

                return context.SaveChangesAsync();
            });
        }
    }
}
