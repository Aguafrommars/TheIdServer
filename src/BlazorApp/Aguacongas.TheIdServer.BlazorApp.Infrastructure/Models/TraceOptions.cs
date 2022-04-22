using System.Collections.Generic;

namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    public class TraceOptions
    {
        public ServiceOptions Service { get; set; }

        public InstrumentationOptions Instrumentation { get; set; }

        public bool ConsoleEnabled { get; set; }

        public OtlpExporterOptions OpenTelemetryProtocol { get; set; }
        public JaegerExporterOptions Jaeger { get; set; }
        public ZipkinExporterOptions Zipkin { get; set; }
        public HoneycombOptions Honeycomb { get; set; }

        public IEnumerable<string> Sources { get; set; }

    }
}