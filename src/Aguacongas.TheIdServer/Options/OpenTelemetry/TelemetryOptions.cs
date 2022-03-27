using OpenTelemetry.Exporter;

namespace Aguacongas.TheIdServer.Options.OpenTelemetry
{
    public class TelemetryOptions
    {
        public OtlpExporterOptions OpenTelemetryProtocol { get; set; }

        public PrometheusExporterOptions Prometheus { get; set; }
        public ConsoleOptions Console { get; set; }
    }
}
