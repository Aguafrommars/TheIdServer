using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.BlazorApp;
using Aguacongas.TheIdServer.Data;
using Aguacongas.TheIdServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Testing;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RichardSzalay.MockHttp;
using System;
using System.Linq;
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

            await WaitForSavedToast(host, component);

            await DbActionAsync<ApplicationDbContext>(async context =>
            {
                var token = await context.UserTokens.FirstOrDefaultAsync(t => t.UserId == userId);
                Assert.Null(token);
            });
        }

        [Fact]
        public async Task DeleteUserLoginClicked_should_remove_user_login()
        {
            var tuple = await SetupPage();
            var userId = tuple.Item1;
            var host = tuple.Item2;
            var component = tuple.Item3;

            await DbActionAsync<ApplicationDbContext>(async context =>
            {
                var login = await context.UserLogins.FirstOrDefaultAsync(t => t.UserId == userId);
                Assert.NotNull(login);
            });

            var deleteButton = component.Find("#external-logins button[type=button]");

            Assert.NotNull(deleteButton);

            host.WaitForNextRender(() => deleteButton.Click());

            var form = component.Find("form");

            Assert.NotNull(form);

            host.WaitForNextRender(() => form.Submit());

            var tokensDiv = component.Find("#external-logins");

            Assert.NotNull(tokensDiv);

            Assert.DoesNotContain("filtered", tokensDiv.InnerText);

            await WaitForSavedToast(host, component);

            await DbActionAsync<ApplicationDbContext>(async context =>
            {
                var login = await context.UserLogins.FirstOrDefaultAsync(t => t.UserId == userId);
                Assert.Null(login);
            });
        }

        [Fact]
        public async Task DeleteUserConsentClicked_should_remove_user_consent()
        {
            var tuple = await SetupPage();
            var userId = tuple.Item1;
            var host = tuple.Item2;
            var component = tuple.Item3;

            await DbActionAsync<IdentityServerDbContext>(async context =>
            {
                var consent = await context.UserConstents.FirstOrDefaultAsync(t => t.UserId == userId);
                Assert.NotNull(consent);
            });

            var deleteButton = component.Find("#consents button[type=button]");

            Assert.NotNull(deleteButton);

            host.WaitForNextRender(() => deleteButton.Click());

            var form = component.Find("form");

            Assert.NotNull(form);

            host.WaitForNextRender(() => form.Submit());

            var tokensDiv = component.Find("#consents");

            Assert.NotNull(tokensDiv);

            Assert.DoesNotContain("filtered", tokensDiv.InnerText);

            await WaitForSavedToast(host, component);

            await DbActionAsync<IdentityServerDbContext>(async context =>
            {
                var consent = await context.UserConstents.FirstOrDefaultAsync(t => t.UserId == userId);
                Assert.Null(consent);
            });
        }

        [Fact]
        public async Task DeleteUserRoleClicked_should_remove_user_from_role()
        {
            var tuple = await SetupPage();
            var userId = tuple.Item1;
            var host = tuple.Item2;
            var component = tuple.Item3;

            await DbActionAsync<ApplicationDbContext>(async context =>
            {
                var role = await context.UserRoles.FirstOrDefaultAsync(t => t.UserId == userId);
                Assert.NotNull(role);
            });

            var deleteButton = component.Find("#roles .input-group-append");

            Assert.NotNull(deleteButton);

            host.WaitForNextRender(() => deleteButton.Click());

            var form = component.Find("form");

            Assert.NotNull(form);

            host.WaitForNextRender(() => form.Submit());

            var tokensDiv = component.Find("#roles");

            Assert.NotNull(tokensDiv);

            Assert.DoesNotContain("filtered", tokensDiv.InnerText);

            host.WaitForNextRender(async () => await Task.Delay(500));

            await WaitForSavedToast(host, component);

            await DbActionAsync<ApplicationDbContext>(async context =>
            {
                var role = await context.UserRoles.FirstOrDefaultAsync(t => t.UserId == userId);
                Assert.Null(role);
            });
        }

        [Fact]
        public async Task AddUserRole_should_add_user_to_role()
        {
            var tuple = await SetupPage();
            var host = tuple.Item2;
            var component = tuple.Item3;

            var roleId = GenerateId();
            await DbActionAsync<ApplicationDbContext>(context =>
            {
                context.Roles.Add(new IdentityRole
                {
                    Id = roleId,
                    Name = roleId,
                    NormalizedName = roleId.ToUpper()
                });

                return context.SaveChangesAsync();
            });

            var input = component.Find("#roles .new-claim");

            Assert.NotNull(input);

            host.WaitForNextRender(async () =>
            {
                await input.TriggerEventAsync("oninput", new ChangeEventArgs
                {
                    Value = roleId
                });
            });

            var markup = component.GetMarkup();

            while (!markup.Contains("dropdown-item"))
            {
                host.WaitForNextRender();
                markup = component.GetMarkup();
            }

            var button = component.Find(".dropdown-item");

            Assert.NotNull(button);

            host.WaitForNextRender(() => button.Click());

            markup = component.GetMarkup();

            Assert.Contains(roleId, markup);

            var form = component.Find("form");

            Assert.NotNull(form);

            host.WaitForNextRender(() => form.Submit());

            var rolessDiv = component.Find("#roles");

            Assert.NotNull(rolessDiv);

            Assert.Contains(roleId, rolessDiv.InnerText);

            await WaitForSavedToast(host, component);

            await DbActionAsync<ApplicationDbContext>(async context =>
            {
                var role = await context.UserRoles.FirstOrDefaultAsync(t => t.RoleId == roleId);
                Assert.NotNull(role);
            });
        }

        [Fact]
        public async Task AddUserClaim_should_add_claim_to_user()
        {
            var tuple = await SetupPage();
            var host = tuple.Item2;
            var component = tuple.Item3;

            var addButton = component.Find("#claims button");

            Assert.NotNull(addButton);

            host.WaitForNextRender(() => addButton.Click());

            var rows = component.FindAll("#claims tr");

            Assert.NotNull(rows);

            var lastRow = rows.Last();
            var inputList = lastRow.Descendants("input");

            Assert.NotEmpty(inputList);

            var expected = GenerateId();
            host.WaitForNextRender(() => inputList.First().Change(expected));

            rows = component.FindAll("#claims tr");
            lastRow = rows.Last();
            inputList = lastRow.Descendants("input");

            host.WaitForNextRender(() => inputList.Last().Change(expected));

            var form = component.Find("form");

            Assert.NotNull(form);

            host.WaitForNextRender(() => form.Submit());

            await WaitForSavedToast(host, component);

            await DbActionAsync<ApplicationDbContext>(async context =>
            {
                var userId = tuple.Item1;
                var claim = await context.UserClaims.FirstOrDefaultAsync(t => t.UserId == userId &&
                    t.ClaimType == expected &&
                    t.ClaimValue == expected);
                Assert.NotNull(claim);
            });
        }

        [Fact]
        public async Task UpdateUserClaim_should_update_claim()
        {
            var tuple = await SetupPage();
            var host = tuple.Item2;
            var component = tuple.Item3;

            var rows = component.FindAll("#claims tr");

            Assert.NotNull(rows);

            var lastRow = rows.Last();
            var inputList = lastRow.Descendants("input");

            Assert.NotEmpty(inputList);

            var expected = GenerateId();

            host.WaitForNextRender(() => inputList.Last().Change(expected));

            var form = component.Find("form");

            Assert.NotNull(form);

            host.WaitForNextRender(() => form.Submit());

            await WaitForSavedToast(host, component);

            await DbActionAsync<ApplicationDbContext>(async context =>
            {
                var userId = tuple.Item1;
                var claim = await context.UserClaims.FirstOrDefaultAsync(t => t.UserId == userId &&
                    t.ClaimValue == expected);
                Assert.NotNull(claim);
            });
        }

        [Fact]
        public async Task DeleteUserClaim_should_remove_claim_from_user()
        {
            var tuple = await SetupPage();
            var host = tuple.Item2;
            var component = tuple.Item3;

            var button = component.Find("#claims tr button");

            Assert.NotNull(button);

            host.WaitForNextRender(() => button.Click());

            var form = component.Find("form");

            Assert.NotNull(form);

            host.WaitForNextRender(() => form.Submit());

            await WaitForSavedToast(host, component);

            await DbActionAsync<ApplicationDbContext>(async context =>
            {
                var userId = tuple.Item1;
                var claim = await context.UserClaims.FirstOrDefaultAsync(t => t.UserId == userId);
                Assert.Null(claim);
            });
        }

        [Fact]
        public async Task SaveClicked_create_new_user()
        {
            CreateTestHost("Alice Smith",
                AuthorizationOptionsExtensions.WRITER,
                null,
                out TestHost host,
                out RenderedComponent<App> component,
                out MockHttpMessageHandler mockHttp);

            host.WaitForNextRender();

            var input = component.Find("#name");

            var userId = GenerateId();
            host.WaitForNextRender(() => input.Change(userId));

            var form = component.Find("form");

            host.WaitForNextRender(() => form.Submit());

            await WaitForSavedToast(host, component);

            await DbActionAsync<ApplicationDbContext>(async context =>
            {
                var user = await context.Users.FirstOrDefaultAsync(u => u.UserName == userId);
                Assert.NotNull(user);
            });
        }

        [Fact]
        public async Task SaveClicked_should_update_user()
        {
            var tuple = await SetupPage();
            var host = tuple.Item2;
            var component = tuple.Item3;

            var input = component.Find("#email");

            Assert.NotNull(input);

            var expected = "test@exemple.com";
            host.WaitForNextRender(() => input.Change(expected));

            var markup = component.GetMarkup();

            Assert.Contains(expected, markup);

            var form = component.Find("form");

            Assert.NotNull(form);

            host.WaitForNextRender(() => form.Submit());

            await WaitForSavedToast(host, component);

            await DbActionAsync<ApplicationDbContext>(async context =>
            {
                var user = await context.Users.FirstOrDefaultAsync(u => u.Id == tuple.Item1);
                Assert.Equal(expected, user.Email);
            });
        }

        [Fact]
        public async Task DeleteClicked_should_remove_user()
        {
            var tuple = await SetupPage();
            var userId = tuple.Item1;
            var host = tuple.Item2;
            var component = tuple.Item3;

            var input = component.Find("#delete-entity input");

            host.WaitForNextRender(() => input.Change(userId));

            var confirm = component.Find("#delete-entity button.btn-danger");

            host.WaitForNextRender(() => confirm.Click());

            await WaitForDeletedToast(host, component);

            await DbActionAsync<ApplicationDbContext>(async context =>
            {
                var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                Assert.Null(user);
            });
        }


        private async Task<Tuple<string, TestHost, RenderedComponent<App>>> SetupPage()
        {
            var userId = GenerateId();
            await CreateTestEntity(userId);

            var roleManager = Fixture.Sut.Host.Services.GetRequiredService<RoleManager<IdentityRole>>();
            var role = await roleManager.FindByNameAsync("filtered");
            if (role == null)
            {
                var roleResult = await roleManager.CreateAsync(new IdentityRole
                {
                    Name = "filtered"
                });
                Assert.True(roleResult.Succeeded);
            }
            var manager = Fixture.Sut.Host.Services.GetRequiredService<UserManager<ApplicationUser>>();
            var user = await manager.FindByIdAsync(userId);
            var result = await manager.AddToRoleAsync(user, "filtered");
            Assert.True(result.Succeeded);

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
                    UserName = userId,
                    NormalizedUserName = userId.ToUpper()
                });
                context.UserClaims.Add(new IdentityUserClaim<string>
                {
                    ClaimType = "filtered",
                    ClaimValue = "filtered",
                    UserId = userId
                });
                context.UserTokens.Add(new IdentityUserToken<string>
                {
                    UserId = userId,
                    LoginProvider = "filtered",
                    Name = "filtered",
                    Value = "filtered"
                });
                context.UserLogins.Add(new IdentityUserLogin<string>
                {
                    UserId = userId,
                    LoginProvider = "filtered",
                    ProviderDisplayName = "filtered",
                    ProviderKey = GenerateId()
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
