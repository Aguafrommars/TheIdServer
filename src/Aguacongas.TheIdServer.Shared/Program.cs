// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.TheIdServer.Areas.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Linq;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var seed = args.Any(x => x == "/seed");
            if (seed)
            {
                args = args.Except(new[] { "/seed" }).ToArray();
            }

            var host = CreateWebHostBuilder(args).Build();

            if (seed)
            {
                var config = host.Services.GetRequiredService<IConfiguration>();
                SeedData.EnsureSeedData(config);
                return;
            }

            await host.RunAsync().ConfigureAwait(false);
        }

        private static IHostBuilder CreateWebHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseSetting(
                            WebHostDefaults.HostingStartupAssembliesKey,
                            typeof(IdentityHostingStartup).Assembly.FullName)
                        .UseStartup<Startup>();
                })
                .UseSerilog((hostingContext, configuration) =>
                        configuration.ReadFrom.Configuration(hostingContext.Configuration));
        }
    }
}
