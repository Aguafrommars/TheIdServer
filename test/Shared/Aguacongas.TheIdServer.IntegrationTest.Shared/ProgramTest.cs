// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.TheIdServer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.TheIdServer.IntegrationTest
{
    public class ProgramTest
    {
        [Fact(Skip = "failed on CI")]
        public async Task StartAsync_should_seed_data()
        {
            var connectionString = @$"Data Source=(LocalDb)\MSSQLLocalDB;database=TheIdServer.Test.SeedData{Guid.NewGuid()};trusted_connection=yes;";

            var provider = new ServiceCollection()
                .AddDbContext<ConfigurationDbContext>(option => option.UseSqlServer(connectionString))
                .AddDbContext<ApplicationDbContext>(option => option.UseSqlServer(connectionString))
                .BuildServiceProvider();

            await Program.Main(new string[]
            {
                "--ConnectionStrings:DefaultConnection",
                connectionString,
                "--DbType",
                "SqlServer",
                "--environment",
                "Development",
                "/seed"
            }).ConfigureAwait(false);

            using var scope = provider.CreateScope();
            using var is4DbContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
            using var appDbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            try
            {
                Assert.NotEmpty(is4DbContext.Apis);
                Assert.NotEmpty(is4DbContext.Clients);
                Assert.NotEmpty(is4DbContext.Identities);

                Assert.NotEmpty(appDbContext.Users);
                Assert.NotEmpty(appDbContext.Roles);
            }
            finally
            {
                await is4DbContext.Database.EnsureDeletedAsync().ConfigureAwait(false);
            }
        }

    }
}
