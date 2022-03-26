using Honeycomb.OpenTelemetry;
using OpenTelemetry.Exporter;

namespace Aguacongas.TheIdServer.Options.OpenTelemetry
{
    public class TraceOptions
    {
        public bool ConsoleEnabled { get; set; }

        public OtlpExporterOptions OpenTelemetryProtocol { get; set; }
        public JaegerExporterOptions Jaeger { get; set; }
        public ZipkinExporterOptions Zipkin { get; set; }
        public HoneycombOptions Honeycomb { get; set; }

    }
}
