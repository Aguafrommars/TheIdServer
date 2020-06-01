using Aguacongas.AspNetCore.Authentication;
using Aguacongas.IdentityServer.Abstractions;
using Aguacongas.IdentityServer.Admin.Services;
using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.Admin.Hubs;
using Aguacongas.TheIdServer.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Xunit;
using Auth = Aguacongas.TheIdServer.Authentication;

namespace Aguacongas.TheIdServer.Test
{
    public class StartupTest
    {
        [Fact]
        public void ConfigureServices_should_add_default_services()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                ["ConnectionStrings:DefaultConnection"] = Guid.NewGuid().ToString(),
                ["DbType"] = "InMemory"
            }).Build();
            var environementMock = new Mock<IWebHostEnvironment>();
            var sut = new Startup(configuration, environementMock.Object);

            var services = new ServiceCollection();
            services.AddTransient<IConfiguration>(p => configuration);
            sut.ConfigureServices(services);

            var provider = services.BuildServiceProvider();

            var schemeChangeSubscriber = provider.GetService<ISchemeChangeSubscriber>();
            Assert.NotNull(schemeChangeSubscriber);
            Assert.Equal(typeof(SchemeChangeSubscriber<SchemeDefinition>), schemeChangeSubscriber.GetType());
            Assert.NotNull(provider.GetService<ApplicationDbContext>());
        }

        [Fact]
        public void ConfigureServices_should_add_services_for_proxy()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                ["PrivateServerAuthentication:ApiUrl"] = "https://localhost:7443/api",
                ["Proxy"] = "true"
            }).Build();

            var environementMock = new Mock<IWebHostEnvironment>();
            var sut = new Startup(configuration, environementMock.Object);

            var services = new ServiceCollection();
            services.AddTransient<IConfiguration>(p => configuration);
            sut.ConfigureServices(services);

            var provider = services.BuildServiceProvider();

            var schemeChangeSubscriber = provider.GetService<ISchemeChangeSubscriber>();
            Assert.NotNull(schemeChangeSubscriber);
            Assert.Equal(typeof(SchemeChangeSubscriber<Auth.SchemeDefinition>), schemeChangeSubscriber.GetType());
            Assert.Null(provider.GetService<ApplicationDbContext>());
        }

        [Fact]
        public void ConfigureServices_should_configure_signalR()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                ["ConnectionStrings:DefaultConnection"] = Guid.NewGuid().ToString(),
                ["DbType"] = "InMemory",
                ["SignalR:UseMessagePack"] = "true",
                ["SignalR:RedisConnectionString"] = "localhost:6379"
            }).Build();

            var environementMock = new Mock<IWebHostEnvironment>();
            var sut = new Startup(configuration, environementMock.Object);

            var services = new ServiceCollection();
            services.AddTransient<IConfiguration>(p => configuration);
            sut.ConfigureServices(services);

            var provider = services.BuildServiceProvider();

            var hubProtocolResolver = provider.GetServices<IHubProtocolResolver>();
            Assert.NotNull(hubProtocolResolver);
            var hubLifetimeManager = provider.GetServices<RedisHubLifetimeManager<ProviderHub>>();
            Assert.NotNull(hubLifetimeManager);
        }

        [Fact]
        public void Configure_should_configure_initial_data()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                ["ConnectionStrings:DefaultConnection"] = "Data source=./db.sql",
                ["DbType"] = "Sqlite",
                ["Migrate"] = "true",
                ["Seed"] = "true",
                ["SeedProvider"] = "true"
            }).Build();
            var environementMock = new Mock<IWebHostEnvironment>();
            var storeMock = new Mock<IDynamicProviderStore<SchemeDefinition>>();
            storeMock.SetupGet(m => m.SchemeDefinitions).Returns(Array.Empty<SchemeDefinition>().AsQueryable()).Verifiable();

            var sut = new Startup(configuration, environementMock.Object);

            using var host = WebHost.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    sut.ConfigureServices(services);
                    services.AddTransient(p => storeMock.Object);
                })
                .Configure(builder => sut.Configure(builder))
                .UseSerilog((hostingContext, configuration) =>
                        configuration.ReadFrom.Configuration(hostingContext.Configuration))
                .Build();

            host.Start();

            storeMock.Verify();
        }

        [Fact]
        public void Configure_should_load_provider_configuration()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                ["PrivateServerAuthentication:ApiUrl"] = "https://localhost:7443/api",
                ["Proxy"] = "true"
            }).Build();
            var environementMock = new Mock<IWebHostEnvironment>();
            var storeMock = new Mock<IDynamicProviderStore<Auth.SchemeDefinition>>();
            storeMock.SetupGet(m => m.SchemeDefinitions).Returns(Array.Empty<Auth.SchemeDefinition>().AsQueryable()).Verifiable();
            var culturestoreMock = new Mock<IAdminStore<Culture>>();
            culturestoreMock.Setup(m => m.GetAsync(It.IsAny<PageRequest>(), default)).ReturnsAsync(new PageResponse<Culture>
            {
                Items = Array.Empty<Culture>()
            });
            var sut = new Startup(configuration, environementMock.Object);
            using var host = WebHost.CreateDefaultBuilder()
                .ConfigureServices(services => 
                {
                    sut.ConfigureServices(services);
                    services.AddTransient(p => storeMock.Object);
                    services.AddTransient(p => culturestoreMock.Object);
                })
                .Configure(builder => sut.Configure(builder))
                .UseSerilog((hostingContext, configuration) =>
                        configuration.ReadFrom.Configuration(hostingContext.Configuration))
                .Build();

            host.Start();

            storeMock.Verify();
        }
    }
}
