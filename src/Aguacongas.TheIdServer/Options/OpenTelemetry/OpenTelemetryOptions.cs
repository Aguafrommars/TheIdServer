

namespace Aguacongas.TheIdServer.Options.OpenTelemetry
{
    public class OpenTelemetryOptions
    {
        public ServiceOptions Service { get; set; }
        public bool ConsoleEnabled { get; set; }
       
        

        public InstrumentationOptions Instrumentation { get; set; }

        public ExporterOptions Exporter { get; set; }
    }
}
