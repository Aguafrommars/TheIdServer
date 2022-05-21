namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    public class OpenTelemetryConfiguration
    {
        public TraceOptions Trace { get; set; }

        public MetricsOptions Metrics { get; set; }
    }
}