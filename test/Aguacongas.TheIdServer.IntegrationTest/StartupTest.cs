// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.KeysRotation;
using Aguacongas.IdentityServer.KeysRotation.RavenDb;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.Models;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using Xunit;

namespace Aguacongas.TheIdServer.IntegrationTest
{
    public class StartupTest
    {
        [Fact]
        public void ConfigureService_should_configure_ravendb_services()
        {
            var wrapper = new RavenDbTestDriverWrapper();
            var sut = TestUtils.CreateTestServer(services =>
            {
                services.AddSingleton(p => wrapper.GetDocumentStore());
            },new Dictionary<string, string>
            {
                ["DbType"] = DbTypes.RavenDb.ToString(),
                ["IdentityServer:Key:StorageKind"] = StorageKind.RavenDb.ToString(),
                ["DataProtectionOptions:StorageKind"] = StorageKind.RavenDb.ToString(),
            });

            var provider = sut.Host.Services;
            Assert.NotNull(provider.GetService<IAdminStore<ApiClaim>>());
            var configureRotationOptions = provider.GetService<IConfigureOptions<KeyRotationOptions>>();
            var rotationOptions = new KeyRotationOptions();
            configureRotationOptions.Configure(rotationOptions);
            Assert.IsType<RavenDbXmlRepository<KeyRotationKey, DocumentSessionWrapper>>(rotationOptions.XmlRepository);
            var configureManagementOptions = provider.GetService<IConfigureOptions<KeyManagementOptions>>();
            var managementOptions = new KeyRotationOptions();
            configureManagementOptions.Configure(managementOptions);
            Assert.IsType<RavenDbXmlRepository<DataProtectionKey, DocumentSessionWrapper>>(managementOptions.XmlRepository);
        }
    }
}
