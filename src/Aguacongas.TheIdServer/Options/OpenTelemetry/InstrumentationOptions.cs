using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Instrumentation.Http;
using OpenTelemetry.Instrumentation.SqlClient;
using OpenTelemetry.Instrumentation.StackExchangeRedis;

namespace Aguacongas.TheIdServer.Options.OpenTelemetry
{
    public class InstrumentationOptions
    {
        public AspNetCoreInstrumentationOptions AspNetCore{ get; set; }
        public HttpClientInstrumentationOptions HttpClient { get; set; }
        public SqlClientInstrumentationOptions SqlClient { get; set; }
        public RedisOptions Redis { get; set; }
    }
}