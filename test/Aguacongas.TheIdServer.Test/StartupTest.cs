// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.AspNetCore.Authentication;
using Aguacongas.IdentityServer.Abstractions;
using Aguacongas.IdentityServer.Admin.Configuration;
using Aguacongas.IdentityServer.Admin.Services;
using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.Admin.Hubs;
using Aguacongas.TheIdServer.Data;
using Aguacongas.TheIdServer.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.Repositories;
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

        [Theory]
        [InlineData(DbTypes.InMemory)]
        [InlineData(DbTypes.MySql)]
        [InlineData(DbTypes.Oracle)]
        [InlineData(DbTypes.PostgreSQL)]
        [InlineData(DbTypes.Sqlite)]
        [InlineData(DbTypes.SqlServer)]
        public void UseDatabaseFromConfiguration_should_configure_context_per_db_type(DbTypes dbTypes)
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                ["ConnectionStrings:DefaultConnection"] = "invalid",
                ["DbType"] = dbTypes.ToString(),
                ["Migrate"] = "true",
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

            if (dbTypes != DbTypes.InMemory)
            {
                Assert.ThrowsAny<Exception>(() => host.Start());
            }            
        }

        [Fact]
        public void Configure_should_configure_data_protection_azure_storage()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                ["DataProtectionOptions:StorageKind"] = StorageKind.AzureStorage.ToString(),
                ["DataProtectionOptions:StorageConnectionString"] = "https://md-3r0d4kzc5jhz.blob.core.windows.net/s3vffgdlczdj/abcd?sv=2017-04-17&sr=b&si=e931bb4b-8a79-4119-b4bb-8b2c1b763369&sig=SIGNATURE_WILL_BE_HERE"
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

            Assert.Null(host.Services.GetService<IXmlRepository>());
        }

        [Fact]
        public void Configure_should_configure_data_protection_ef_storage()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                ["DataProtectionOptions:StorageKind"] = StorageKind.EntityFramework.ToString()
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

            Assert.Null(host.Services.GetService<IXmlRepository>());
        }

        [Fact]
        public void Configure_should_configure_data_protection_fs_storage()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                ["DataProtectionOptions:StorageKind"] = StorageKind.FileSytem.ToString(),
                ["DataProtectionOptions:StorageConnectionString"] = @"C:\test"
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

            Assert.Null(host.Services.GetService<IXmlRepository>());
        }

        [Fact]
        public void Configure_should_configure_data_protection_redis_storage()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                ["DataProtectionOptions:StorageKind"] = StorageKind.Redis.ToString(),
                ["DataProtectionOptions:StorageConnectionString"] = "localhost:6379"
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

            Assert.Null(host.Services.GetService<IXmlRepository>());
        }

        [Fact]
        public void Configure_should_configure_data_protection_registry_storage()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                ["DataProtectionOptions:StorageKind"] = StorageKind.Registry.ToString(),
                ["DataProtectionOptions:StorageConnectionString"] = @"SOFTWARE\Microsoft"
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

            Assert.Null(host.Services.GetService<IXmlRepository>());
        }

        [Fact]
        public void Configure_should_configure_data_protection_storage_azure_protection()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                ["DataProtectionOptions:StorageKind"] = StorageKind.None.ToString(),
                ["DataProtectionOptions:KeyProtectionOptions:KeyProtectionKind"] = KeyProtectionKind.AzureKeyVault.ToString(),
                ["DataProtectionOptions:KeyProtectionOptions:AzureKeyVaultKeyId"] = "http://test",
                ["DataProtectionOptions:KeyProtectionOptions:AzureKeyVaultClientId"] = "test",
                ["DataProtectionOptions:KeyProtectionOptions:AzureKeyVaultClientSecret"] = "test",
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

            Assert.Null(host.Services.GetService<IXmlRepository>());
        }

        [Fact]
        public void Configure_should_configure_data_protection_storage_dpapi_protection()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                ["DataProtectionOptions:StorageKind"] = StorageKind.None.ToString(),
                ["DataProtectionOptions:KeyProtectionOptions:KeyProtectionKind"] = KeyProtectionKind.WindowsDpApi.ToString()
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

            Assert.Null(host.Services.GetService<IXmlRepository>());
        }

        [Fact]
        public void Configure_should_configure_data_protection_storage_dpaping_protection()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                ["DataProtectionOptions:StorageKind"] = StorageKind.None.ToString(),
                ["DataProtectionOptions:KeyProtectionOptions:KeyProtectionKind"] = KeyProtectionKind.WindowsDpApiNg.ToString()
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

            Assert.Null(host.Services.GetService<IXmlRepository>());
        }

        [Fact]
        public void Configure_should_configure_data_protection_storage_dpaping_protection_with_cert()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                ["DataProtectionOptions:StorageKind"] = StorageKind.None.ToString(),
                ["DataProtectionOptions:KeyProtectionOptions:KeyProtectionKind"] = KeyProtectionKind.WindowsDpApiNg.ToString(),
                ["DataProtectionOptions:KeyProtectionOptions:WindowsDpApiNgCerticate"] = "test"
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

            Assert.Null(host.Services.GetService<IXmlRepository>());
        }

        [Fact]
        public void Configure_should_configure_data_protection_storage_dpaping_protection_with_sid()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                ["DataProtectionOptions:StorageKind"] = StorageKind.None.ToString(),
                ["DataProtectionOptions:KeyProtectionOptions:KeyProtectionKind"] = KeyProtectionKind.WindowsDpApiNg.ToString(),
                ["DataProtectionOptions:KeyProtectionOptions:WindowsDpApiNgSid"] = "test"
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

            Assert.Null(host.Services.GetService<IXmlRepository>());
        }

        [Fact]
        public void Configure_should_configure_data_protection_storage_cert_protection()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                ["DataProtectionOptions:StorageKind"] = StorageKind.None.ToString(),
                ["DataProtectionOptions:KeyProtectionOptions:KeyProtectionKind"] = KeyProtectionKind.X509.ToString(),
                ["DataProtectionOptions:KeyProtectionOptions:X509CertificateThumbprint"] = "test"
            }).Build();
            var environementMock = new Mock<IWebHostEnvironment>();
            var storeMock = new Mock<IDynamicProviderStore<SchemeDefinition>>();
            storeMock.SetupGet(m => m.SchemeDefinitions).Returns(Array.Empty<SchemeDefinition>().AsQueryable()).Verifiable();

            var sut = new Startup(configuration, environementMock.Object);

            Assert.Throws<InvalidOperationException>(() => WebHost.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    sut.ConfigureServices(services);
                    services.AddTransient(p => storeMock.Object);
                })
                .Configure(builder => sut.Configure(builder))
                .UseSerilog((hostingContext, configuration) =>
                        configuration.ReadFrom.Configuration(hostingContext.Configuration))
                .Build());
        }

        [Fact]
        public void Configure_should_configure_data_protection_storage_cert_file_protection()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                ["DataProtectionOptions:StorageKind"] = StorageKind.None.ToString(),
                ["DataProtectionOptions:KeyProtectionOptions:KeyProtectionKind"] = KeyProtectionKind.X509.ToString(),
                ["DataProtectionOptions:KeyProtectionOptions:X509CertificatePath"] = "theidserver.pfx",
                ["DataProtectionOptions:KeyProtectionOptions:X509CertificatePassword"] = "YourSecurePassword"
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

            Assert.Null(host.Services.GetService<IXmlRepository>());
        }

        [Fact]
        public void Configure_should_configure_data_protection_algorithms()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                ["DataProtectionOptions:AuthenticatedEncryptorConfiguration:EncryptionAlgorithm"] = EncryptionAlgorithm.AES_128_CBC.ToString(),
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

            Assert.Null(host.Services.GetService<IXmlRepository>());
        }

        [Fact]
        public void Configure_should_configure_keys_rotation_azure_storage()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                ["IdentityServer:Key:Type"] = KeyKinds.KeysRotation.ToString(),
                ["IdentityServer:Key:StorageKind"] = StorageKind.AzureStorage.ToString(),
                ["IdentityServer:Key:StorageConnectionString"] = "https://azure.com?blobUri=test"
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

            Assert.Null(host.Services.GetService<IXmlRepository>());
        }

        [Fact]
        public void Configure_should_configure_keys_rotation_ef_storage()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                ["IdentityServer:Key:Type"] = KeyKinds.KeysRotation.ToString(),
                ["IdentityServer:Key:StorageKind"] = StorageKind.EntityFramework.ToString()
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

            Assert.Null(host.Services.GetService<IXmlRepository>());
        }

        [Fact]
        public void Configure_should_configure_keys_rotation_fs_storage()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                ["IdentityServer:Key:Type"] = KeyKinds.KeysRotation.ToString(),
                ["IdentityServer:Key:StorageKind"] = StorageKind.FileSytem.ToString(),
                ["IdentityServer:Key:StorageConnectionString"] = @"C:\test"
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

            Assert.Null(host.Services.GetService<IXmlRepository>());
        }

        [Fact]
        public void Configure_should_configure_keys_rotation_redis_storage()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                ["IdentityServer:Key:Type"] = KeyKinds.KeysRotation.ToString(),
                ["IdentityServer:Key:StorageKind"] = StorageKind.Redis.ToString(),
                ["IdentityServer:Key:StorageConnectionString"] = "localhost:6379"
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

            Assert.Null(host.Services.GetService<IXmlRepository>());
        }

        [Fact]
        public void Configure_should_configure_keys_rotation_storage_azure_protection()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                ["IdentityServer:Key:Type"] = KeyKinds.KeysRotation.ToString(),
                ["IdentityServer:Key:StorageKind"] = StorageKind.None.ToString(),
                ["IdentityServer:Key:KeyProtectionOptions:KeyProtectionKind"] = KeyProtectionKind.AzureKeyVault.ToString(),
                ["IdentityServer:Key:KeyProtectionOptions:AzureKeyVaultKeyId"] = "test",
                ["IdentityServer:Key:KeyProtectionOptions:AzureKeyVaultClientId"] = "test",
                ["IdentityServer:Key:KeyProtectionOptions:AzureKeyVaultClientSecret"] = "test",
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

            Assert.Null(host.Services.GetService<IXmlRepository>());
        }

        [Fact]
        public void Configure_should_configure_keys_rotation_storage_cert_protection()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                ["IdentityServer:Key:Type"] = KeyKinds.KeysRotation.ToString(),
                ["IdentityServer:Key:StorageKind"] = StorageKind.None.ToString(),
                ["IdentityServer:Key:KeyProtectionOptions:KeyProtectionKind"] = KeyProtectionKind.X509.ToString(),
                ["IdentityServer:Key:KeyProtectionOptions:X509CertificateThumbprint"] = "test"
            }).Build();
            var environementMock = new Mock<IWebHostEnvironment>();
            var storeMock = new Mock<IDynamicProviderStore<SchemeDefinition>>();
            storeMock.SetupGet(m => m.SchemeDefinitions).Returns(Array.Empty<SchemeDefinition>().AsQueryable()).Verifiable();

            var sut = new Startup(configuration, environementMock.Object);

            Assert.Throws<InvalidOperationException>(() => WebHost.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    sut.ConfigureServices(services);
                    services.AddTransient(p => storeMock.Object);
                })
                .Configure(builder => sut.Configure(builder))
                .UseSerilog((hostingContext, configuration) =>
                        configuration.ReadFrom.Configuration(hostingContext.Configuration))
                .Build());
        }

        [Fact]
        public void Configure_should_configure_keys_rotation_storage_cert_file_protection()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                ["IdentityServer:Key:Type"] = KeyKinds.KeysRotation.ToString(),
                ["IdentityServer:Key:StorageKind"] = StorageKind.None.ToString(),
                ["IdentityServer:Key:KeyProtectionOptions:KeyProtectionKind"] = KeyProtectionKind.X509.ToString(),
                ["IdentityServer:Key:KeyProtectionOptions:X509CertificatePath"] = "theidserver.pfx",
                ["IdentityServer:Key:KeyProtectionOptions:X509CertificatePassword"] = "YourSecurePassword"
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

            Assert.Null(host.Services.GetService<IXmlRepository>());
        }

    }
}
