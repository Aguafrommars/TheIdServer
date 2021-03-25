// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.RavenDb.Store.Api;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.IdentityServer.RavenDb.Store.Test.Extensions
{
    public class ServiceCollectionExtensionsTest
    {
        [Fact]
        public void AddIdentityServer4AdminRavenDbkStores_should_add_ravendb_stores()
        {
            var services = new ServiceCollection();

            services
                .AddSingleton(p => new RavenDbTestDriverWrapper().GetDocumentStore())
                .AddIdentityServer4AdminRavenDbkStores();

            Assert.Contains(services, s => s.ImplementationType == typeof(ApiClaimStore));
        }
    }
}
