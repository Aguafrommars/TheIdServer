using Aguacongas.TheIdServer.Models;
using Microsoft.Extensions.Configuration;

namespace Microsoft.EntityFrameworkCore
{
    public static class DbContextOptionsBuilderExtensions
    {
        public static DbContextOptionsBuilder UseDatabaseFromConfiguration(this DbContextOptionsBuilder options, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var dbType = configuration.GetValue<DbTypes>("DbType");
            switch (dbType)
            {
                case DbTypes.InMemory:
                    options.UseInMemoryDatabase(connectionString);
                    break;
                case DbTypes.SqlServer:
                    options.UseSqlServer(connectionString);
                    break;
                case DbTypes.Sqlite:
                    options.UseSqlite(connectionString);
                    break;
            }
            return options;
        }
    }
}
