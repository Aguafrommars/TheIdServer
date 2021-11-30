// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.KeysRotation;
using Aguacongas.IdentityServer.KeysRotation.RavenDb;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.Models;
using IdentityModel.AspNetCore.OAuth2Introspection;
#if DUENDE
using Duende.IdentityServer.Services;
#else
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
#endif
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Moq;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using MongoDb = Aguacongas.IdentityServer.KeysRotation.MongoDb;

namespace Aguacongas.TheIdServer.IntegrationTest
{
    public class ServiceCollectionExtensionsTest
    {
        [Fact]
        public void ConfigureService_should_configure_ravendb_services()
        {
            var sessionMock = new Mock<IAsyncDocumentSession>();
            var advancedMock = new Mock<IAsyncAdvancedSessionOperations>();
            sessionMock.SetupGet(m => m.Advanced).Returns(advancedMock.Object);
            using var sut = new HostBuilder()
                .ConfigureAppConfiguration(builder =>
                {
                    builder.AddJsonFile(Path.Combine(Environment.CurrentDirectory, @"appsettings.json"));
                    builder.AddJsonFile(Path.Combine(Environment.CurrentDirectory, @"appsettings.Test.json"), true);
                    builder.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        ["DbType"] = DbTypes.RavenDb.ToString(),
                        ["IdentityServer:Key:StorageKind"] = StorageKind.RavenDb.ToString(),
                        ["DataProtectionOptions:StorageKind"] = StorageKind.RavenDb.ToString(),
                        ["RavenDbOptions:CertificatePath"] = string.Empty,
                        ["Seed"] = "false"
                    });
                })
                .ConfigureServices((context, services) =>
                {                    
                    services.AddTheIdServer(context.Configuration);
                }).Build();
                 
            var provider = sut.Services;
            Assert.NotNull(provider.GetService<IAdminStore<ApiClaim>>());
            var configureRotationOptions = provider.GetService<IConfigureOptions<KeyRotationOptions>>();
            var rotationOptions = new KeyRotationOptions();
            configureRotationOptions?.Configure(rotationOptions);
            Assert.IsType<RavenDbXmlRepository<KeyRotationKey>>(rotationOptions.XmlRepository);
            var configureManagementOptions = provider.GetService<IConfigureOptions<KeyManagementOptions>>();
            var managementOptions = new KeyRotationOptions();
            configureManagementOptions?.Configure(managementOptions);
            Assert.IsType<RavenDbXmlRepository<DataProtectionKey>>(managementOptions.XmlRepository);
        }

        [Fact]
        public void ConfigureService_should_configure_mongodb_services()
        {
            var sessionMock = new Mock<IAsyncDocumentSession>();
            var advancedMock = new Mock<IAsyncAdvancedSessionOperations>();
            sessionMock.SetupGet(m => m.Advanced).Returns(advancedMock.Object);
            using var sut = new HostBuilder()
                .ConfigureAppConfiguration(builder =>
                {
                    builder.AddJsonFile(Path.Combine(Environment.CurrentDirectory, @"appsettings.json"));
                    builder.AddJsonFile(Path.Combine(Environment.CurrentDirectory, @"appsettings.Test.json"), true);
                    builder.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        ["DbType"] = DbTypes.MongoDb.ToString(),
                        ["ConnectionStrings:DefaultConnection"] = "mongodb://localhost/test",
                        ["IdentityServer:Key:StorageKind"] = StorageKind.MongoDb.ToString(),
                        ["DataProtectionOptions:StorageKind"] = StorageKind.MongoDb.ToString(),
                        ["Seed"] = "false"
                    });
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddTheIdServer(context.Configuration);
                }).Build();

            var provider = sut.Services;
            Assert.NotNull(provider.GetService<IAdminStore<ApiClaim>>());
            var configureRotationOptions = provider.GetService<IConfigureOptions<KeyRotationOptions>>();
            var rotationOptions = new KeyRotationOptions();
            configureRotationOptions?.Configure(rotationOptions);
            Assert.IsType<MongoDb.MongoDbXmlRepository<MongoDb.KeyRotationKey>>(rotationOptions.XmlRepository);
            var configureManagementOptions = provider.GetService<IConfigureOptions<KeyManagementOptions>>();
            var managementOptions = new KeyRotationOptions();
            configureManagementOptions?.Configure(managementOptions);
            Assert.IsType<MongoDb.MongoDbXmlRepository<MongoDb.DataProtectionKey>>(managementOptions.XmlRepository);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ConfigureService_should_configure_proxy_services(bool disableStrictSll)
        {
            var sessionMock = new Mock<IAsyncDocumentSession>();
            var advancedMock = new Mock<IAsyncAdvancedSessionOperations>();
            sessionMock.SetupGet(m => m.Advanced).Returns(advancedMock.Object);
            using var sut = new HostBuilder()
                .ConfigureAppConfiguration(builder =>
                {
                    builder.AddJsonFile(Path.Combine(Environment.CurrentDirectory, @"appsettings.json"));
                    builder.AddJsonFile(Path.Combine(Environment.CurrentDirectory, @"appsettings.Test.json"), true);
                    builder.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        ["Proxy"] = "true",
                        ["DisableStrictSsl"]= $"{disableStrictSll}",
                        ["Seed"] = "false"
                    });
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddTheIdServer(context.Configuration);
                }).Build();

            var provider = sut.Services;
            Assert.NotNull(provider.GetService<IProfileService>());
            Assert.NotNull(provider.GetService<IAdminStore<Client>>());
            var postConfigureJwtBeareOpttions = provider.GetService<IPostConfigureOptions<JwtBearerOptions>>();
            Assert.NotNull(postConfigureJwtBeareOpttions);
            postConfigureJwtBeareOpttions?.PostConfigure("test", new JwtBearerOptions());

            var postConfigureOAuth2IntrospectionOptions = provider.GetService<IPostConfigureOptions<OAuth2IntrospectionOptions>>();
            Assert.NotNull(postConfigureOAuth2IntrospectionOptions);
            postConfigureOAuth2IntrospectionOptions?.PostConfigure("test", new OAuth2IntrospectionOptions
            {
                Authority = "https://localhost"
            });
        }

    }
}
