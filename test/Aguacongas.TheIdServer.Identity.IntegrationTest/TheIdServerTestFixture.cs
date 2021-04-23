// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Abstractions;
using Aguacongas.IdentityServer.Admin.Services;
using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.TheIdServer.Authentication;
using Aguacongas.TheIdServer.Data;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Xunit.Abstractions;

namespace Aguacongas.TheIdServer.Identity.IntegrationTest
{
    public class TheIdServerTestFixture
    {
        private readonly TestLoggerProvider _testLoggerProvider = new TestLoggerProvider();
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
            Sut = TestUtils.CreateTestServer(
                services =>
                {
                    services.AddLogging(configure => configure.AddProvider(_testLoggerProvider))
                    .AddSingleton<HubConnectionFactory>()
                    .AddTransient<IProviderClient, ProviderClient>()
                    .AddIdentityServer4AdminEntityFrameworkStores(options =>
                        options.UseInMemoryDatabase(dbName))
                    .AddIdentityProviderStore()
                    .AddConfigurationEntityFrameworkStores(options =>
                        options.UseInMemoryDatabase(dbName))
                    .AddOperationalEntityFrameworkStores(options =>
                        options.UseInMemoryDatabase(dbName));
                });

            using var scope = Sut.Host.Services.CreateScope();
            using var identityContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
            identityContext.Database.EnsureCreated();
            using var operationalContext = scope.ServiceProvider.GetRequiredService<OperationalDbContext>();
            operationalContext.Database.EnsureCreated();
            using var appContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            appContext.Database.EnsureCreated();
        }
    }
}
