// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Sqlite.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Xunit;

namespace Aguacongas.TheIdServer.Test
{
    public class DbContextOptionsBuilderExtensionsTest
    {
        [Fact]
        public void UseDatabaseFromConfiguration_should_add_in_memory_options()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                ["ConnectionStrings:DefaultConnection"] = Guid.NewGuid().ToString(),
                ["DbType"] = "InMemory",
            }).Build();
            var sut = new DbContextOptionsBuilder();

            sut.UseDatabaseFromConfiguration(configuration);

            Assert.Contains(sut.Options.Extensions, e => typeof(InMemoryOptionsExtension) == e.GetType());
        }

        [Fact]
        public void UseDatabaseFromConfiguration_should_add_sqlite_options()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                ["ConnectionStrings:DefaultConnection"] = Guid.NewGuid().ToString(),
                ["DbType"] = "Sqlite",
            }).Build();
            var sut = new DbContextOptionsBuilder();

            sut.UseDatabaseFromConfiguration(configuration);

            Assert.Contains(sut.Options.Extensions, e => typeof(SqliteOptionsExtension) == e.GetType());
        }


        [Fact]
        public void UseDatabaseFromConfiguration_should_add_sql_server_options()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                ["ConnectionStrings:DefaultConnection"] = Guid.NewGuid().ToString(),
                ["DbType"] = "SqlServer",
            }).Build();
            var sut = new DbContextOptionsBuilder();

            sut.UseDatabaseFromConfiguration(configuration);

            Assert.Contains(sut.Options.Extensions, e => typeof(SqlServerOptionsExtension) == e.GetType());
        }
    }
}
