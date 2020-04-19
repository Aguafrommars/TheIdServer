using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.IdentityServer.Abstractions;
using Aguacongas.IdentityServer.Admin.Services;
using Aguacongas.TheIdServer.Admin.Hubs;
using Aguacongas.TheIdServer.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
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
                ["ConnectionStrings:DefaultConnection"] = Guid.NewGuid().ToString(),
                ["PrivateServerAuthentication:ApiUrl"] = "https://localhost:7443/api",
                ["DbType"] = "InMemory",
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
    }
}
