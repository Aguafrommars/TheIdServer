// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.KeysRotation;
using Aguacongas.IdentityServer.KeysRotation.RavenDb;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using IdentityModel.AspNetCore.OAuth2Introspection;
#if DUENDE
using Duende.IdentityServer.Services;
#else
using IdentityServer4.Services;
#endif
using Moq;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using MongoDb = Aguacongas.IdentityServer.KeysRotation.MongoDb;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using Aguacongas.IdentityServer.Abstractions;

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
                .ConfigureServices((context, services) =>
                {
                    var configurationManager = new ConfigurationManager();
                    configurationManager.AddJsonFile(Path.Combine(Environment.CurrentDirectory, @"appsettings.json"));
                    configurationManager.AddJsonFile(Path.Combine(Environment.CurrentDirectory, @"appsettings.Test.json"), true);
                    configurationManager.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        ["DbType"] = DbTypes.RavenDb.ToString(),
                        ["IdentityServer:Key:StorageKind"] = StorageKind.RavenDb.ToString(),
                        ["DataProtectionOptions:StorageKind"] = StorageKind.RavenDb.ToString(),
                        ["RavenDbOptions:CertificatePath"] = string.Empty,
                        ["Seed"] = "false"
                    });
                    services.AddTheIdServer(configurationManager);
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
                .ConfigureServices((context, services) =>
                {
                    var configurationManager = new ConfigurationManager();
                    configurationManager.AddJsonFile(Path.Combine(Environment.CurrentDirectory, @"appsettings.json"));
                    configurationManager.AddJsonFile(Path.Combine(Environment.CurrentDirectory, @"appsettings.Test.json"), true);
                    configurationManager.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        ["DbType"] = DbTypes.MongoDb.ToString(),
                        ["ConnectionStrings:DefaultConnection"] = "mongodb://localhost/test",
                        ["IdentityServer:Key:StorageKind"] = StorageKind.MongoDb.ToString(),
                        ["DataProtectionOptions:StorageKind"] = StorageKind.MongoDb.ToString(),
                        ["Seed"] = "false"
                    });
                    services.AddTheIdServer(configurationManager);
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
        [InlineData(true, null, null, null)]
        [InlineData(true, null, null, "test.test")]
        [InlineData(true, null, "test", null)]
        [InlineData(true, null, "test", "test.test")]
        [InlineData(true, "/providerhub", "test", null)]
        [InlineData(true, "/providerhub", "test", "test.test")]
        [InlineData(false, null, null, null)]
        [InlineData(false, null, null, "test.test")]
        [InlineData(false, null, "test", null)]
        [InlineData(false, null, "test", "test.test")]
        [InlineData(false, "/providerhub", "test", null)]
        [InlineData(false, "/providerhub", "test", "test.test")]
        public async Task ConfigureService_should_configure_proxy_services(bool disableStrictSll, string path, string otk, string token)
        {
            var sessionMock = new Mock<IAsyncDocumentSession>();
            var advancedMock = new Mock<IAsyncAdvancedSessionOperations>();
            sessionMock.SetupGet(m => m.Advanced).Returns(advancedMock.Object);
            using var sut = new HostBuilder()
                .ConfigureServices((context, services) =>
                {
                    var configurationMAnager = new ConfigurationManager();
                    configurationMAnager.AddJsonFile(Path.Combine(Environment.CurrentDirectory, @"appsettings.json"));
                    configurationMAnager.AddJsonFile(Path.Combine(Environment.CurrentDirectory, @"appsettings.Test.json"), true);
                    configurationMAnager.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        ["Proxy"] = "true",
                        ["DisableStrictSsl"] = $"{disableStrictSll}",
                        ["Seed"] = "false"
                    });
                    services.AddTheIdServer(configurationMAnager);
                }).Build();

            var provider = sut.Services;
            Assert.NotNull(provider.GetService<IProfileService>());
            Assert.NotNull(provider.GetService<IAdminStore<Client>>());
            var jwtBearerHandler = provider.GetService<JwtBearerHandler>();
            Assert.NotNull(jwtBearerHandler);
            var mockHeader = new Mock<IHeaderDictionary>();
            var queryCollection = new QueryCollection(new Dictionary<string, StringValues>
            {
                ["otk"] = otk
            });
            var mockOneTimeTokenRetriver = new Mock<IRetrieveOneTimeToken>();
            mockOneTimeTokenRetriver.Setup(m => m.GetOneTimeToken(It.IsAny<string>())).Returns(token);
            var requestServices = new ServiceCollection()
                .AddTransient(p => mockOneTimeTokenRetriver.Object)
                .BuildServiceProvider();

            var mockHttRequest = new Mock<HttpRequest>();
            mockHttRequest.SetupGet(m => m.Headers).Returns(mockHeader.Object);
            mockHttRequest.SetupGet(m => m.Query).Returns(queryCollection);
            mockHttRequest.SetupGet(m => m.Path).Returns(path);
            var mockHttpContext = new Mock<HttpContext>();
            mockHttRequest.SetupGet(m => m.HttpContext).Returns(mockHttpContext.Object);
            mockHttpContext.SetupGet(m => m.Request).Returns(mockHttRequest.Object);
            mockHttpContext.SetupGet(m => m.RequestServices).Returns(requestServices);
            if (jwtBearerHandler != null)
            {
                await jwtBearerHandler.InitializeAsync(new AuthenticationScheme("Bearer", null, typeof(JwtBearerHandler)), mockHttpContext.Object).ConfigureAwait(false);
                await jwtBearerHandler.AuthenticateAsync().ConfigureAwait(false);
            }            

            var oauthIntrospectionHandler = provider.GetService<OAuth2IntrospectionHandler>();
            Assert.NotNull(oauthIntrospectionHandler);
            if (oauthIntrospectionHandler != null)
            {
                await oauthIntrospectionHandler.InitializeAsync(new AuthenticationScheme("introspection", null, typeof(OAuth2IntrospectionHandler)), mockHttpContext.Object).ConfigureAwait(false);
                await oauthIntrospectionHandler.AuthenticateAsync().ConfigureAwait(false);
            }
        }

    }
}
