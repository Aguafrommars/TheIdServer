using System;

namespace Honeycomb.OpenTelemetry
{
    public static class HoneycombOptionsExtensions
    {
        public static void FromOther(this HoneycombOptions options, HoneycombOptions other)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (other is null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            Copy(options, other);
        }

        private static void Copy(HoneycombOptions options, HoneycombOptions other)
        {
            options.ApiKey = other.ApiKey;
            options.TracesApiKey = other.TracesApiKey;
            options.MetricsApiKey = other.MetricsApiKey;
            options.Dataset = other.Dataset;
            options.TracesDataset = other.TracesDataset;
            options.MetricsDataset = other.MetricsDataset;
            options.Endpoint = other.Endpoint;
            options.TracesEndpoint = other.TracesEndpoint;
            options.MetricsEndpoint = other.MetricsEndpoint;
            options.SampleRate = other.SampleRate;
            options.ServiceName = other.ServiceName;
            options.ServiceVersion = other.ServiceVersion;
            options.InstrumentHttpClient = other.InstrumentHttpClient;
            options.InstrumentSqlClient = other.InstrumentSqlClient;
            options.InstrumentGrpcClient = other.InstrumentGrpcClient;
            options.InstrumentStackExchangeRedisClient = other.InstrumentStackExchangeRedisClient;
            options.MeterNames = other.MeterNames;
        }
    }
}
