namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    public class MetricsOptions
    {
        public OtlpExporterOptions OpenTelemetryProtocol { get; set; }

        public PrometheusOptions Prometheus { get; set; }
        public ConsoleOptions Console { get; set; }

        public HoneycombOptions Honeycomb { get; set; }
    }
}