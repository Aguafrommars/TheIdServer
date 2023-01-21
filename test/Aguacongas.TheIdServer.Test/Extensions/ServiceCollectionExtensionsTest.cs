// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Xunit;

namespace Aguacongas.TheIdServer.Test.Extensions
{
    public class ServiceCollectionExtensionsTest
    {
        [Fact]
        public void AddOpenTelemetry_should_not_add_opt_when_service_name_is_null()
        {
            var configuration = new ConfigurationBuilder().Build();
            var services = new ServiceCollection().AddOpenTelemetry(configuration);
            Assert.False(services.Any());
        }
    }
}
