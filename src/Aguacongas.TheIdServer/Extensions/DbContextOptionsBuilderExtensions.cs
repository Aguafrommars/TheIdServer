using Aguacongas.TheIdServer.Models;
using Microsoft.Extensions.Configuration;
using System;

namespace Microsoft.EntityFrameworkCore
{
    public static class DbContextOptionsBuilderExtensions
    {
        public static DbContextOptionsBuilder UseDatabaseFromConfiguration(this DbContextOptionsBuilder options, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var assembly = typeof(DbContextOptionsBuilderExtensions).Assembly;

            var dbType = configuration.GetValue<DbTypes>("DbType");
            switch (dbType)
            {
                case DbTypes.InMemory:
                    options.UseInMemoryDatabase(connectionString);
                    break;
                case DbTypes.SqlServer:
                    options.UseSqlServer(connectionString, sqlServerOptions => sqlServerOptions.MigrationsAssembly(assembly.FullName));
                    break;
                case DbTypes.Sqlite:
                    options.UseSqlite(connectionString, sqliteOptions => sqliteOptions.MigrationsAssembly(assembly.FullName));
                    break;
            }
            return options;
        }
    }
}
