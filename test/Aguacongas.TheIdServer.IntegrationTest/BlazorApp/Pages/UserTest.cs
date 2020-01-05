using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.BlazorApp;
using Aguacongas.TheIdServer.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Testing;
using Microsoft.EntityFrameworkCore;
using RichardSzalay.MockHttp;
using System;
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
            var tuple = await SetupPage();
            var userId = tuple.Item1;
            var host = tuple.Item2;
            var component = tuple.Item3;

            var filterInput = component.Find("input[placeholder=\"filter\"]");

            Assert.NotNull(filterInput);

            host.WaitForNextRender(async () => await filterInput.TriggerEventAsync("oninput", new ChangeEventArgs
            {
                Value = userId
            }));

            var markup = component.GetMarkup();

            Assert.DoesNotContain("filtered", markup);
        }

        [Fact]
        public async Task DeleteUserTokenClicked_should_remove_user_token()
        {
            var tuple = await SetupPage();
            var userId = tuple.Item1;
            var host = tuple.Item2;
            var component = tuple.Item3;

            await DbActionAsync<ApplicationDbContext>(async context =>
            {
                var token = await context.UserTokens.FirstOrDefaultAsync(t => t.UserId == userId);
                Assert.NotNull(token);
            });


            var deleteButton = component.Find("#external-logins-tokens button[type=button]");

            Assert.NotNull(deleteButton);

            host.WaitForNextRender(() => deleteButton.Click());

            var form = component.Find("form");

            Assert.NotNull(form);

            host.WaitForNextRender(() => form.Submit());


            var tokensDiv = component.Find("#external-logins-tokens");

            Assert.NotNull(tokensDiv);

            Assert.DoesNotContain("filtered", tokensDiv.InnerText);

            host.WaitForNextRender();

            var toasts = component.FindAll(".toast-body.text-success");

            Assert.Contains(toasts, t => t.InnerText.Contains("Saved"));
            await DbActionAsync<ApplicationDbContext>(async context =>
            {
                var token = await context.UserTokens.FirstOrDefaultAsync(t => t.UserId == userId);
                Assert.Null(token);
            });
        }

        private async Task<Tuple<string, TestHost, RenderedComponent<App>>> SetupPage()
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

            Assert.Contains("filtered", markup);

            return new Tuple<string, TestHost, RenderedComponent<App>>(userId, host, component);
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
                context.UserLogins.Add(new Microsoft.AspNetCore.Identity.IdentityUserLogin<string>
                {
                    UserId = userId,
                    LoginProvider = "filtered",
                    ProviderDisplayName = "filtered",
                    ProviderKey = "filtered"
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
