// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.Data;
using System.Threading.Tasks;
using page = Aguacongas.TheIdServer.BlazorApp.Pages.Roles.Roles;

namespace Aguacongas.TheIdServer.IntegrationTest.BlazorApp.Pages
{
    public class RolesTest : EntitiesPageTestBase<Role, page>
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
