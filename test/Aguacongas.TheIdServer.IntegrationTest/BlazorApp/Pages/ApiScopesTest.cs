using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.IdentityServer.Store.Entity;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Aguacongas.TheIdServer.IntegrationTest.BlazorApp.Pages
{
    [Collection("api collection")]
    public class ApiScopesTest : EntitiesPageTestBase<ApiScope>
    {
        public override string Entities => "scopes";
        public ApiScopesTest(ApiFixture fixture, ITestOutputHelper testOutputHelper)
            : base (fixture, testOutputHelper)
        {
        }

        protected override Task PopulateList()
        {
            return DbActionAsync<ConfigurationDbContext>(context =>
            {
                context.ApiScopes.Add(new ApiScope
                {
                    Id = GenerateId(),
                    DisplayName = "filtered"
                });

                return context.SaveChangesAsync();
            });
        }
    }
}
