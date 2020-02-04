using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.IdentityServer.Http.Store;
using Aguacongas.TheIdServer.Data;
using Aguacongas.TheIdServer.Models;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Aguacongas.TheIdServer.IntegrationTest
{
    public class ApiFixture : IDisposable
    {
        private readonly TestLoggerProvider _testLoggerProvider = new TestLoggerProvider();
        /// <summary>
        /// Gets the private server
        /// </summary>
        public TestServer PrivateServer { get; }

        /// <summary>
        /// Gets the public server
        /// </summary>
        public TestServer PublicServer { get; }

        public ITestOutputHelper TestOutputHelper 
        { 
            get { return _testLoggerProvider.TestOutputHelper; } 
            set { _testLoggerProvider.TestOutputHelper = value; }
        }

        public ApiFixture()
        {
            var dbName = Guid.NewGuid().ToString();
            PrivateServer = TestUtils.CreateTestServer(
                services =>
                {
                    services.AddLogging(configure => configure.AddProvider(_testLoggerProvider))
                    .AddDbContext<ApplicationDbContext>(options =>
                        options.UseInMemoryDatabase(dbName))
                    .AddIdentityServer4AdminEntityFrameworkStores<ApplicationUser, ApplicationDbContext>(options =>
                        options.UseInMemoryDatabase(dbName))
                    .AddOperationalEntityFrameworkStores(options =>
                        options.UseInMemoryDatabase(dbName))
                    .AddIdentityProviderStore();
                });

            PublicServer = TestUtils.CreateTestServer(
                services =>
                {
                    var client = PrivateServer.CreateClient();
                    services.AddConfigurationHttpStores(p =>
                    {
                        var hanlder = PrivateServer.CreateHandler();
                        var manager = new OAuthTokenManager(PrivateServer.CreateClient(), p.GetRequiredService<IOptions<AuthorizationOptions>>());
                        return Task.FromResult(new HttpClient(new OAuthDelegatingHandler(manager, hanlder)));
                    }, options =>
                    {
                        options.Authority = client.BaseAddress.ToString();
                    });
                });

            using var scope = PrivateServer.Host.Services.CreateScope();
            using var identityContext = scope.ServiceProvider.GetRequiredService<IdentityServerDbContext>();
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
        public Task DbActionAsync<T>(Func<T, Task> action) where T: DbContext
        {
            var services = PrivateServer.Host.Services;
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
                    PrivateServer?.Dispose();
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
