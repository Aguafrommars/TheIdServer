using Aguacongas.TheIdServer.Options.OpenTelemetry;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.TheIdServer.Test.Extensions
{
    public class MeterProviderBuilderExtensionsTest
    {
        [Fact]
        public void AddTheIdServerTelemetry_should_add_exporters()
        {
            using var provider = Sdk.CreateMeterProviderBuilder()
                .AddTheIdServerTelemetry(new OpenTelemetryOptions
                {
                    Exporter = new ExporterOptions
                    {
                        Telemetry = new TelemetryOptions
                        {
                            Console = new ConsoleOptions(),
                            OpenTelemetryProtocol = new OtlpExporterOptions(),
                            Prometheus = new PrometheusExporterOptions()
                        }
                    }
                }).Build();

            Assert.NotNull(provider);
        }
    }
}
