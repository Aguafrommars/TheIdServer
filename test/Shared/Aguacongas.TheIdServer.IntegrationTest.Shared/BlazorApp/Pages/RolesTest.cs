// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.Data;
using System.Threading.Tasks;
using Xunit;
using RolesPage = Aguacongas.TheIdServer.BlazorApp.Pages.Roles.Roles;

namespace Aguacongas.TheIdServer.IntegrationTest.BlazorApp.Pages
{
    [Collection(BlazorAppCollection.Name)]
    public class RolesTest : EntitiesPageTestBase<Role, RolesPage>
    {
        public override string Entities => "roles";

        public RolesTest(TheIdServerFactory factory)
            : base(factory)
        {
        }

        protected override Task PopulateList()
        {
            return DbActionAsync<ApplicationDbContext>(context =>
            {
                context.Roles.Add(new Role
                {
                    Id = GenerateId(),
                    Name = "filtered",
                });

                return context.SaveChangesAsync();
            });
        }
    }
}
