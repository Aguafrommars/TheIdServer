// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.IdentityServer.Abstractions;
using Aguacongas.IdentityServer.Admin.Services;
using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.TheIdServer.Data;
using Aguacongas.TheIdServer.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using Xunit.Abstractions;

namespace Aguacongas.TheIdServer.Identity.IntegrationTest
{
    public class TheIdServerTestFixture
    {
        private readonly TestLoggerProvider _testLoggerProvider = new();
        /// <summary>
        /// Gets the system under test
        /// </summary>
        public TestServer Sut { get; }

        public ITestOutputHelper TestOutputHelper
        {
            get { return _testLoggerProvider.TestOutputHelper; }
            set { _testLoggerProvider.TestOutputHelper = value; }
        }

        public ILoggerProvider LoggerProvider
        {
            get
            {
                return _testLoggerProvider;
            }
        }

        public TheIdServerTestFixture()
        {
            var dbName = Guid.NewGuid().ToString();
            var factory = new WebApplicationFactory<AccountController>()
                .WithWebHostBuilder(webHostBuilder =>
                {
                    webHostBuilder.ConfigureAppConfiguration(configuration =>
                    {
                        configuration.AddJsonFile(Path.Combine(Environment.CurrentDirectory, @"appsettings.Test.json"), true);
                    })
                    .ConfigureServices(services =>
                    {
                        services.AddSingleton<TestUserService>()
                            .AddMvc().AddApplicationPart(typeof(Config).Assembly);
                        services.AddSingleton<HubConnectionFactory>()
                        .AddTransient<IProviderClient, ProviderClient>()
                        .AddTheIdServerAdminEntityFrameworkStores(options =>
                            options.UseInMemoryDatabase(dbName))
                        .AddIdentityProviderStore()
                        .AddConfigurationEntityFrameworkStores(options =>
                            options.UseInMemoryDatabase(dbName))
                        .AddOperationalEntityFrameworkStores(options =>
                            options.UseInMemoryDatabase(dbName));
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
                });
            Sut = factory.Server;

            using var scope = factory.Services.CreateScope();
            using var identityContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
            identityContext.Database.EnsureCreated();
            using var operationalContext = scope.ServiceProvider.GetRequiredService<OperationalDbContext>();
            operationalContext.Database.EnsureCreated();
            using var appContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            appContext.Database.EnsureCreated();
        }
    }
}
