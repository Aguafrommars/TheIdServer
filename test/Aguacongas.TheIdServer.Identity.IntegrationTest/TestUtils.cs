// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
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
            Startup startup = null;
            var webHostBuilder = new WebHostBuilder()
                .UseEnvironment("Development")
                .UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration
                    .ReadFrom.Configuration(hostingContext.Configuration))
                .ConfigureAppConfiguration(builder =>
                {
                    builder.AddJsonFile(Path.Combine(Environment.CurrentDirectory, @"..\..\..\..\..\src\Aguacongas.TheIdServer\appsettings.json"));
                    builder.AddJsonFile(Path.Combine(Environment.CurrentDirectory, @"appsettings.Test.json"), true);
                    if (configurationOverrides != null)
                    {
                        builder.AddInMemoryCollection(configurationOverrides);
                    }
                })
                .ConfigureServices((context, services) =>
                {
                    startup = new Startup(context.Configuration, context.HostingEnvironment);
                    configureServices?.Invoke(services);
                    startup.ConfigureServices(services);
                    services.AddSingleton<TestUserService>()
                        .AddMvc().AddApplicationPart(startup.GetType().Assembly);
                    configureServices?.Invoke(services);
                })
                .Configure(builder =>
                {
                    builder.Use(async (context, next) =>
                    {
                        var testService = context.RequestServices.GetRequiredService<TestUserService>();
                        context.User = testService.User;
                        await next();
                    });
                    startup.Configure(builder);
                });

            var testServer = new TestServer(webHostBuilder);

            return testServer;
        }

    }
}
