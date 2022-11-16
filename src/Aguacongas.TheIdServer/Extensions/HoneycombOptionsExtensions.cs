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
            options.AddBaggageSpanProcessor = other.AddBaggageSpanProcessor;
            options.AddDeterministicSampler = other.AddDeterministicSampler;
            options.ApiKey = other.ApiKey;
            options.Dataset = other.Dataset;
            options.Debug = other.Debug;
            options.EnableLocalVisualizations = other.EnableLocalVisualizations; 
            options.Endpoint = other.Endpoint;
            options.MeterNames = other.MeterNames;
            options.MetricsApiKey = other.MetricsApiKey;
            options.MetricsDataset = other.MetricsDataset;
            options.MetricsEndpoint = other.MetricsEndpoint;
            options.ResourceBuilder = other.ResourceBuilder;
            options.SampleRate = other.SampleRate;
            options.ServiceName = other.ServiceName;
            options.ServiceVersion = other.ServiceVersion;
            options.TracesApiKey = other.TracesApiKey;
            options.TracesDataset = other.TracesDataset;
            options.TracesEndpoint = other.TracesEndpoint;
        }
    }
}
