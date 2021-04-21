// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.TheIdServer.Authentication;
using Aguacongas.TheIdServer.Data;
using Aguacongas.TheIdServer.Models;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Aguacongas.TheIdServer.IntegrationTest
{
    public class ApiFixture : IDisposable
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

        public ApiFixture()
        {
            var dbName = Guid.NewGuid().ToString();
            Sut = TestUtils.CreateTestServer(
                services =>
                {
                    services.AddLogging(configure => configure.AddProvider(_testLoggerProvider))
                    .AddIdentityServer4AdminEntityFrameworkStores(options =>
                        options.UseInMemoryDatabase(dbName))
                    .AddIdentityProviderStore()
                    .AddConfigurationEntityFrameworkStores<SchemeDefinition>(options =>
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

        /// <summary>
        /// Performs a scopped action on <see cref="ApplicationDbContext"/>
        /// </summary>
        /// <param name="action">The action to perform</param>
        /// <returns></returns>
        public Task DbActionAsync<T>(Func<T, Task> action) where T : DbContext
        {
            var services = Sut.Host.Services;
            using var scope = services.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<T>();
            return action(context);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Sut?.Dispose();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }

    [CollectionDefinition("api collection")]
    public class ApiCollection : ICollectionFixture<ApiFixture>
    {

    }
}
