// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Aguacongas.IdentityServer.Admin.Services;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Linq;

namespace Aguacongas.TheIdServer
{
#pragma warning disable CA1052 // Static holder types should be Static or NotInheritable
#pragma warning disable S1118 // Utility classes should not have public constructors
    public class Program
#pragma warning restore S1118 // Utility classes should not have public constructors
#pragma warning restore CA1052 // Static holder types should be Static or NotInheritable
    {
        public static void Main(string[] args)
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

            CreateCertificate(host.Services);

            host.Run();
        }

        private static void CreateCertificate(IServiceProvider services)
        {
            var letsEncryptService = services.GetRequiredService<LetsEncryptService>();
            letsEncryptService.CreateCertificateAsync().GetAwaiter().GetResult();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                    .UseStartup<Startup>()
                    .UseSerilog((hostingContext, configuration) =>
                        configuration.ReadFrom.Configuration(hostingContext.Configuration));
        }
    }
}
