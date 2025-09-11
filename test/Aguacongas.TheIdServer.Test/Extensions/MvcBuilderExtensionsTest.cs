// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.TheIdServer.Options.OpenTelemetry;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using System;
using Xunit;

namespace Aguacongas.TheIdServer.Test.Extensions
{
    public class MvcBuilderExtensionsTest
    {
        [Fact]
        public void AddTheIdServerMetrics_should_add_otlp_exporter()
        {
            using var provider = Sdk.CreateMeterProviderBuilder()
                .AddTheIdServerMetrics(new OpenTelemetryOptions
                {
                    Metrics = new MetricsOptions
                    {
                        OpenTelemetryProtocol = new OtlpExporterOptions
                        {
                            Endpoint = new Uri("https://exemple.com")
                        },
                        Prometheus = new PrometheusOptions
                        {
                            HttpListenerPrefixes = new string[] { "http://localhost:9090" }
                        }
                    }
                })
                .Build();

            Assert.NotNull(provider);

        }
    }
}
