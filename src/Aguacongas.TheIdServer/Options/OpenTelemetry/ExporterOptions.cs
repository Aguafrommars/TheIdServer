using Honeycomb.OpenTelemetry;
using OpenTelemetry.Exporter;

namespace Aguacongas.TheIdServer.Options.OpenTelemetry
{
    public class ExporterOptions
    {
        public JaegerExporterOptions Jaeger { get; set; }
        public OtlpExporterOptions OpenTelemetryProtocol { get; set; }
        public ZipkinExporterOptions Zipkin { get; set; }

        public HoneycombOptions Honeycomb { get; set; }
    }
}