// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.AspNetCore.Authentication;
using Aguacongas.IdentityServer.Abstractions;
using Aguacongas.IdentityServer.Admin.Configuration;
using Aguacongas.IdentityServer.Admin.Services;
using Aguacongas.TheIdServer.Admin.Hubs;
using Aguacongas.TheIdServer.Authentication;
using Aguacongas.TheIdServer.Models;
using Aguacongas.TheIdServer.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
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

        [Theory]
        [InlineData(DbTypes.InMemory)]
        [InlineData(DbTypes.MySql)]
        [InlineData(DbTypes.Oracle)]
        [InlineData(DbTypes.PostgreSQL)]
        [InlineData(DbTypes.Sqlite)]
        [InlineData(DbTypes.SqlServer)]
        public void UseDatabaseFromConfiguration_should_configure_context_per_db_type(DbTypes dbTypes)
        {
            var environementMock = new Mock<IWebHostEnvironment>();
            var storeMock = new Mock<IDynamicProviderStore<SchemeDefinition>>();
            storeMock.SetupGet(m => m.SchemeDefinitions).Returns(Array.Empty<SchemeDefinition>().AsQueryable()).Verifiable();

            var factory = new WebApplicationFactory<AccountController>()
                .WithWebHostBuilder(webHostBuilder =>
                {
                    webHostBuilder.ConfigureAppConfiguration(configuration =>
                    {
                        configuration.AddInMemoryCollection(new Dictionary<string, string>
                        {
                            ["ConnectionStrings:DefaultConnection"] = "invalid",
                            ["DbType"] = dbTypes.ToString(),
                            ["Migrate"] = "true",
                        });
                    })
                    .ConfigureServices(services =>
                    {
                        services.AddTransient(p => storeMock.Object);
                    })
                    .Configure((context, builder) => builder.UseTheIdServer(context.HostingEnvironment, context.Configuration));
                });


            if (dbTypes != DbTypes.InMemory)
            {
                Assert.ThrowsAny<Exception>(() => factory.CreateClient());
            }            
        }

        [Fact]
        public void Configure_should_configure_data_protection_azure_storage()
        {
            var environementMock = new Mock<IWebHostEnvironment>();
            var storeMock = new Mock<IDynamicProviderStore<SchemeDefinition>>();
            storeMock.SetupGet(m => m.SchemeDefinitions).Returns(Array.Empty<SchemeDefinition>().AsQueryable()).Verifiable();

            var factory = new WebApplicationFactory<AccountController>()
                .WithWebHostBuilder(webHostBuilder =>
                {
                    webHostBuilder.ConfigureAppConfiguration(configuration =>
                    {
                        configuration.AddInMemoryCollection(new Dictionary<string, string>
                        {
                            ["DataProtectionOptions:StorageKind"] = StorageKind.AzureStorage.ToString(),
                            ["DataProtectionOptions:StorageConnectionString"] = "https://md-3r0d4kzc5jhz.blob.core.windows.net/s3vffgdlczdj/abcd?sv=2017-04-17&sr=b&si=e931bb4b-8a79-4119-b4bb-8b2c1b763369&sig=SIGNATURE_WILL_BE_HERE"
                        });
                    })
                    .ConfigureServices(services =>
                     {
                         services.AddTransient(p => storeMock.Object);
                     })
                    .Configure((context, builder) => builder.UseTheIdServer(context.HostingEnvironment, context.Configuration));
                });


            Assert.Null(factory.Services.GetService<IXmlRepository>());
        }

        [Fact]
        public void Configure_should_configure_data_protection_ef_storage()
        {
            var environementMock = new Mock<IWebHostEnvironment>();
            var storeMock = new Mock<IDynamicProviderStore<SchemeDefinition>>();
            storeMock.SetupGet(m => m.SchemeDefinitions).Returns(Array.Empty<SchemeDefinition>().AsQueryable()).Verifiable();

            var factory = new WebApplicationFactory<AccountController>()
                .WithWebHostBuilder(webHostBuilder =>
                {
                    webHostBuilder.ConfigureAppConfiguration(configuration =>
                    {
                        configuration.AddInMemoryCollection(new Dictionary<string, string>
                        {
                            ["DataProtectionOptions:StorageKind"] = StorageKind.EntityFramework.ToString()
                        });
                    })
                    .ConfigureServices(services =>
                    {
                        services.AddTransient(p => storeMock.Object);
                    })
                    .Configure((context, builder) => builder.UseTheIdServer(context.HostingEnvironment, context.Configuration));
                });

            Assert.Null(factory.Services.GetService<IXmlRepository>());
        }

        [Fact]
        public void Configure_should_configure_data_protection_fs_storage()
        {
            var environementMock = new Mock<IWebHostEnvironment>();
            var storeMock = new Mock<IDynamicProviderStore<SchemeDefinition>>();
            storeMock.SetupGet(m => m.SchemeDefinitions).Returns(Array.Empty<SchemeDefinition>().AsQueryable()).Verifiable();

            var factory = new WebApplicationFactory<AccountController>()
                .WithWebHostBuilder(webHostBuilder =>
                {
                    webHostBuilder.ConfigureAppConfiguration(configuration =>
                    {
                        configuration.AddInMemoryCollection(new Dictionary<string, string>
                        {
                            ["DataProtectionOptions:StorageKind"] = StorageKind.FileSystem.ToString(),
                            ["DataProtectionOptions:StorageConnectionString"] = @"C:\test"
                        });
                    })
                    .ConfigureServices(services =>
                    {
                        services.AddTransient(p => storeMock.Object);
                    })
                    .Configure((context, builder) => builder.UseTheIdServer(context.HostingEnvironment, context.Configuration));
                });

            Assert.Null(factory.Services.GetService<IXmlRepository>());
        }

        [Fact]
        public void Configure_should_configure_data_protection_redis_storage()
        {
            var environementMock = new Mock<IWebHostEnvironment>();
            var storeMock = new Mock<IDynamicProviderStore<SchemeDefinition>>();
            storeMock.SetupGet(m => m.SchemeDefinitions).Returns(Array.Empty<SchemeDefinition>().AsQueryable()).Verifiable();

            var factory = new WebApplicationFactory<AccountController>()
                .WithWebHostBuilder(webHostBuilder =>
                {
                    webHostBuilder.ConfigureAppConfiguration(configuration =>
                    {
                        configuration.AddInMemoryCollection(new Dictionary<string, string>
                        {
                            ["DataProtectionOptions:StorageKind"] = StorageKind.Redis.ToString(),
                            ["DataProtectionOptions:StorageConnectionString"] = "localhost:6379"
                        });
                    })
                    .ConfigureServices(services =>
                    {
                        services.AddTransient(p => storeMock.Object);
                    })
                    .Configure((context, builder) => builder.UseTheIdServer(context.HostingEnvironment, context.Configuration));
                });

            Assert.Null(factory.Services.GetService<IXmlRepository>());
        }

        [Fact]
        public void Configure_should_configure_data_protection_registry_storage()
        {
            var environementMock = new Mock<IWebHostEnvironment>();
            var storeMock = new Mock<IDynamicProviderStore<SchemeDefinition>>();
            storeMock.SetupGet(m => m.SchemeDefinitions).Returns(Array.Empty<SchemeDefinition>().AsQueryable()).Verifiable();

            var factory = new WebApplicationFactory<AccountController>()
                .WithWebHostBuilder(webHostBuilder =>
                {
                    webHostBuilder.ConfigureAppConfiguration(configuration =>
                    {
                        configuration.AddInMemoryCollection(new Dictionary<string, string>
                        {
                            ["DataProtectionOptions:StorageKind"] = StorageKind.Registry.ToString(),
                            ["DataProtectionOptions:StorageConnectionString"] = @"SOFTWARE\Microsoft"
                        });
                    })
                    .ConfigureServices(services =>
                    {
                        services.AddTransient(p => storeMock.Object);
                    })
                    .Configure((context, builder) => builder.UseTheIdServer(context.HostingEnvironment, context.Configuration));
                });

            Assert.Null(factory.Services.GetService<IXmlRepository>());
        }

        [Fact]
        public void Configure_should_configure_data_protection_storage_azure_protection()
        {
            var environementMock = new Mock<IWebHostEnvironment>();
            var storeMock = new Mock<IDynamicProviderStore<SchemeDefinition>>();
            storeMock.SetupGet(m => m.SchemeDefinitions).Returns(Array.Empty<SchemeDefinition>().AsQueryable()).Verifiable();

            var factory = new WebApplicationFactory<AccountController>()
                .WithWebHostBuilder(webHostBuilder =>
                {
                    webHostBuilder.ConfigureAppConfiguration(configuration =>
                    {
                        configuration.AddInMemoryCollection(new Dictionary<string, string>
                        {
                            ["DataProtectionOptions:StorageKind"] = StorageKind.None.ToString(),
                            ["DataProtectionOptions:KeyProtectionOptions:KeyProtectionKind"] = KeyProtectionKind.AzureKeyVault.ToString(),
                            ["DataProtectionOptions:KeyProtectionOptions:AzureKeyVaultKeyId"] = "http://test",
                            ["DataProtectionOptions:KeyProtectionOptions:AzureKeyVaultClientId"] = "test",
                            ["DataProtectionOptions:KeyProtectionOptions:AzureKeyVaultClientSecret"] = "test",
                        });
                    })
                    .ConfigureServices(services =>
                    {
                        services.AddTransient(p => storeMock.Object);
                    })
                    .Configure((context, builder) => builder.UseTheIdServer(context.HostingEnvironment, context.Configuration));
                });

            Assert.Null(factory.Services.GetService<IXmlRepository>());
        }

        [Fact]
        public void Configure_should_configure_data_protection_storage_dpapi_protection()
        {
            var environementMock = new Mock<IWebHostEnvironment>();
            var storeMock = new Mock<IDynamicProviderStore<SchemeDefinition>>();
            storeMock.SetupGet(m => m.SchemeDefinitions).Returns(Array.Empty<SchemeDefinition>().AsQueryable()).Verifiable();

            var factory = new WebApplicationFactory<AccountController>()
                .WithWebHostBuilder(webHostBuilder =>
                {
                    webHostBuilder.ConfigureAppConfiguration(configuration =>
                    {
                        configuration.AddInMemoryCollection(new Dictionary<string, string>
                        {
                            ["DataProtectionOptions:StorageKind"] = StorageKind.None.ToString(),
                            ["DataProtectionOptions:KeyProtectionOptions:KeyProtectionKind"] = KeyProtectionKind.WindowsDpApi.ToString()
                        });
                    })
                    .ConfigureServices(services =>
                    {
                        services.AddTransient(p => storeMock.Object);
                    })
                    .Configure((context, builder) => builder.UseTheIdServer(context.HostingEnvironment, context.Configuration));
                });

            Assert.Null(factory.Services.GetService<IXmlRepository>());
        }

        [Fact]
        public void Configure_should_configure_data_protection_storage_dpaping_protection()
        {
            var environementMock = new Mock<IWebHostEnvironment>();
            var storeMock = new Mock<IDynamicProviderStore<SchemeDefinition>>();
            storeMock.SetupGet(m => m.SchemeDefinitions).Returns(Array.Empty<SchemeDefinition>().AsQueryable()).Verifiable();

            var factory = new WebApplicationFactory<AccountController>()
                .WithWebHostBuilder(webHostBuilder =>
                {
                    webHostBuilder.ConfigureAppConfiguration(configuration =>
                    {
                        configuration.AddInMemoryCollection(new Dictionary<string, string>
                        {
                            ["DataProtectionOptions:StorageKind"] = StorageKind.None.ToString(),
                            ["DataProtectionOptions:KeyProtectionOptions:KeyProtectionKind"] = KeyProtectionKind.WindowsDpApiNg.ToString()
                        });
                    })
                    .ConfigureServices(services =>
                    {
                        services.AddTransient(p => storeMock.Object);
                    })
                    .Configure((context, builder) => builder.UseTheIdServer(context.HostingEnvironment, context.Configuration));
                });

            Assert.Null(factory.Services.GetService<IXmlRepository>());
        }

        [Fact]
        public void Configure_should_configure_data_protection_storage_dpaping_protection_with_cert()
        {
            var environementMock = new Mock<IWebHostEnvironment>();
            var storeMock = new Mock<IDynamicProviderStore<SchemeDefinition>>();
            storeMock.SetupGet(m => m.SchemeDefinitions).Returns(Array.Empty<SchemeDefinition>().AsQueryable()).Verifiable();

            var factory = new WebApplicationFactory<AccountController>()
                .WithWebHostBuilder(webHostBuilder =>
                {
                    webHostBuilder.ConfigureAppConfiguration(configuration =>
                    {
                        configuration.AddInMemoryCollection(new Dictionary<string, string>
                        {
                            ["DataProtectionOptions:StorageKind"] = StorageKind.None.ToString(),
                            ["DataProtectionOptions:KeyProtectionOptions:KeyProtectionKind"] = KeyProtectionKind.WindowsDpApiNg.ToString(),
                            ["DataProtectionOptions:KeyProtectionOptions:WindowsDpApiNgCerticate"] = "test"
                        });
                    })
                    .ConfigureServices(services =>
                    {
                        services.AddTransient(p => storeMock.Object);
                    })
                    .Configure((context, builder) => builder.UseTheIdServer(context.HostingEnvironment, context.Configuration));
                });

            Assert.Null(factory.Services.GetService<IXmlRepository>());
        }

        [Fact]
        public void Configure_should_configure_data_protection_storage_dpaping_protection_with_sid()
        {
            var environementMock = new Mock<IWebHostEnvironment>();
            var storeMock = new Mock<IDynamicProviderStore<SchemeDefinition>>();
            storeMock.SetupGet(m => m.SchemeDefinitions).Returns(Array.Empty<SchemeDefinition>().AsQueryable()).Verifiable();

            var factory = new WebApplicationFactory<AccountController>()
                .WithWebHostBuilder(webHostBuilder =>
                {
                    webHostBuilder.ConfigureAppConfiguration(configuration =>
                    {
                        configuration.AddInMemoryCollection(new Dictionary<string, string>
                        {
                            ["DataProtectionOptions:StorageKind"] = StorageKind.None.ToString(),
                            ["DataProtectionOptions:KeyProtectionOptions:KeyProtectionKind"] = KeyProtectionKind.WindowsDpApiNg.ToString(),
                            ["DataProtectionOptions:KeyProtectionOptions:WindowsDpApiNgSid"] = "test"
                        });
                    })
                    .ConfigureServices(services =>
                    {
                        services.AddTransient(p => storeMock.Object);
                    })
                    .Configure((context, builder) => builder.UseTheIdServer(context.HostingEnvironment, context.Configuration));
                });

            Assert.Null(factory.Services.GetService<IXmlRepository>());
        }

        [Fact]
        public void Configure_should_configure_data_protection_storage_cert_protection()
        {
            var environementMock = new Mock<IWebHostEnvironment>();
            var storeMock = new Mock<IDynamicProviderStore<SchemeDefinition>>();
            storeMock.SetupGet(m => m.SchemeDefinitions).Returns(Array.Empty<SchemeDefinition>().AsQueryable()).Verifiable();

            var factory = new WebApplicationFactory<AccountController>()
                .WithWebHostBuilder(webHostBuilder =>
                {
                    webHostBuilder.ConfigureAppConfiguration(configuration =>
                    {
                        configuration.AddInMemoryCollection(new Dictionary<string, string>
                        {
                            ["DataProtectionOptions:StorageKind"] = StorageKind.None.ToString(),
                            ["DataProtectionOptions:KeyProtectionOptions:KeyProtectionKind"] = KeyProtectionKind.X509.ToString(),
                            ["DataProtectionOptions:KeyProtectionOptions:X509CertificateThumbprint"] = "test"
                        });
                    })
                    .ConfigureServices(services =>
                    {
                        services.AddTransient(p => storeMock.Object);
                    })
                    .Configure((context, builder) => builder.UseTheIdServer(context.HostingEnvironment, context.Configuration));
                });

            var client = factory.CreateClient();
            Assert.NotNull(client);
        }

        [Fact]
        public void Configure_should_configure_data_protection_storage_cert_file_protection()
        {
            var environementMock = new Mock<IWebHostEnvironment>();
            var storeMock = new Mock<IDynamicProviderStore<SchemeDefinition>>();
            storeMock.SetupGet(m => m.SchemeDefinitions).Returns(Array.Empty<SchemeDefinition>().AsQueryable()).Verifiable();

            var factory = new WebApplicationFactory<AccountController>()
                .WithWebHostBuilder(webHostBuilder =>
                {
                    webHostBuilder.ConfigureAppConfiguration(configuration =>
                    {
                        configuration.AddInMemoryCollection(new Dictionary<string, string>
                        {
                            ["DataProtectionOptions:StorageKind"] = StorageKind.None.ToString(),
                            ["DataProtectionOptions:KeyProtectionOptions:KeyProtectionKind"] = KeyProtectionKind.X509.ToString(),
                            ["DataProtectionOptions:KeyProtectionOptions:X509CertificatePath"] = "theidserver.pfx",
                            ["DataProtectionOptions:KeyProtectionOptions:X509CertificatePassword"] = "YourSecurePassword"
                        });
                    })
                    .ConfigureServices(services =>
                    {
                        services.AddTransient(p => storeMock.Object);
                    })
                    .Configure((context, builder) => builder.UseTheIdServer(context.HostingEnvironment, context.Configuration));
                });

            Assert.Null(factory.Services.GetService<IXmlRepository>());
        }

        [Fact]
        public void Configure_should_configure_data_protection_algorithms()
        {
            var environementMock = new Mock<IWebHostEnvironment>();
            var storeMock = new Mock<IDynamicProviderStore<SchemeDefinition>>();
            storeMock.SetupGet(m => m.SchemeDefinitions).Returns(Array.Empty<SchemeDefinition>().AsQueryable()).Verifiable();

            var factory = new WebApplicationFactory<AccountController>()
                .WithWebHostBuilder(webHostBuilder =>
                {
                    webHostBuilder.ConfigureAppConfiguration(configuration =>
                    {
                        configuration.AddInMemoryCollection(new Dictionary<string, string>
                        {
                            ["DataProtectionOptions:AuthenticatedEncryptorConfiguration:EncryptionAlgorithm"] = EncryptionAlgorithm.AES_128_CBC.ToString(),
                        });
                    })
                    .ConfigureServices(services =>
                    {
                        services.AddTransient(p => storeMock.Object);
                    })
                    .Configure((context, builder) => builder.UseTheIdServer(context.HostingEnvironment, context.Configuration));
                });

            Assert.Null(factory.Services.GetService<IXmlRepository>());
        }

        [Fact]
        public void Configure_should_configure_keys_rotation_azure_storage()
        {
            var environementMock = new Mock<IWebHostEnvironment>();
            var storeMock = new Mock<IDynamicProviderStore<SchemeDefinition>>();
            storeMock.SetupGet(m => m.SchemeDefinitions).Returns(Array.Empty<SchemeDefinition>().AsQueryable()).Verifiable();

            var factory = new WebApplicationFactory<AccountController>()
                .WithWebHostBuilder(webHostBuilder =>
                {
                    webHostBuilder.ConfigureAppConfiguration(configuration =>
                    {
                        configuration.AddInMemoryCollection(new Dictionary<string, string>
                        {
                            ["IdentityServer:Key:Type"] = KeyKinds.KeysRotation.ToString(),
                            ["IdentityServer:Key:StorageKind"] = StorageKind.AzureStorage.ToString(),
                            ["IdentityServer:Key:StorageConnectionString"] = "https://azure.com?sv=test"
                        });
                    })
                    .ConfigureServices(services =>
                    {
                        services.AddTransient(p => storeMock.Object);
                    })
                    .Configure((context, builder) => builder.UseTheIdServer(context.HostingEnvironment, context.Configuration));
                });

            Assert.Null(factory.Services.GetService<IXmlRepository>());
        }

        [Fact]
        public void Configure_should_configure_keys_rotation_ef_storage()
        {
            var environementMock = new Mock<IWebHostEnvironment>();
            var storeMock = new Mock<IDynamicProviderStore<SchemeDefinition>>();
            storeMock.SetupGet(m => m.SchemeDefinitions).Returns(Array.Empty<SchemeDefinition>().AsQueryable()).Verifiable();

            var factory = new WebApplicationFactory<AccountController>()
                .WithWebHostBuilder(webHostBuilder =>
                {
                    webHostBuilder.ConfigureAppConfiguration(configuration =>
                    {
                        configuration.AddInMemoryCollection(new Dictionary<string, string>
                        {
                            ["IdentityServer:Key:Type"] = KeyKinds.KeysRotation.ToString(),
                            ["IdentityServer:Key:StorageKind"] = StorageKind.EntityFramework.ToString()
                        });
                    })
                    .ConfigureServices(services =>
                    {
                        services.AddTransient(p => storeMock.Object);
                    })
                    .Configure((context, builder) => builder.UseTheIdServer(context.HostingEnvironment, context.Configuration));
                });

            Assert.Null(factory.Services.GetService<IXmlRepository>());
        }

        [Fact]
        public void Configure_should_configure_keys_rotation_fs_storage()
        {
            var environementMock = new Mock<IWebHostEnvironment>();
            var storeMock = new Mock<IDynamicProviderStore<SchemeDefinition>>();
            storeMock.SetupGet(m => m.SchemeDefinitions).Returns(Array.Empty<SchemeDefinition>().AsQueryable()).Verifiable();

            var factory = new WebApplicationFactory<AccountController>()
                .WithWebHostBuilder(webHostBuilder =>
                {
                    webHostBuilder.ConfigureAppConfiguration(configuration =>
                    {
                        configuration.AddInMemoryCollection(new Dictionary<string, string>
                        {
                            ["IdentityServer:Key:Type"] = KeyKinds.KeysRotation.ToString(),
                            ["IdentityServer:Key:StorageKind"] = StorageKind.FileSystem.ToString(),
                            ["IdentityServer:Key:StorageConnectionString"] = @"C:\test"
                        });
                    })
                    .ConfigureServices(services =>
                    {
                        services.AddTransient(p => storeMock.Object);
                    })
                    .Configure((context, builder) => builder.UseTheIdServer(context.HostingEnvironment, context.Configuration));
                });

            Assert.Null(factory.Services.GetService<IXmlRepository>());
        }

        [Fact]
        public void Configure_should_configure_keys_rotation_redis_storage()
        {
            var environementMock = new Mock<IWebHostEnvironment>();
            var storeMock = new Mock<IDynamicProviderStore<SchemeDefinition>>();
            storeMock.SetupGet(m => m.SchemeDefinitions).Returns(Array.Empty<SchemeDefinition>().AsQueryable()).Verifiable();

            var factory = new WebApplicationFactory<AccountController>()
                .WithWebHostBuilder(webHostBuilder =>
                {
                    webHostBuilder.ConfigureAppConfiguration(configuration =>
                    {
                        configuration.AddInMemoryCollection(new Dictionary<string, string>
                        {
                            ["IdentityServer:Key:Type"] = KeyKinds.KeysRotation.ToString(),
                            ["IdentityServer:Key:StorageKind"] = StorageKind.Redis.ToString(),
                            ["IdentityServer:Key:StorageConnectionString"] = "localhost:6379"
                        });
                    })
                    .ConfigureServices(services =>
                    {
                        services.AddTransient(p => storeMock.Object);
                    })
                    .Configure((context, builder) => builder.UseTheIdServer(context.HostingEnvironment, context.Configuration));
                });

            Assert.Null(factory.Services.GetService<IXmlRepository>());
        }

        [Fact]
        public void Configure_should_configure_keys_rotation_storage_azure_protection()
        {
            var environementMock = new Mock<IWebHostEnvironment>();
            var storeMock = new Mock<IDynamicProviderStore<SchemeDefinition>>();
            storeMock.SetupGet(m => m.SchemeDefinitions).Returns(Array.Empty<SchemeDefinition>().AsQueryable()).Verifiable();

            var factory = new WebApplicationFactory<AccountController>()
                .WithWebHostBuilder(webHostBuilder =>
                {
                    webHostBuilder.ConfigureAppConfiguration(configuration =>
                    {
                        configuration.AddInMemoryCollection(new Dictionary<string, string>
                        {
                            ["IdentityServer:Key:Type"] = KeyKinds.KeysRotation.ToString(),
                            ["IdentityServer:Key:StorageKind"] = StorageKind.None.ToString(),
                            ["IdentityServer:Key:KeyProtectionOptions:KeyProtectionKind"] = KeyProtectionKind.AzureKeyVault.ToString(),
                            ["IdentityServer:Key:KeyProtectionOptions:AzureKeyVaultKeyId"] = "test",
                            ["IdentityServer:Key:KeyProtectionOptions:AzureKeyVaultClientId"] = "test",
                            ["IdentityServer:Key:KeyProtectionOptions:AzureKeyVaultClientSecret"] = "test",
                        });
                    })
                    .ConfigureServices(services =>
                    {
                        services.AddTransient(p => storeMock.Object);
                    })
                    .Configure((context, builder) => builder.UseTheIdServer(context.HostingEnvironment, context.Configuration));
                });

            Assert.Null(factory.Services.GetService<IXmlRepository>());
        }

        [Fact]
        public void Configure_should_configure_keys_rotation_storage_cert_protection()
        {
            var environementMock = new Mock<IWebHostEnvironment>();
            var storeMock = new Mock<IDynamicProviderStore<SchemeDefinition>>();
            storeMock.SetupGet(m => m.SchemeDefinitions).Returns(Array.Empty<SchemeDefinition>().AsQueryable()).Verifiable();

            var factory = new WebApplicationFactory<AccountController>()
                .WithWebHostBuilder(webHostBuilder =>
                {
                    webHostBuilder.ConfigureAppConfiguration(configuration =>
                    {
                        configuration.AddInMemoryCollection(new Dictionary<string, string>
                        {
                            ["IdentityServer:Key:Type"] = KeyKinds.KeysRotation.ToString(),
                            ["IdentityServer:Key:StorageKind"] = StorageKind.None.ToString(),
                            ["IdentityServer:Key:KeyProtectionOptions:KeyProtectionKind"] = KeyProtectionKind.X509.ToString(),
                            ["IdentityServer:Key:KeyProtectionOptions:X509CertificateThumbprint"] = "test"
                        });
                    })
                    .ConfigureServices(services =>
                    {
                        services.AddTransient(p => storeMock.Object);
                    })
                    .Configure((context, builder) => builder.UseTheIdServer(context.HostingEnvironment, context.Configuration));
                });

            Assert.NotNull(factory.CreateClient());
        }

        [Fact]
        public void Configure_should_configure_keys_rotation_storage_cert_file_protection()
        {
            var environementMock = new Mock<IWebHostEnvironment>();
            var storeMock = new Mock<IDynamicProviderStore<SchemeDefinition>>();
            storeMock.SetupGet(m => m.SchemeDefinitions).Returns(Array.Empty<SchemeDefinition>().AsQueryable()).Verifiable();

            var factory = new WebApplicationFactory<AccountController>()
                .WithWebHostBuilder(webHostBuilder =>
                {
                    webHostBuilder.ConfigureAppConfiguration(configuration =>
                    {
                        configuration.AddInMemoryCollection(new Dictionary<string, string>
                        {
                            ["IdentityServer:Key:Type"] = KeyKinds.KeysRotation.ToString(),
                            ["IdentityServer:Key:StorageKind"] = StorageKind.None.ToString(),
                            ["IdentityServer:Key:KeyProtectionOptions:KeyProtectionKind"] = KeyProtectionKind.X509.ToString(),
                            ["IdentityServer:Key:KeyProtectionOptions:X509CertificatePath"] = "theidserver.pfx",
                            ["IdentityServer:Key:KeyProtectionOptions:X509CertificatePassword"] = "YourSecurePassword"
                        });
                    })
                    .ConfigureServices(services =>
                    {
                        services.AddTransient(p => storeMock.Object);
                    })
                    .Configure((context, builder) => builder.UseTheIdServer(context.HostingEnvironment, context.Configuration));
                });

            Assert.Null(factory.Services.GetService<IXmlRepository>());
        }

    }
}
