using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Instrumentation.Http;
using OpenTelemetry.Instrumentation.SqlClient;

namespace Aguacongas.TheIdServer.Options.OpenTelemetry
{
    public class InstrumentationOptions
    {
        public AspNetCoreTraceInstrumentationOptions AspNetCore { get; set; }
        public HttpClientTraceInstrumentationOptions HttpClient { get; set; }
        public SqlClientInstrumentationOptions SqlClient { get; set; }
        public RedisOptions Redis { get; set; }
    }
}