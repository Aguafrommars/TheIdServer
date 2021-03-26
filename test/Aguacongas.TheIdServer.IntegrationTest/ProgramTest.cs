// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.TheIdServer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.TheIdServer.IntegrationTest
{
    public class ProgramTest
    {
        [Fact]
        public async Task StartAsync_should_seed_data()
        {
            var connectionString = @"Data Source=(LocalDb)\MSSQLLocalDB;database=TheIdServer.Test.SeedData;trusted_connection=yes;";

            var provider = new ServiceCollection()
                .AddDbContext<ConfigurationDbContext>(option => option.UseSqlServer(connectionString))
                .AddDbContext<ApplicationDbContext>(option => option.UseSqlServer(connectionString))
                .BuildServiceProvider();

            using var scope1 = provider.CreateScope();
            using var is4DbContext1 = scope1.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
            is4DbContext1.Database.EnsureDeleted();

            await Program.StartAsync(new string[]
            {
                "--ConnectionStrings:DefaultConnection",
                connectionString,
                "--environment",
                "Development",
                "/seed"
            });

            using var scope2 = provider.CreateScope();
            using var is4DbContext2 = scope2.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
            using var appDbContext = scope2.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            Assert.NotEmpty(is4DbContext2.Apis);
            Assert.NotEmpty(is4DbContext2.Clients);
            Assert.NotEmpty(is4DbContext2.Identities);

            Assert.NotEmpty(appDbContext.Users);
            Assert.NotEmpty(appDbContext.Roles);
        }
    }
}
