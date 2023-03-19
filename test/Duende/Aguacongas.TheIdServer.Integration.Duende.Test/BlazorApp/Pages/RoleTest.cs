// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.Data;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using RolePage = Aguacongas.TheIdServer.BlazorApp.Pages.Role.Role;

namespace Aguacongas.TheIdServer.IntegrationTest.BlazorApp.Pages
{
    [Collection(BlazorAppCollection.Name)]
    public class RoleTest : EntityPageTestBase<RolePage>
    {
        public override string Entity => "role";

        public RoleTest(TheIdServerFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task OnFilterChanged_should_filter_claims()
        {
            string roleId = await CreateRole();

            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY,
                roleId);

            var filterInput = component.Find("input[placeholder=\"filter\"]");

            Assert.NotNull(filterInput);

            await filterInput.TriggerEventAsync("oninput", new ChangeEventArgs
            {
                Value = roleId
            }).ConfigureAwait(false);

            Assert.DoesNotContain("filtered", component.Markup);
        }

        [Fact]
        public async Task SaveClick_should_create_role()
        {
            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY,
                null);

            var input = WaitForNode(component, "#name");

            var roleName = GenerateId();

            await input.ChangeAsync(new ChangeEventArgs
            {
                Value = roleName
            }).ConfigureAwait(false);
            
            var form = component.Find("form");
            Assert.NotNull(form);

            await form.SubmitAsync().ConfigureAwait(false);

            await DbActionAsync<ApplicationDbContext>(async context =>
            {
                var role = await context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
                Assert.NotNull(role);
            });
        }


        [Fact]
        public async Task DeleteButtonClick_should_delete_Role()
        {
            string roleId = await CreateRole();

            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY,
                roleId);

            var input = WaitForNode(component, "#delete-entity input");

            await input.ChangeAsync(new ChangeEventArgs
            {
                Value = roleId
            }).ConfigureAwait(false);

            var confirm = component.Find("#delete-entity button.btn-danger");

            await confirm.ClickAsync(new MouseEventArgs()).ConfigureAwait(false);

            await DbActionAsync<ApplicationDbContext>(async context =>
            {
                var role = await context.Roles.FirstOrDefaultAsync(a => a.Id == roleId);
                Assert.Null(role);
            });
        }

        [Fact]
        public async Task AddRoleClaim_should_add_claim_to_role()
        {
            string roleId = await CreateRole();

            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY,
                roleId);

            var addButton = WaitForNode(component, "#claims button");

            await addButton.ClickAsync(new MouseEventArgs()).ConfigureAwait(false);

            var rows = component.FindAll("#claims tr");

            Assert.NotNull(rows);

            var lastRow = rows[rows.Count - 1];
            var inputList = lastRow.QuerySelectorAll("input");

            Assert.NotEmpty(inputList);

            var expected = GenerateId();
            await inputList.First().ChangeAsync(new ChangeEventArgs
            {
                Value = expected
            }).ConfigureAwait(false);

            rows = component.FindAll("#claims tr");
            lastRow = rows[rows.Count - 1];
            inputList = lastRow.QuerySelectorAll("input");

            await inputList.Last().ChangeAsync(new ChangeEventArgs
            {
                Value = expected
            }).ConfigureAwait(false);

            var form = component.Find("form");

            Assert.NotNull(form);

            await form.SubmitAsync().ConfigureAwait(false);

            await DbActionAsync<ApplicationDbContext>(async context =>
            {
                var claim = await context.RoleClaims.FirstOrDefaultAsync(t => t.RoleId == roleId &&
                    t.ClaimType == expected &&
                    t.ClaimValue == expected);
                Assert.NotNull(claim);
            });
        }

        [Fact]
        public async Task UpdateRoleClaim_should_update_claim()
        {
            string roleId = await CreateRole();

            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY,
                roleId);

            component.WaitForState(() => component.Markup.Contains("filtered"));

            var rows = WaitForAllNodes(component, "#claims tr");

            var lastRow = rows.Last();
            var inputList = lastRow.QuerySelectorAll("input");

            Assert.NotEmpty(inputList);

            var expected = GenerateId();

            await inputList.Last().ChangeAsync(new ChangeEventArgs
            {
                Value = expected
            }).ConfigureAwait(false);

            var form = component.Find("form");

            Assert.NotNull(form);

            await form.SubmitAsync().ConfigureAwait(false);

            await DbActionAsync<ApplicationDbContext>(async context =>
            {
                var claim = await context.RoleClaims.FirstOrDefaultAsync(t => t.RoleId == roleId &&
                    t.ClaimValue == expected);
                Assert.NotNull(claim);
            });
        }

        [Fact]
        public async Task DeleteRoleClaim_should_remove_claim_from_role()
        {
            string roleId = await CreateRole();

            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY,
                roleId);

            var button = WaitForNode(component, "#claims tr button");

            await button.ClickAsync(new MouseEventArgs()).ConfigureAwait(false);

            var form = component.Find("form");

            Assert.NotNull(form);

            await form.SubmitAsync().ConfigureAwait(false);

            await DbActionAsync<ApplicationDbContext>(async context =>
            {
                var claim = await context.RoleClaims.FirstOrDefaultAsync(t => t.RoleId == roleId);
                Assert.Null(claim);
            });
        }

        [Fact]
        public async Task WhenWriter_should_be_able_to_clone_entity()
        {
            string roleId = await CreateRole();

            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY,
                roleId,
                true);

            var input = WaitForNode(component, "#name");

            Assert.Contains(input.Attributes, a => a.Value == $"Clone of {roleId}");
        }

        private async Task<string> CreateRole()
        {
            var roleId = GenerateId();
            await DbActionAsync<ApplicationDbContext>(context =>
            {
                context.Roles.Add(new Role
                {
                    Id = roleId,
                    Name = roleId
                });
                context.RoleClaims.Add(new RoleClaim
                {
                    Id = Guid.NewGuid().ToString(),
                    RoleId = roleId,
                    ClaimType = "filtered",
                    ClaimValue = "filtered"
                });
                return context.SaveChangesAsync();
            });
            return roleId;
        }
    }
}
