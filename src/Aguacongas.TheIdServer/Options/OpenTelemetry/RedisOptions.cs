using OpenTelemetry.Instrumentation.StackExchangeRedis;

namespace Aguacongas.TheIdServer.Options.OpenTelemetry
{
    public class RedisOptions : StackExchangeRedisCallsInstrumentationOptions
    {
        public string ConnectionString { get; set; }
    }
}
