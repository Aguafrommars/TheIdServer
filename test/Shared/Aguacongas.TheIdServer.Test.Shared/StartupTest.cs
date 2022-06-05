// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.AspNetCore.Authentication;
using Aguacongas.IdentityServer.Abstractions;
using Aguacongas.IdentityServer.Admin.Configuration;
using Aguacongas.IdentityServer.Admin.Services;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.Admin.Hubs;
using Aguacongas.TheIdServer.Authentication;
using Aguacongas.TheIdServer.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
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
using System.Diagnostics.CodeAnalysis;
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
            var configuration = new ConfigurationManager();
            configuration.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["ConnectionStrings:DefaultConnection"] = Guid.NewGuid().ToString(),
                ["DbType"] = "InMemory"
            });
            var services = new ServiceCollection();
            services.AddTransient<IConfiguration>(p => configuration)
                .AddTheIdServer(configuration);

            var provider = services.BuildServiceProvider();

            var schemeChangeSubscriber = provider.GetService<ISchemeChangeSubscriber>();
            Assert.NotNull(schemeChangeSubscriber);
            Assert.Equal(typeof(SchemeChangeSubscriber<SchemeDefinition>), schemeChangeSubscriber.GetType());
        }

        [Fact]
        public void ConfigureServices_should_add_services_for_proxy()
        {
            var configuration = new ConfigurationManager();
            configuration.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["PrivateServerAuthentication:ApiUrl"] = "https://localhost:7443/api",
                ["Proxy"] = "true"
            });

            var environementMock = new Mock<IWebHostEnvironment>();
            
            var services = new ServiceCollection();
            services.AddTransient<IConfiguration>(p => configuration);
            services.AddTheIdServer(configuration);

            var provider = services.BuildServiceProvider();

            var schemeChangeSubscriber = provider.GetService<ISchemeChangeSubscriber>();
            Assert.NotNull(schemeChangeSubscriber);
            Assert.Equal(typeof(SchemeChangeSubscriber<Auth.SchemeDefinition>), schemeChangeSubscriber.GetType());
        }

        [Fact]
        public void ConfigureServices_should_configure_signalR()
        {
            var configuration = new ConfigurationManager();
            configuration.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["ConnectionStrings:DefaultConnection"] = Guid.NewGuid().ToString(),
                ["DbType"] = "InMemory",
                ["SignalR:UseMessagePack"] = "true",
                ["SignalR:RedisConnectionString"] = "localhost:6379"
            });

            var environementMock = new Mock<IWebHostEnvironment>();
            var services = new ServiceCollection();
            services.AddTransient<IConfiguration>(p => configuration);
            services.AddTheIdServer(configuration);

            var provider = services.BuildServiceProvider();

            var hubProtocolResolver = provider.GetServices<IHubProtocolResolver>();
            Assert.NotNull(hubProtocolResolver);
            var hubLifetimeManager = provider.GetServices<RedisHubLifetimeManager<ProviderHub>>();
            Assert.NotNull(hubLifetimeManager);
        }

#if !DUENDE
        [Fact(Skip = "fail")]
        public void Configure_should_configure_initial_data()
        {
            var configuration = new ConfigurationManager();
            configuration.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["ConnectionStrings:DefaultConnection"] = "Data source=./db.sql",
                ["DbType"] = "Sqlite",
                ["Migrate"] = "true",
                ["Seed"] = "true",
                ["SeedProvider"] = "true"
            });
            var environementMock = new Mock<IWebHostEnvironment>();
            var storeMock = new Mock<IDynamicProviderStore<SchemeDefinition>>();
            storeMock.SetupGet(m => m.SchemeDefinitions).Returns(Array.Empty<SchemeDefinition>().AsQueryable()).Verifiable();

#pragma warning disable CS0618 // Type or member is obsolete
            using var host = WebHost.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddTheIdServer(configuration);
                    services.AddTransient(p => storeMock.Object);
                })
                .Configure((context, builder) => builder.UseTheIdServer(context.HostingEnvironment, context.Configuration))
                .UseSerilog((hostingContext, configuration) =>
                        configuration.ReadFrom.Configuration(hostingContext.Configuration))
                .Build();
#pragma warning restore CS0618 // Type or member is obsolete

            host.Start();

            storeMock.Verify();
        }

        [Fact(Skip = "fail")]
        public void Configure_should_load_provider_configuration()
        {
            var configuration = new ConfigurationManager();
            configuration.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["PrivateServerAuthentication:ApiUrl"] = "https://localhost:7443/api",
                ["Proxy"] = "true"
            });
            var environementMock = new Mock<IWebHostEnvironment>();
            var storeMock = new Mock<IDynamicProviderStore<Auth.SchemeDefinition>>();
            storeMock.SetupGet(m => m.SchemeDefinitions).Returns(Array.Empty<Auth.SchemeDefinition>().AsQueryable()).Verifiable();
            var culturestoreMock = new Mock<IAdminStore<Culture>>();
            culturestoreMock.Setup(m => m.GetAsync(It.IsAny<PageRequest>(), default)).ReturnsAsync(new PageResponse<Culture>
            {
                Items = Array.Empty<Culture>()
            });
#pragma warning disable CS0618 // Type or member is obsolete
            using var host = WebHost.CreateDefaultBuilder()
                .ConfigureServices(services => 
                {
                    services.AddTheIdServer(configuration);
                    services.AddTransient(p => storeMock.Object);
                    services.AddTransient(p => culturestoreMock.Object);
                })
                .Configure((context, builder) => builder.UseTheIdServer(context.HostingEnvironment, context.Configuration))
                .UseSerilog((hostingContext, configuration) =>
                        configuration.ReadFrom.Configuration(hostingContext.Configuration))
                .Build();
#pragma warning restore CS0618 // Type or member is obsolete

            host.Start();

            storeMock.Verify();
        }
#endif

        [Theory]
        [InlineData(DbTypes.InMemory)]
        [InlineData(DbTypes.MySql)]
        [InlineData(DbTypes.Oracle)]
        [InlineData(DbTypes.PostgreSQL)]
        [InlineData(DbTypes.Sqlite)]
        [InlineData(DbTypes.SqlServer)]
        public void UseDatabaseFromConfiguration_should_configure_context_per_db_type(DbTypes dbTypes)
        {
            var configuration = new ConfigurationManager();
            configuration.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["ConnectionStrings:DefaultConnection"] = "invalid",
                ["DbType"] = dbTypes.ToString(),
                ["Migrate"] = "true",
            });
            var environementMock = new Mock<IWebHostEnvironment>();
            var storeMock = new Mock<IDynamicProviderStore<SchemeDefinition>>();
            storeMock.SetupGet(m => m.SchemeDefinitions).Returns(Array.Empty<SchemeDefinition>().AsQueryable()).Verifiable();

#pragma warning disable CS0618 // Type or member is obsolete
            using var host = WebHost.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddTheIdServer(configuration);
                    services.AddTransient(p => storeMock.Object);
                })
                .Configure((context, builder) => builder.UseTheIdServer(context.HostingEnvironment, context.Configuration))
                .UseSerilog((hostingContext, configuration) =>
                        configuration.ReadFrom.Configuration(hostingContext.Configuration))
                .Build();
#pragma warning restore CS0618 // Type or member is obsolete

            if (dbTypes != DbTypes.InMemory)
            {
                Assert.ThrowsAny<Exception>(() => host.Start());
            }            
        }

        [Fact]
        public void Configure_should_configure_data_protection_azure_storage()
        {
            var configuration = new ConfigurationManager();
            configuration.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["DataProtectionOptions:StorageKind"] = StorageKind.AzureStorage.ToString(),
                ["DataProtectionOptions:StorageConnectionString"] = "https://md-3r0d4kzc5jhz.blob.core.windows.net/s3vffgdlczdj/abcd?sv=2017-04-17&sr=b&si=e931bb4b-8a79-4119-b4bb-8b2c1b763369&sig=SIGNATURE_WILL_BE_HERE"
            });
            var environementMock = new Mock<IWebHostEnvironment>();
            var storeMock = new Mock<IDynamicProviderStore<SchemeDefinition>>();
            storeMock.SetupGet(m => m.SchemeDefinitions).Returns(Array.Empty<SchemeDefinition>().AsQueryable()).Verifiable();

#pragma warning disable CS0618 // Type or member is obsolete
            using var host = WebHost.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddTheIdServer(configuration);
                    services.AddTransient(p => storeMock.Object);
                })
                .Configure((context, builder) => builder.UseTheIdServer(context.HostingEnvironment, context.Configuration))
                .UseSerilog((hostingContext, configuration) =>
                        configuration.ReadFrom.Configuration(hostingContext.Configuration))
                .Build();
#pragma warning restore CS0618 // Type or member is obsolete

            Assert.Null(host.Services.GetService<IXmlRepository>());
        }

        [Fact]
        public void Configure_should_configure_data_protection_ef_storage()
        {
            var configuration = new ConfigurationManager();
            configuration.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["DataProtectionOptions:StorageKind"] = StorageKind.EntityFramework.ToString()
            });
            var environementMock = new Mock<IWebHostEnvironment>();
            var storeMock = new Mock<IDynamicProviderStore<SchemeDefinition>>();
            storeMock.SetupGet(m => m.SchemeDefinitions).Returns(Array.Empty<SchemeDefinition>().AsQueryable()).Verifiable();

#pragma warning disable CS0618 // Type or member is obsolete
            using var host = WebHost.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddTheIdServer(configuration);
                    services.AddTransient(p => storeMock.Object);
                })
                .Configure((context, builder) => builder.UseTheIdServer(context.HostingEnvironment, context.Configuration))
                .UseSerilog((hostingContext, configuration) =>
                        configuration.ReadFrom.Configuration(hostingContext.Configuration))
                .Build();
#pragma warning restore CS0618 // Type or member is obsolete

            Assert.Null(host.Services.GetService<IXmlRepository>());
        }

        [Fact]
        public void Configure_should_configure_data_protection_fs_storage()
        {
            var configuration = new ConfigurationManager();
            configuration.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["DataProtectionOptions:StorageKind"] = StorageKind.FileSystem.ToString(),
                ["DataProtectionOptions:StorageConnectionString"] = @"C:\test"
            });
            var environementMock = new Mock<IWebHostEnvironment>();
            var storeMock = new Mock<IDynamicProviderStore<SchemeDefinition>>();
            storeMock.SetupGet(m => m.SchemeDefinitions).Returns(Array.Empty<SchemeDefinition>().AsQueryable()).Verifiable();

#pragma warning disable CS0618 // Type or member is obsolete
            using var host = WebHost.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddTheIdServer(configuration);
                    services.AddTransient(p => storeMock.Object);
                })
                .Configure((context, builder) => builder.UseTheIdServer(context.HostingEnvironment, context.Configuration))
                .UseSerilog((hostingContext, configuration) =>
                        configuration.ReadFrom.Configuration(hostingContext.Configuration))
                .Build();
#pragma warning restore CS0618 // Type or member is obsolete

            Assert.Null(host.Services.GetService<IXmlRepository>());
        }

        [Fact]
        public void Configure_should_configure_data_protection_redis_storage()
        {
            var configuration = new ConfigurationManager();
            configuration.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["DataProtectionOptions:StorageKind"] = StorageKind.Redis.ToString(),
                ["DataProtectionOptions:StorageConnectionString"] = "localhost:6379"
            });
            var environementMock = new Mock<IWebHostEnvironment>();
            var storeMock = new Mock<IDynamicProviderStore<SchemeDefinition>>();
            storeMock.SetupGet(m => m.SchemeDefinitions).Returns(Array.Empty<SchemeDefinition>().AsQueryable()).Verifiable();

#pragma warning disable CS0618 // Type or member is obsolete
            using var host = WebHost.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddTheIdServer(configuration);
                    services.AddTransient(p => storeMock.Object);
                })
                .Configure((context, builder) => builder.UseTheIdServer(context.HostingEnvironment, context.Configuration))
                .UseSerilog((hostingContext, configuration) =>
                        configuration.ReadFrom.Configuration(hostingContext.Configuration))
                .Build();
#pragma warning restore CS0618 // Type or member is obsolete

            Assert.Null(host.Services.GetService<IXmlRepository>());
        }

        [Fact]
        public void Configure_should_configure_data_protection_registry_storage()
        {
            var configuration = new ConfigurationManager();
            configuration.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["DataProtectionOptions:StorageKind"] = StorageKind.Registry.ToString(),
                ["DataProtectionOptions:StorageConnectionString"] = @"SOFTWARE\Microsoft"
            });
            var environementMock = new Mock<IWebHostEnvironment>();
            var storeMock = new Mock<IDynamicProviderStore<SchemeDefinition>>();
            storeMock.SetupGet(m => m.SchemeDefinitions).Returns(Array.Empty<SchemeDefinition>().AsQueryable()).Verifiable();

#pragma warning disable CS0618 // Type or member is obsolete
            using var host = WebHost.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddTheIdServer(configuration);
                    services.AddTransient(p => storeMock.Object);
                })
                .Configure((context, builder) => builder.UseTheIdServer(context.HostingEnvironment, context.Configuration))
                .UseSerilog((hostingContext, configuration) =>
                        configuration.ReadFrom.Configuration(hostingContext.Configuration))
                .Build();
#pragma warning restore CS0618 // Type or member is obsolete

            Assert.Null(host.Services.GetService<IXmlRepository>());
        }

        [Fact]
        public void Configure_should_configure_data_protection_storage_azure_protection()
        {
            var configuration = new ConfigurationManager();
            configuration.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["DataProtectionOptions:StorageKind"] = StorageKind.None.ToString(),
                ["DataProtectionOptions:KeyProtectionOptions:KeyProtectionKind"] = KeyProtectionKind.AzureKeyVault.ToString(),
                ["DataProtectionOptions:KeyProtectionOptions:AzureKeyVaultKeyId"] = "http://test",
                ["DataProtectionOptions:KeyProtectionOptions:AzureKeyVaultClientId"] = "test",
                ["DataProtectionOptions:KeyProtectionOptions:AzureKeyVaultClientSecret"] = "test",
            });
            var environementMock = new Mock<IWebHostEnvironment>();
            var storeMock = new Mock<IDynamicProviderStore<SchemeDefinition>>();
            storeMock.SetupGet(m => m.SchemeDefinitions).Returns(Array.Empty<SchemeDefinition>().AsQueryable()).Verifiable();

#pragma warning disable CS0618 // Type or member is obsolete
            using var host = WebHost.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddTheIdServer(configuration);
                    services.AddTransient(p => storeMock.Object);
                })
                .Configure((context, builder) => builder.UseTheIdServer(context.HostingEnvironment, context.Configuration))
                .UseSerilog((hostingContext, configuration) =>
                        configuration.ReadFrom.Configuration(hostingContext.Configuration))
                .Build();
#pragma warning restore CS0618 // Type or member is obsolete

            Assert.Null(host.Services.GetService<IXmlRepository>());
        }

        [Fact]
        public void Configure_should_configure_data_protection_storage_dpapi_protection()
        {
            var configuration = new ConfigurationManager();
            configuration.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["DataProtectionOptions:StorageKind"] = StorageKind.None.ToString(),
                ["DataProtectionOptions:KeyProtectionOptions:KeyProtectionKind"] = KeyProtectionKind.WindowsDpApi.ToString()
            });
            var environementMock = new Mock<IWebHostEnvironment>();
            var storeMock = new Mock<IDynamicProviderStore<SchemeDefinition>>();
            storeMock.SetupGet(m => m.SchemeDefinitions).Returns(Array.Empty<SchemeDefinition>().AsQueryable()).Verifiable();

#pragma warning disable CS0618 // Type or member is obsolete
            using var host = WebHost.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddTheIdServer(configuration);
                    services.AddTransient(p => storeMock.Object);
                })
                .Configure((context, builder) => builder.UseTheIdServer(context.HostingEnvironment, context.Configuration))
                .UseSerilog((hostingContext, configuration) =>
                        configuration.ReadFrom.Configuration(hostingContext.Configuration))
                .Build();
#pragma warning restore CS0618 // Type or member is obsolete

            Assert.Null(host.Services.GetService<IXmlRepository>());
        }

        [Fact]
        public void Configure_should_configure_data_protection_storage_dpaping_protection()
        {
            var configuration = new ConfigurationManager();
            configuration.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["DataProtectionOptions:StorageKind"] = StorageKind.None.ToString(),
                ["DataProtectionOptions:KeyProtectionOptions:KeyProtectionKind"] = KeyProtectionKind.WindowsDpApiNg.ToString()
            });
            var environementMock = new Mock<IWebHostEnvironment>();
            var storeMock = new Mock<IDynamicProviderStore<SchemeDefinition>>();
            storeMock.SetupGet(m => m.SchemeDefinitions).Returns(Array.Empty<SchemeDefinition>().AsQueryable()).Verifiable();

#pragma warning disable CS0618 // Type or member is obsolete
            using var host = WebHost.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddTheIdServer(configuration);
                    services.AddTransient(p => storeMock.Object);
                })
                .Configure((context, builder) => builder.UseTheIdServer(context.HostingEnvironment, context.Configuration))
                .UseSerilog((hostingContext, configuration) =>
                        configuration.ReadFrom.Configuration(hostingContext.Configuration))
                .Build();
#pragma warning restore CS0618 // Type or member is obsolete

            Assert.Null(host.Services.GetService<IXmlRepository>());
        }

        [Fact]
        public void Configure_should_configure_data_protection_storage_dpaping_protection_with_cert()
        {
            var configuration = new ConfigurationManager();
            configuration.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["DataProtectionOptions:StorageKind"] = StorageKind.None.ToString(),
                ["DataProtectionOptions:KeyProtectionOptions:KeyProtectionKind"] = KeyProtectionKind.WindowsDpApiNg.ToString(),
                ["DataProtectionOptions:KeyProtectionOptions:WindowsDpApiNgCerticate"] = "test"
            });
            var environementMock = new Mock<IWebHostEnvironment>();
            var storeMock = new Mock<IDynamicProviderStore<SchemeDefinition>>();
            storeMock.SetupGet(m => m.SchemeDefinitions).Returns(Array.Empty<SchemeDefinition>().AsQueryable()).Verifiable();

#pragma warning disable CS0618 // Type or member is obsolete
            using var host = WebHost.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddTheIdServer(configuration);
                    services.AddTransient(p => storeMock.Object);
                })
                .Configure((context, builder) => builder.UseTheIdServer(context.HostingEnvironment, context.Configuration))
                .UseSerilog((hostingContext, configuration) =>
                        configuration.ReadFrom.Configuration(hostingContext.Configuration))
                .Build();
#pragma warning restore CS0618 // Type or member is obsolete

            Assert.Null(host.Services.GetService<IXmlRepository>());
        }

        [Fact]
        public void Configure_should_configure_data_protection_storage_dpaping_protection_with_sid()
        {
            var configuration = new ConfigurationManager();
            configuration.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["DataProtectionOptions:StorageKind"] = StorageKind.None.ToString(),
                ["DataProtectionOptions:KeyProtectionOptions:KeyProtectionKind"] = KeyProtectionKind.WindowsDpApiNg.ToString(),
                ["DataProtectionOptions:KeyProtectionOptions:WindowsDpApiNgSid"] = "test"
            });
            var environementMock = new Mock<IWebHostEnvironment>();
            var storeMock = new Mock<IDynamicProviderStore<SchemeDefinition>>();
            storeMock.SetupGet(m => m.SchemeDefinitions).Returns(Array.Empty<SchemeDefinition>().AsQueryable()).Verifiable();

#pragma warning disable CS0618 // Type or member is obsolete
            using var host = WebHost.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddTheIdServer(configuration);
                    services.AddTransient(p => storeMock.Object);
                })
                .Configure((context, builder) => builder.UseTheIdServer(context.HostingEnvironment, context.Configuration))
                .UseSerilog((hostingContext, configuration) =>
                        configuration.ReadFrom.Configuration(hostingContext.Configuration))
                .Build();
#pragma warning restore CS0618 // Type or member is obsolete

            Assert.Null(host.Services.GetService<IXmlRepository>());
        }

        [Fact]
        public void Configure_should_configure_data_protection_storage_cert_protection()
        {
            var configuration = new ConfigurationManager();
            configuration.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["DataProtectionOptions:StorageKind"] = StorageKind.None.ToString(),
                ["DataProtectionOptions:KeyProtectionOptions:KeyProtectionKind"] = KeyProtectionKind.X509.ToString(),
                ["DataProtectionOptions:KeyProtectionOptions:X509CertificateThumbprint"] = "test"
            });
            var environementMock = new Mock<IWebHostEnvironment>();
            var storeMock = new Mock<IDynamicProviderStore<SchemeDefinition>>();
            storeMock.SetupGet(m => m.SchemeDefinitions).Returns(Array.Empty<SchemeDefinition>().AsQueryable()).Verifiable();

#pragma warning disable CS0618 // Type or member is obsolete
            Assert.Throws<InvalidOperationException>(() => WebHost.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddTheIdServer(configuration);
                    services.AddTransient(p => storeMock.Object);
                })
                .Configure((context, builder) => builder.UseTheIdServer(context.HostingEnvironment, context.Configuration))
                .UseSerilog((hostingContext, configuration) =>
                        configuration.ReadFrom.Configuration(hostingContext.Configuration))
                .Build());
#pragma warning restore CS0618 // Type or member is obsolete
        }

        [Fact]
        public void Configure_should_configure_data_protection_storage_cert_file_protection()
        {
            var configuration = new ConfigurationManager();
            configuration.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["DataProtectionOptions:StorageKind"] = StorageKind.None.ToString(),
                ["DataProtectionOptions:KeyProtectionOptions:KeyProtectionKind"] = KeyProtectionKind.X509.ToString(),
                ["DataProtectionOptions:KeyProtectionOptions:X509CertificatePath"] = "theidserver.pfx",
                ["DataProtectionOptions:KeyProtectionOptions:X509CertificatePassword"] = "YourSecurePassword"
            });
            var environementMock = new Mock<IWebHostEnvironment>();
            var storeMock = new Mock<IDynamicProviderStore<SchemeDefinition>>();
            storeMock.SetupGet(m => m.SchemeDefinitions).Returns(Array.Empty<SchemeDefinition>().AsQueryable()).Verifiable();

#pragma warning disable CS0618 // Type or member is obsolete
            using var host = WebHost.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddTheIdServer(configuration);
                    services.AddTransient(p => storeMock.Object);
                })
                .Configure((context, builder) => builder.UseTheIdServer(context.HostingEnvironment, context.Configuration))
                .UseSerilog((hostingContext, configuration) =>
                        configuration.ReadFrom.Configuration(hostingContext.Configuration))
                .Build();
#pragma warning restore CS0618 // Type or member is obsolete

            Assert.Null(host.Services.GetService<IXmlRepository>());
        }

        [Fact]
        public void Configure_should_configure_data_protection_algorithms()
        {
            var configuration = new ConfigurationManager();
            configuration.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["DataProtectionOptions:AuthenticatedEncryptorConfiguration:EncryptionAlgorithm"] = EncryptionAlgorithm.AES_128_CBC.ToString(),
            });
            var environementMock = new Mock<IWebHostEnvironment>();
            var storeMock = new Mock<IDynamicProviderStore<SchemeDefinition>>();
            storeMock.SetupGet(m => m.SchemeDefinitions).Returns(Array.Empty<SchemeDefinition>().AsQueryable()).Verifiable();

#pragma warning disable CS0618 // Type or member is obsolete
            using var host = WebHost.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddTheIdServer(configuration);
                    services.AddTransient(p => storeMock.Object);
                })
                .Configure((context, builder) => builder.UseTheIdServer(context.HostingEnvironment, context.Configuration))
                .UseSerilog((hostingContext, configuration) =>
                        configuration.ReadFrom.Configuration(hostingContext.Configuration))
                .Build();
#pragma warning restore CS0618 // Type or member is obsolete

            Assert.Null(host.Services.GetService<IXmlRepository>());
        }

        [Fact]
        public void Configure_should_configure_keys_rotation_azure_storage()
        {
            var configuration = new ConfigurationManager();
            configuration.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["IdentityServer:Key:Type"] = KeyKinds.KeysRotation.ToString(),
                ["IdentityServer:Key:StorageKind"] = StorageKind.AzureStorage.ToString(),
                ["IdentityServer:Key:StorageConnectionString"] = "https://azure.com?sv=test"
            });
            var environementMock = new Mock<IWebHostEnvironment>();
            var storeMock = new Mock<IDynamicProviderStore<SchemeDefinition>>();
            storeMock.SetupGet(m => m.SchemeDefinitions).Returns(Array.Empty<SchemeDefinition>().AsQueryable()).Verifiable();

#pragma warning disable CS0618 // Type or member is obsolete
            using var host = WebHost.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddTheIdServer(configuration);
                    services.AddTransient(p => storeMock.Object);
                })
                .Configure((context, builder) => builder.UseTheIdServer(context.HostingEnvironment, context.Configuration))
                .UseSerilog((hostingContext, configuration) =>
                        configuration.ReadFrom.Configuration(hostingContext.Configuration))
                .Build();
#pragma warning restore CS0618 // Type or member is obsolete

            Assert.Null(host.Services.GetService<IXmlRepository>());
        }

        [Fact]
        public void Configure_should_configure_keys_rotation_ef_storage()
        {
            var configuration = new ConfigurationManager();
            configuration.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["IdentityServer:Key:Type"] = KeyKinds.KeysRotation.ToString(),
                ["IdentityServer:Key:StorageKind"] = StorageKind.EntityFramework.ToString()
            });
            var environementMock = new Mock<IWebHostEnvironment>();
            var storeMock = new Mock<IDynamicProviderStore<SchemeDefinition>>();
            storeMock.SetupGet(m => m.SchemeDefinitions).Returns(Array.Empty<SchemeDefinition>().AsQueryable()).Verifiable();

#pragma warning disable CS0618 // Type or member is obsolete
            using var host = WebHost.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddTheIdServer(configuration);
                    services.AddTransient(p => storeMock.Object);
                })
                .Configure((context, builder) => builder.UseTheIdServer(context.HostingEnvironment, context.Configuration))
                .UseSerilog((hostingContext, configuration) =>
                        configuration.ReadFrom.Configuration(hostingContext.Configuration))
                .Build();
#pragma warning restore CS0618 // Type or member is obsolete

            Assert.Null(host.Services.GetService<IXmlRepository>());
        }

        [Fact]
        public void Configure_should_configure_keys_rotation_fs_storage()
        {
            var configuration = new ConfigurationManager();
            configuration.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["IdentityServer:Key:Type"] = KeyKinds.KeysRotation.ToString(),
                ["IdentityServer:Key:StorageKind"] = StorageKind.FileSystem.ToString(),
                ["IdentityServer:Key:StorageConnectionString"] = @"C:\test"
            });
            var environementMock = new Mock<IWebHostEnvironment>();
            var storeMock = new Mock<IDynamicProviderStore<SchemeDefinition>>();
            storeMock.SetupGet(m => m.SchemeDefinitions).Returns(Array.Empty<SchemeDefinition>().AsQueryable()).Verifiable();

#pragma warning disable CS0618 // Type or member is obsolete
            using var host = WebHost.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddTheIdServer(configuration);
                    services.AddTransient(p => storeMock.Object);
                })
                .Configure((context, builder) => builder.UseTheIdServer(context.HostingEnvironment, context.Configuration))
                .UseSerilog((hostingContext, configuration) =>
                        configuration.ReadFrom.Configuration(hostingContext.Configuration))
                .Build();
#pragma warning restore CS0618 // Type or member is obsolete

            Assert.Null(host.Services.GetService<IXmlRepository>());
        }

        [Fact]
        public void Configure_should_configure_keys_rotation_redis_storage()
        {
            var configuration = new ConfigurationManager();
            configuration.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["IdentityServer:Key:Type"] = KeyKinds.KeysRotation.ToString(),
                ["IdentityServer:Key:StorageKind"] = StorageKind.Redis.ToString(),
                ["IdentityServer:Key:StorageConnectionString"] = "localhost:6379"
            });
            var environementMock = new Mock<IWebHostEnvironment>();
            var storeMock = new Mock<IDynamicProviderStore<SchemeDefinition>>();
            storeMock.SetupGet(m => m.SchemeDefinitions).Returns(Array.Empty<SchemeDefinition>().AsQueryable()).Verifiable();

#pragma warning disable CS0618 // Type or member is obsolete
            using var host = WebHost.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddTheIdServer(configuration); 
                    services.AddTransient(p => storeMock.Object);
                })
                .Configure((context, builder) => builder.UseTheIdServer(context.HostingEnvironment, context.Configuration))
                .UseSerilog((hostingContext, configuration) =>
                        configuration.ReadFrom.Configuration(hostingContext.Configuration))
                .Build();
#pragma warning restore CS0618 // Type or member is obsolete

            Assert.Null(host.Services.GetService<IXmlRepository>());
        }

        [Fact]
        public void Configure_should_configure_keys_rotation_storage_azure_protection()
        {
            var configuration = new ConfigurationManager();
            configuration.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["IdentityServer:Key:Type"] = KeyKinds.KeysRotation.ToString(),
                ["IdentityServer:Key:StorageKind"] = StorageKind.None.ToString(),
                ["IdentityServer:Key:KeyProtectionOptions:KeyProtectionKind"] = KeyProtectionKind.AzureKeyVault.ToString(),
                ["IdentityServer:Key:KeyProtectionOptions:AzureKeyVaultKeyId"] = "test",
                ["IdentityServer:Key:KeyProtectionOptions:AzureKeyVaultClientId"] = "test",
                ["IdentityServer:Key:KeyProtectionOptions:AzureKeyVaultClientSecret"] = "test",
            });
            var environementMock = new Mock<IWebHostEnvironment>();
            var storeMock = new Mock<IDynamicProviderStore<SchemeDefinition>>();
            storeMock.SetupGet(m => m.SchemeDefinitions).Returns(Array.Empty<SchemeDefinition>().AsQueryable()).Verifiable();

#pragma warning disable CS0618 // Type or member is obsolete
            using var host = WebHost.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddTheIdServer(configuration);
                    services.AddTransient(p => storeMock.Object);
                })
                .Configure((context, builder) => builder.UseTheIdServer(context.HostingEnvironment, context.Configuration))
                .UseSerilog((hostingContext, configuration) =>
                        configuration.ReadFrom.Configuration(hostingContext.Configuration))
                .Build();
#pragma warning restore CS0618 // Type or member is obsolete

            Assert.Null(host.Services.GetService<IXmlRepository>());
        }

        [Fact]
        public void Configure_should_configure_keys_rotation_storage_cert_protection()
        {
            var configuration = new ConfigurationManager();
            configuration.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["IdentityServer:Key:Type"] = KeyKinds.KeysRotation.ToString(),
                ["IdentityServer:Key:StorageKind"] = StorageKind.None.ToString(),
                ["IdentityServer:Key:KeyProtectionOptions:KeyProtectionKind"] = KeyProtectionKind.X509.ToString(),
                ["IdentityServer:Key:KeyProtectionOptions:X509CertificateThumbprint"] = "test"
            });
            var environementMock = new Mock<IWebHostEnvironment>();
            var storeMock = new Mock<IDynamicProviderStore<SchemeDefinition>>();
            storeMock.SetupGet(m => m.SchemeDefinitions).Returns(Array.Empty<SchemeDefinition>().AsQueryable()).Verifiable();

#pragma warning disable CS0618 // Type or member is obsolete
            Assert.Throws<InvalidOperationException>(() => WebHost.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddTheIdServer(configuration);
                    services.AddTransient(p => storeMock.Object);
                })
                .Configure((context, builder) => builder.UseTheIdServer(context.HostingEnvironment, context.Configuration))
                .UseSerilog((hostingContext, configuration) =>
                        configuration.ReadFrom.Configuration(hostingContext.Configuration))
                .Build());
#pragma warning restore CS0618 // Type or member is obsolete
        }

        [Fact]
        public void Configure_should_configure_keys_rotation_storage_cert_file_protection()
        {
            var configuration = new ConfigurationManager();
            configuration.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["IdentityServer:Key:Type"] = KeyKinds.KeysRotation.ToString(),
                ["IdentityServer:Key:StorageKind"] = StorageKind.None.ToString(),
                ["IdentityServer:Key:KeyProtectionOptions:KeyProtectionKind"] = KeyProtectionKind.X509.ToString(),
                ["IdentityServer:Key:KeyProtectionOptions:X509CertificatePath"] = "theidserver.pfx",
                ["IdentityServer:Key:KeyProtectionOptions:X509CertificatePassword"] = "YourSecurePassword"
            });
            var environementMock = new Mock<IWebHostEnvironment>();
            var storeMock = new Mock<IDynamicProviderStore<SchemeDefinition>>();
            storeMock.SetupGet(m => m.SchemeDefinitions).Returns(Array.Empty<SchemeDefinition>().AsQueryable()).Verifiable();

#pragma warning disable CS0618 // Type or member is obsolete
            using var host = WebHost.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddTheIdServer(configuration);
                    services.AddTransient(p => storeMock.Object);
                })
                .Configure((context, builder) => builder.UseTheIdServer(context.HostingEnvironment, context.Configuration))
                .UseSerilog((hostingContext, configuration) =>
                        configuration.ReadFrom.Configuration(hostingContext.Configuration))
                .Build();
#pragma warning restore CS0618 // Type or member is obsolete

            Assert.Null(host.Services.GetService<IXmlRepository>());
        }

    }
}
