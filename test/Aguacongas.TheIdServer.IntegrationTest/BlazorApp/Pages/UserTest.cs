using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.BlazorApp;
using Aguacongas.TheIdServer.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Testing;
using RichardSzalay.MockHttp;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Aguacongas.TheIdServer.IntegrationTest.BlazorApp.Pages
{
    [Collection("api collection")]
    public class UserTest : EntityPageTestBase
    {
        public override string Entity => "user";

        public UserTest(ApiFixture fixture, ITestOutputHelper testOutputHelper):base(fixture, testOutputHelper)
        {
        }

        [Fact]
        public async Task OnFilterChanged_should_filter_roles_consents_claims_and_tokens()
        {
            var userId = GenerateId();
            await CreateTestEntity(userId);

            CreateTestHost("Alice Smith",
                AuthorizationOptionsExtensions.WRITER,
                userId,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler mockHttp);

            host.WaitForNextRender();

            var markup = component.GetMarkup();

            if (markup.Contains("Loading..."))
            {
                host.WaitForNextRender();
                markup = component.GetMarkup();
            }

            markup = component.GetMarkup();

            Assert.Contains("filtered", markup);

            var filterInput = component.Find("input[placeholder=\"filter\"]");

            Assert.NotNull(filterInput);

            host.WaitForNextRender(async () => await filterInput.TriggerEventAsync("oninput", new ChangeEventArgs
            {
                Value = userId
            }));

            markup = component.GetMarkup();

            Assert.DoesNotContain("filtered", markup);
        }

        private async Task CreateTestEntity(string userId)
        {
            await DbActionAsync<ApplicationDbContext>(context =>
            {
                context.Users.Add(new Models.ApplicationUser
                {
                    Id = userId,
                    UserName = "test"
                });
                var roleId = GenerateId();
                context.Roles.Add(new Microsoft.AspNetCore.Identity.IdentityRole
                {
                    Id = roleId,
                    Name = "filtered"
                });
                context.UserRoles.Add(new Microsoft.AspNetCore.Identity.IdentityUserRole<string>
                {
                    RoleId = roleId,
                    UserId = userId
                });
                context.UserClaims.Add(new Microsoft.AspNetCore.Identity.IdentityUserClaim<string>
                {
                    ClaimType = "filtered",
                    ClaimValue = "filtered",
                    UserId = userId
                });
                context.UserTokens.Add(new Microsoft.AspNetCore.Identity.IdentityUserToken<string>
                {
                    UserId = userId,
                    LoginProvider = "filtered",
                    Name = "filtered",
                    Value = "filtered"
                });
                return context.SaveChangesAsync();
            });
            await DbActionAsync<IdentityServerDbContext>(context =>
            {
                var clientId = GenerateId();
                context.Clients.Add(new Client
                {
                    Id = clientId,
                    ClientName = "filtered",
                    ProtocolType = "oidc"
                });
                context.UserConstents.Add(new UserConsent
                {
                    Id = GenerateId(),
                    ClientId = clientId,
                    UserId = userId,
                    Data = "{\"Scopes\": [\"filtered\"]}"
                });
                return context.SaveChangesAsync();
            });
        }
    }
}
