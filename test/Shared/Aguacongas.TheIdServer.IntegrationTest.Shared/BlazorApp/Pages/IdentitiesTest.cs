// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.IdentityServer.Store.Entity;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using page = Aguacongas.TheIdServer.BlazorApp.Pages.Identities.Indenties;

namespace Aguacongas.TheIdServer.IntegrationTest.BlazorApp.Pages
{
    [Collection(BlazorAppCollection.Name)]
    public class IdentitiesTest : EntitiesPageTestBase<IdentityResource, page>
    {
        public override string Entities => "identities";
        public IdentitiesTest(TheIdServerFactory factory)
            : base(factory)
        {
        }

        protected override Task PopulateList()
        {
            return DbActionAsync<ConfigurationDbContext>(context =>
            {
                context.Identities.Add(new IdentityResource
                {
                    Id = GenerateId(),
                    DisplayName = "filtered"
                });

                return context.SaveChangesAsync();
            });
        }
    }
}
