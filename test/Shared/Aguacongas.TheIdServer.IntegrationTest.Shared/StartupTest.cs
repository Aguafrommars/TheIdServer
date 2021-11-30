// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.KeysRotation;
using Aguacongas.IdentityServer.KeysRotation.RavenDb;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.Models;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Moq;
using Raven.Client.Documents;
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
            var documentStoreMock = new Mock<IDocumentStore>();
            var sessionMock = new Mock<IAsyncDocumentSession>();
            var advancedMock = new Mock<IAsyncAdvancedSessionOperations>();
            sessionMock.SetupGet(m => m.Advanced).Returns(advancedMock.Object);
            documentStoreMock.Setup(m => m.OpenAsyncSession(It.IsAny<SessionOptions>())).Returns(sessionMock.Object);
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
                        ["Seed"] = "false"
                    });
                })
                .ConfigureServices((context, services) =>
                {                    
                    services.AddSingleton(p => documentStoreMock.Object);
                    services.AddTheIdServer(context.Configuration);
                    services.AddSingleton(p => documentStoreMock.Object);
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
            var documentStoreMock = new Mock<IDocumentStore>();
            var sessionMock = new Mock<IAsyncDocumentSession>();
            var advancedMock = new Mock<IAsyncAdvancedSessionOperations>();
            sessionMock.SetupGet(m => m.Advanced).Returns(advancedMock.Object);
            documentStoreMock.Setup(m => m.OpenAsyncSession(It.IsAny<SessionOptions>())).Returns(sessionMock.Object);
            using var sut = new HostBuilder()
                .ConfigureAppConfiguration(builder =>
                {
                    builder.AddJsonFile(Path.Combine(Environment.CurrentDirectory, @"appsettings.json"));
                    builder.AddJsonFile(Path.Combine(Environment.CurrentDirectory, @"appsettings.Test.json"), true);
                    builder.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        ["DbType"] = DbTypes.RavenDb.ToString(),
                        ["IdentityServer:Key:StorageKind"] = StorageKind.MongoDb.ToString(),
                        ["DataProtectionOptions:StorageKind"] = StorageKind.MongoDb.ToString(),
                        ["Seed"] = "false"
                    });
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton(p => documentStoreMock.Object);
                    services.AddTheIdServer(context.Configuration);
                    services.AddSingleton(p => documentStoreMock.Object);
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
    }
}
