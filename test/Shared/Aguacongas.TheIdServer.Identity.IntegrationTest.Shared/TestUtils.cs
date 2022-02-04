// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.TheIdServer.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;

namespace Aguacongas.TheIdServer.Identity.IntegrationTest
{
    public static class TestUtils
    {

        public static TestServer CreateTestServer(
                    Action<IServiceCollection> configureServices = null,
                    IEnumerable<KeyValuePair<string, string>> configurationOverrides = null)
        {
            var webHostBuilder = new WebHostBuilder()
                .UseEnvironment("Development")
                .UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration
                    .ReadFrom.Configuration(hostingContext.Configuration))
                .ConfigureServices((context, services) =>
                {
                    configureServices?.Invoke(services);

                    var configurationManager = new ConfigurationManager();
#if DUENDE
                    configurationManager.AddJsonFile(Path.Combine(Environment.CurrentDirectory, @"..\..\..\..\..\..\src\Aguacongas.TheIdServer.Duende\appsettings.json"));
#else
                    configurationManager.AddJsonFile(Path.Combine(Environment.CurrentDirectory, @"..\..\..\..\..\..\src\Aguacongas.TheIdServer.IS4\appsettings.json"));
#endif
                    configurationManager.AddJsonFile(Path.Combine(Environment.CurrentDirectory, @"appsettings.Test.json"), true);
                    if (configurationOverrides != null)
                    {
                        configurationManager.AddInMemoryCollection(configurationOverrides);
                    }

                    var isProxy = configurationManager.GetValue<bool>("Proxy");
                    var dbType = configurationManager.GetValue<DbTypes>("DbType");

                    services.AddTheIdServer(configurationManager);
                    services.AddSingleton<TestUserService>()
                        .AddMvc().AddApplicationPart(typeof(Config).Assembly);
                    configureServices?.Invoke(services);
                })
                .Configure((context, builder) =>
                {
                    builder.Use(async (context, next) =>
                    {
                        var testService = context.RequestServices.GetRequiredService<TestUserService>();
                        context.User = testService.User;
                        await next();
                    });
                    
                    builder.UseTheIdServer(context.HostingEnvironment, context.Configuration);
                });

            var testServer = new TestServer(webHostBuilder);

            return testServer;
        }

    }
}
