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
                    ConsoleEnabled = true,
                    Service = new ServiceOptions
                    {
                        Name = "test"
                    },
                    Exporter = new ExporterOptions
                    {
                        OpenTelemetryProtocol = new OtlpExporterOptions
                        {
                            Endpoint = new Uri("https://exemple.com")
                        }
                    }

                })
                .Build();

            Assert.NotNull(provider);

        }
    }
}
