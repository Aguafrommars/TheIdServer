namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    public class HoneycombOptions
    {
        //
        // Summary:
        //     API key used to send telemetry data to Honeycomb.
        public string ApiKey { get; set; }

        //
        // Summary:
        //     API key used to send trace telemetry data to Honeycomb. Defaults to Honeycomb.OpenTelemetry.HoneycombOptions.ApiKey.
        public string TracesApiKey { get; set; }

        //
        // Summary:
        //     API key used to send metrics telemetry data to Honeycomb. Defaults to Honeycomb.OpenTelemetry.HoneycombOptions.ApiKey.
        public string MetricsApiKey { get; set; }

        //
        // Summary:
        //     Honeycomb dataset to store telemetry data.
        public string Dataset { get; set; }

        //
        // Summary:
        //     Honeycomb dataset to store trace telemetry data. Defaults to Honeycomb.OpenTelemetry.HoneycombOptions.Dataset.
        public string TracesDataset { get; set; }

        //
        // Summary:
        //     Honeycomb dataset to store metrics telemetry data. Defaults to "null".
        //     Required to enable metrics.
        public string MetricsDataset { get; set; }

        //
        // Summary:
        //     API endpoint to send telemetry data. Defaults to Honeycomb.OpenTelemetry.HoneycombOptions.DefaultEndpoint.
        public string Endpoint { get; set; }


        //
        // Summary:
        //     API endpoint to send telemetry data. Defaults to Honeycomb.OpenTelemetry.HoneycombOptions.Endpoint.
        public string TracesEndpoint { get; set; }

        //
        // Summary:
        //     API endpoint to send telemetry data. Defaults to Honeycomb.OpenTelemetry.HoneycombOptions.Endpoint.
        public string MetricsEndpoint { get; set; }

        //
        // Summary:
        //     Sample rate for sending telemetry data. Defaults to Honeycomb.OpenTelemetry.HoneycombOptions.DefaultSampleRate.
        //     See Honeycomb.OpenTelemetry.DeterministicSampler for more details on how sampling
        //     is applied.
        public uint SampleRate { get; set; }


        //
        // Summary:
        //     Service name used to identify application. Defaults to unknown_process:processname.
        public string ServiceName { get; set; }

        //
        // Summary:
        //     Service version. Defaults to application assembly information version.
        public string ServiceVersion { get; set; }

        //
        // Summary:
        //     Controls whether to instrument HttpClient calls.
        public bool InstrumentHttpClient { get; set; }


        //
        // Summary:
        //     Controls whether to instrument SqlClient calls.
        public bool InstrumentSqlClient { get; set; }


        //
        // Summary:
        //     Controls whether to instrument GrpcClient calls when running on .NET Standard
        //     2.1 or greater. Requires Honeycomb.OpenTelemetry.HoneycombOptions.InstrumentHttpClient
        //     to be true due to the underlying implementation.
        public bool InstrumentGrpcClient { get; set; }


        //
        // Summary:
        //     Controls whether the Stack Exchange Redis Client is instrumented. Requires that
        //     either Honeycomb.OpenTelemetry.HoneycombOptions.RedisConnection is set, if you're
        //     not using a DI Container, or if you are using a DI Container, then it requires
        //     that an StackExchange.Redis.IConnectionMultiplexer has been registered with the
        //     System.IServiceProvider.
        public bool InstrumentStackExchangeRedisClient { get; set; }


    }
}