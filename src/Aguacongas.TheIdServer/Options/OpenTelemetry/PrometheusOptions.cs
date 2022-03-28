using OpenTelemetry.Exporter;

namespace Aguacongas.TheIdServer.Options.OpenTelemetry
{
    public class PrometheusOptions : PrometheusExporterOptions
    {
        public bool Protected { get; set; }
    }
}