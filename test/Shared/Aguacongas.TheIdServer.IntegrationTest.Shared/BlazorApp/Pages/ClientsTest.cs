// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.IdentityServer.Store.Entity;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using page = Aguacongas.TheIdServer.BlazorApp.Pages.Clients.Clients;

namespace Aguacongas.TheIdServer.IntegrationTest.BlazorApp.Pages
{
    [Collection(BlazorAppCollection.Name)]
    public class CientsTest : EntitiesPageTestBase<Client, page>
    {
        public override string Entities => "clients";
        public CientsTest(TheIdServerFactory factory)
            : base(factory)
        {
        }

        protected override Task PopulateList()
        {
            return DbActionAsync<ConfigurationDbContext>(context =>
            {
                context.Clients.Add(new Client
                {
                    Id = GenerateId(),
                    ProtocolType = "oidc",
                    ClientName = "filtered"
                });

                return context.SaveChangesAsync();
            });
        }
    }
}
