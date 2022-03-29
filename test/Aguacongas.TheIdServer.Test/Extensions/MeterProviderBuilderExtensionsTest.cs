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
        public void AddTheIdServerMetrics_should_add_exporters()
        {
            using var provider = Sdk.CreateMeterProviderBuilder()
                .AddTheIdServerMetrics(new OpenTelemetryOptions
                {
                    Metrics = new MetricsOptions
                    {
                        Console = new ConsoleOptions(),
                        OpenTelemetryProtocol = new OtlpExporterOptions(),
                        Prometheus = new PrometheusOptions()
                    }
                }).Build();

            Assert.NotNull(provider);
        }
    }
}
