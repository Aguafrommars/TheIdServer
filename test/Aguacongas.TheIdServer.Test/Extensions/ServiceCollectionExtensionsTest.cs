// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Aguacongas.TheIdServer.Options.OpenTelemetry;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Xunit;
using ZstdSharp.Unsafe;

namespace Aguacongas.TheIdServer.Test.Extensions
{
    public class ServiceCollectionExtensionsTest
    {
        [Fact]
        public void AddOpenTelemetry_should_not_add_opt_when_service_name_is_null()
        {
            var configuration = new ConfigurationBuilder()
                .Build();
            var services = new ServiceCollection().AddOpenTelemetry(configuration);
            Assert.False(services.Any());
        }

        [Fact]
        public void AddOpenTelemetry_should_add_open_telemetry_services() {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?> 
                {                    
                    [$"{nameof(OpenTelemetryOptions.Metrics)}:{nameof(MetricsOptions.Prometheus)}:{nameof(PrometheusOptions.Protected)}"] = "true",
                    [$"{nameof(OpenTelemetryOptions.Metrics)}:{nameof(MetricsOptions.Prometheus)}:{nameof(PrometheusOptions.StartHttpListener)}"] = "true"
                })
                .Build();
            var services = new ServiceCollection().AddOpenTelemetry(configuration);
            Assert.True(services.Any());
            var provider = services.BuildServiceProvider();
            var meterProvider = provider.GetRequiredService<MeterProvider>();
            Assert.NotNull(meterProvider);
        }
    }
}
