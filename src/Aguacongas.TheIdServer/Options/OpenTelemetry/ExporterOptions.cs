
namespace Aguacongas.TheIdServer.Options.OpenTelemetry
{
    public class ExporterOptions
    {
        public TraceOptions Trace { get; set; }

        public MetricsOptions Telemetry { get; set; }

        
    }
}