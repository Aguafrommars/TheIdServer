// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.TheIdServer.Options.OpenTelemetry;
using Honeycomb.OpenTelemetry;

namespace OpenTelemetry.Metrics
{
    public static class MeterProviderBuilderExtensions
    {
        public static MeterProviderBuilder AddTheIdServerMetrics(this MeterProviderBuilder builder, OpenTelemetryOptions options)
        {
            builder = builder.AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation();

            var metricsOptions = options.Metrics;
            if (metricsOptions is null)
            {
                return builder;
            }

            if (metricsOptions.Console is not null)
            {
                builder = builder.AddConsoleExporter((c, r) =>
                {
                    var consoleOptions = metricsOptions.Console;
                    c.Targets = consoleOptions.Targets;
                    r.PeriodicExportingMetricReaderOptions.FromOther(consoleOptions.PeriodicExportingMetricReaderOptions);
                    r.TemporalityPreference = consoleOptions.TemporalityPreference;
                });
            }

            if (metricsOptions.OpenTelemetryProtocol?.Endpoint is not null)
            {
                builder.AddOtlpExporter(o =>
                {
                    var otlpOptions = metricsOptions.OpenTelemetryProtocol;
                    o.BatchExportProcessorOptions = otlpOptions.BatchExportProcessorOptions;
                    o.Protocol = otlpOptions.Protocol;
                    o.TimeoutMilliseconds = otlpOptions.TimeoutMilliseconds;
                    o.ExportProcessorType = otlpOptions.ExportProcessorType;
                    o.Endpoint = otlpOptions.Endpoint;
                    o.Headers = otlpOptions.Headers;                    
                });
            }

            if (metricsOptions.Prometheus is not null)
            {
                builder.AddPrometheusExporter(o =>
                {
                    var prometeuspOptions = metricsOptions.Prometheus;
                    o.StartHttpListener = prometeuspOptions.StartHttpListener;
                    o.HttpListenerPrefixes = prometeuspOptions.HttpListenerPrefixes;
                    o.ScrapeEndpointPath = prometeuspOptions.ScrapeEndpointPath;
                    o.ScrapeResponseCacheDurationMilliseconds = prometeuspOptions.ScrapeResponseCacheDurationMilliseconds;
                });
            }

            if (metricsOptions.Honeycomb?.ApiKey is not null)
            {
                builder = builder.AddHoneycomb(o =>
                {
                    o.FromOther(metricsOptions.Honeycomb);

                });
            }

            return builder;
        }
    }
}
