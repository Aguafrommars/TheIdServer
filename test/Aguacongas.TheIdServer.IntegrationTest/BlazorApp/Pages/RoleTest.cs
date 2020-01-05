using Aguacongas.TheIdServer.BlazorApp;
using Aguacongas.TheIdServer.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Testing;
using Microsoft.AspNetCore.Identity;
using RichardSzalay.MockHttp;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Aguacongas.TheIdServer.IntegrationTest.BlazorApp.Pages
{
    [Collection("api collection")]
    public class RoleTest : EntityPageTestBase
    {
        public override string Entity => "role";

        public RoleTest(ApiFixture fixture, ITestOutputHelper testOutputHelper):base(fixture, testOutputHelper)
        {
        }

        [Fact]
        public async Task OnFilterChanged_should_filter_claims()
        {
            var roleId = GenerateId();
            await DbActionAsync<ApplicationDbContext>(context =>
            {
                context.Roles.Add(new IdentityRole
                {
                    Id = roleId,
                    Name = "test"
                });
                context.RoleClaims.Add(new IdentityRoleClaim<string>
                {
                    RoleId = roleId,
                    ClaimType = "filtered",
                    ClaimValue = "filtered"
                });
                return context.SaveChangesAsync();
            });

            CreateTestHost("Alice Smith",
                AuthorizationOptionsExtensions.WRITER,
                roleId,
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
                Value = roleId
            }));

            markup = component.GetMarkup();

            Assert.DoesNotContain("filtered", markup);
        }
    }
}
