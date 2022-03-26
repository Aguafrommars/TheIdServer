// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
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
        public void AddTheIdServerTelemetry_should_add_otlp_exporter()
        {
            using var provider = Sdk.CreateMeterProviderBuilder()
                .AddTheIdServerTelemetry(new OpenTelemetryOptions
                {
                    Service = new ServiceOptions
                    {
                        Name = "test"
                    },
                    Exporter = new ExporterOptions
                    {
                        Telemetry =  new TelemetryOptions
                        {
                            OpenTelemetryProtocol = new OtlpExporterOptions
                            {
                                Endpoint = new Uri("https://exemple.com")
                            },
                            Prometheus = new PrometheusExporterOptions
                            {
                                HttpListenerPrefixes = new string[] { "http://localhost:9090" }
                            }
                        }
                    }

                })
                .Build();

            Assert.NotNull(provider);

        }
    }
}
