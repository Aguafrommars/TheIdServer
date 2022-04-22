namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    public class InstrumentationOptions
    {
        public AspNetCoreInstrumentationOptions AspNetCore { get; set; }
        public HttpClientInstrumentationOptions HttpClient { get; set; }
        public SqlClientInstrumentationOptions SqlClient { get; set; }
        public RedisOptions Redis { get; set; }
    }
}