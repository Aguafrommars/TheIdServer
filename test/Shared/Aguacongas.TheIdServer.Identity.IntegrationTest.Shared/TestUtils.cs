// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
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
                .ConfigureAppConfiguration(builder =>
                {
#if DUENDE
                    builder.AddJsonFile(Path.Combine(Environment.CurrentDirectory, @"..\..\..\..\..\..\src\Aguacongas.TheIdServer.Duende\appsettings.json"));
#else
                    builder.AddJsonFile(Path.Combine(Environment.CurrentDirectory, @"..\..\..\..\..\..\src\Aguacongas.TheIdServer.IS4\appsettings.json"));
#endif
                    builder.AddJsonFile(Path.Combine(Environment.CurrentDirectory, @"appsettings.Test.json"), true);
                    if (configurationOverrides != null)
                    {
                        builder.AddInMemoryCollection(configurationOverrides);
                    }
                })
                .ConfigureServices((context, services) =>
                {
                    configureServices?.Invoke(services);
                    var configuration = context.Configuration;
                    var isProxy = configuration.GetValue<bool>("Proxy");
                    var dbType = configuration.GetValue<DbTypes>("DbType");

                    services.AddTheIdServer(configuration as ConfigurationManager);
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
