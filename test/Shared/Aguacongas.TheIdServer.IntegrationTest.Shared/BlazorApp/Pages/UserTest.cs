// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.BlazorApp;
using Aguacongas.TheIdServer.Data;
using Aguacongas.TheIdServer.Models;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using page = Aguacongas.TheIdServer.BlazorApp.Pages.User.User;

namespace Aguacongas.TheIdServer.IntegrationTest.BlazorApp.Pages
{
    [Collection("api collection")]
    public class UserTest : EntityPageTestBase<page>
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
            var component = tuple.Item2;

            var filterInput = WaitForNode(component, "input[placeholder=\"filter\"]");

            await filterInput.TriggerEventAsync("oninput", new ChangeEventArgs
            {
                Value = userId
            }).ConfigureAwait(false);

            Assert.DoesNotContain("filtered", component.Markup);
        }

        [Fact]
        public async Task DeleteUserTokenClicked_should_remove_user_token()
        {
            var tuple = await SetupPage();
            var userId = tuple.Item1;
            var component = tuple.Item2;

            await DbActionAsync<ApplicationDbContext>(async context =>
            {
                var token = await context.UserTokens.FirstOrDefaultAsync(t => t.UserId == userId);
                Assert.NotNull(token);
            });

            var deleteButton = WaitForNode(component, "#external-logins-tokens button[type=button]");

            await deleteButton.ClickAsync(new MouseEventArgs()).ConfigureAwait(false);

            var form = component.Find("form");

            Assert.NotNull(form);

            await form.SubmitAsync().ConfigureAwait(false);

            var tokensDiv = component.Find("#external-logins-tokens");

            Assert.NotNull(tokensDiv);

            Assert.DoesNotContain("filtered", tokensDiv.ToMarkup());

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
            var component = tuple.Item2;

            await DbActionAsync<ApplicationDbContext>(async context =>
            {
                var login = await context.UserLogins.FirstOrDefaultAsync(t => t.UserId == userId);
                Assert.NotNull(login);
            });

            var deleteButton = WaitForNode(component, "#external-logins button[type=button]");

            await deleteButton.ClickAsync(new MouseEventArgs()).ConfigureAwait(false);

            var form = component.Find("form");

            Assert.NotNull(form);

            await form.SubmitAsync().ConfigureAwait(false);

            var tokensDiv = component.Find("#external-logins");

            Assert.NotNull(tokensDiv);

            Assert.DoesNotContain("filtered", tokensDiv.ToMarkup());

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
            var component = tuple.Item2;

            await DbActionAsync<OperationalDbContext>(async context =>
            {
                var consent = await context.UserConstents.FirstOrDefaultAsync(t => t.UserId == userId);
                Assert.NotNull(consent);
            });

            var deleteButton = WaitForNode(component, "#consents button[type=button]");

            await deleteButton.ClickAsync(new MouseEventArgs()).ConfigureAwait(false);

            var form = component.Find("form");

            Assert.NotNull(form);

            await form.SubmitAsync().ConfigureAwait(false);

            var tokensDiv = component.Find("#consents");

            Assert.NotNull(tokensDiv);

            Assert.DoesNotContain("filtered", tokensDiv.ToMarkup());

            await DbActionAsync<OperationalDbContext>(async context =>
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
            var component = tuple.Item2;

            await DbActionAsync<ApplicationDbContext>(async context =>
            {
                var role = await context.UserRoles.FirstOrDefaultAsync(t => t.UserId == userId);
                Assert.NotNull(role);
            });

            var deleteButton = WaitForNode(component, "#roles .input-group-append");

            await deleteButton.ClickAsync(new MouseEventArgs()).ConfigureAwait(false);

            var form = component.Find("form");

            Assert.NotNull(form);

            await form.SubmitAsync().ConfigureAwait(false);

            var tokensDiv = component.Find("#roles");

            Assert.NotNull(tokensDiv);

            Assert.DoesNotContain("filtered", tokensDiv.ToMarkup());

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
            var component = tuple.Item2;

            var roleId = GenerateId();
            await DbActionAsync<ApplicationDbContext>(context =>
            {
                context.Roles.Add(new Role
                {
                    Id = roleId,
                    Name = roleId,
                    NormalizedName = roleId.ToUpper()
                });

                return context.SaveChangesAsync();
            });

            var input = WaitForNode(component, "#roles .new-claim");

            await input.TriggerEventAsync("oninput", new ChangeEventArgs
                {
                    Value = roleId
                }).ConfigureAwait(false);

            var button = WaitForNode(component, ".dropdown-item");

            Assert.NotNull(button);

            await button.ClickAsync(new MouseEventArgs()).ConfigureAwait(false);

            Assert.Contains(roleId, component.Markup);

            var form = component.Find("form");

            Assert.NotNull(form);

            await form.SubmitAsync().ConfigureAwait(false);

            var rolessDiv = component.Find("#roles");

            Assert.NotNull(rolessDiv);

            Assert.Contains(roleId, rolessDiv.ToMarkup());

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
            var component = tuple.Item2;

            var addButton = WaitForNode(component, "#claims button");

            Assert.NotNull(addButton);

            await addButton.ClickAsync(new MouseEventArgs()).ConfigureAwait(false);

            var rows = component.FindAll("#claims tr");

            Assert.NotNull(rows);

            var lastRow = rows.Last();
            var inputList = lastRow.QuerySelectorAll("input");

            Assert.NotEmpty(inputList);

            var expected = GenerateId();
            await inputList.First().ChangeAsync(new ChangeEventArgs { Value = expected }).ConfigureAwait(false);

            rows = component.FindAll("#claims tr");
            lastRow = rows.Last();
            inputList = lastRow.QuerySelectorAll("input");

            await inputList.Last().ChangeAsync(new ChangeEventArgs { Value = expected }).ConfigureAwait(false);

            var form = component.Find("form");

            Assert.NotNull(form);

            await form.SubmitAsync().ConfigureAwait(false);

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
            var component = tuple.Item2;

            var rows = WaitForAllNodes(component, "#claims tr");

            var lastRow = rows.Last();
            var inputList = lastRow.QuerySelectorAll("input");

            Assert.NotEmpty(inputList);

            var expected = GenerateId();

            await inputList.Last().ChangeAsync(new ChangeEventArgs { Value = expected }).ConfigureAwait(false);

            var form = component.Find("form");

            Assert.NotNull(form);

            await form.SubmitAsync().ConfigureAwait(false);

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
            var component = tuple.Item2;

            var button = WaitForNode(component, "#claims tr button");

            Assert.NotNull(button);

            await button.ClickAsync(new MouseEventArgs()).ConfigureAwait(false);

            var form = component.Find("form");

            Assert.NotNull(form);

            await form.SubmitAsync().ConfigureAwait(false);

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
                SharedConstants.WRITERPOLICY,
                null,
                out IRenderedComponent<page> component);

            var input = WaitForNode(component, "#name");

            var userId = GenerateId();
            await input.ChangeAsync(new ChangeEventArgs { Value = userId }).ConfigureAwait(false);

            var form = component.Find("form");

            await form.SubmitAsync().ConfigureAwait(false);

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
            var component = tuple.Item2;

            var input = WaitForNode(component, "#email");

            var expected = "test@exemple.com";
            await input.ChangeAsync(new ChangeEventArgs { Value = expected }).ConfigureAwait(false);

            Assert.Contains(expected, component.Markup);

            var form = component.Find("form");

            Assert.NotNull(form);

            await form.SubmitAsync().ConfigureAwait(false);

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
            var component = tuple.Item2;

            var input = component.Find("#delete-entity input");

            await input.ChangeAsync(new ChangeEventArgs { Value = userId }).ConfigureAwait(false);

            var confirm = component.Find("#delete-entity button.btn-danger");

            await confirm.ClickAsync(new MouseEventArgs()).ConfigureAwait(false);

            await DbActionAsync<ApplicationDbContext>(async context =>
            {
                var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                Assert.Null(user);
            });
        }


        private async Task<Tuple<string, IRenderedComponent<page>>> SetupPage()
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
                SharedConstants.WRITERPOLICY,
                userId,
                out IRenderedComponent<page> component);

            return new Tuple<string, IRenderedComponent<page>>(userId, component);
        }
        private async Task CreateTestEntity(string userId)
        {
            await DbActionAsync<ApplicationDbContext>(context =>
            {
                context.Users.Add(new User
                {
                    Id = userId,
                    UserName = userId,
                    NormalizedUserName = userId.ToUpper(),
                    SecurityStamp = Guid.NewGuid().ToString()
                });
                context.UserClaims.Add(new UserClaim
                {
                    Id = Guid.NewGuid().ToString(),
                    ClaimType = "filtered",
                    ClaimValue = "filtered",
                    Issuer = ClaimsIdentity.DefaultIssuer,
                    UserId = userId
                });
                context.UserTokens.Add(new UserToken
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = userId,
                    LoginProvider = "filtered",
                    Name = "filtered",
                    Value = "filtered"
                });
                context.UserLogins.Add(new UserLogin
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = userId,
                    LoginProvider = "filtered",
                    ProviderDisplayName = "filtered",
                    ProviderKey = GenerateId()
                });
                return context.SaveChangesAsync();
            });
            var clientId = GenerateId();
            await DbActionAsync<ConfigurationDbContext>(context =>
            {
                context.Clients.Add(new Client
                {
                    Id = clientId,
                    ClientName = "filtered",
                    ProtocolType = "oidc"
                });
                return context.SaveChangesAsync();
            });
            await DbActionAsync<OperationalDbContext>(context =>
            {
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
