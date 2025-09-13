// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.TheIdServer.Options.OpenTelemetry;
using Honeycomb.OpenTelemetry;
using System.Linq;

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

            var prometeuspOptions = metricsOptions.Prometheus;
            if (prometeuspOptions is not null)
            {
                builder.AddPrometheusExporter(o =>
                {
                    o.ScrapeEndpointPath = prometeuspOptions.ScrapeEndpointPath;
                    o.ScrapeResponseCacheDurationMilliseconds = prometeuspOptions.ScrapeResponseCacheDurationMilliseconds;
                });

                if (prometeuspOptions.StartHttpListener) 
                {
                    builder.AddPrometheusHttpListener(o =>
                    {
                        o.UriPrefixes = prometeuspOptions.HttpListenerPrefixes?.ToArray() ?? o.UriPrefixes;
                        o.ScrapeEndpointPath = prometeuspOptions.ScrapeEndpointPath;
                    });
                }
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
