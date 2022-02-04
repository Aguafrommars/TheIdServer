// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.Data;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using page = Aguacongas.TheIdServer.BlazorApp.Pages.Users.Users;

namespace Aguacongas.TheIdServer.IntegrationTest.BlazorApp.Pages
{
    [Collection(BlazorAppCollection.Name)]
    public class UsersTest : EntitiesPageTestBase<User, page>
    {
        public override string Entities => "users";

        public UsersTest(TheIdServerFactory factory)
            : base(factory)
        {
        }

        protected override Task PopulateList()
        {
            return DbActionAsync<ApplicationDbContext>(context =>
            {
                context.Users.Add(new User
                {
                    Id = GenerateId(),
                    UserName = "filtered",
                });

                return context.SaveChangesAsync();
            });
        }
    }
}
