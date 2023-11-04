using OpenTelemetry.Instrumentation.StackExchangeRedis;

namespace Aguacongas.TheIdServer.Options.OpenTelemetry
{
    public class RedisOptions : StackExchangeRedisInstrumentationOptions
    {
        public string ConnectionString { get; set; }
    }
}
