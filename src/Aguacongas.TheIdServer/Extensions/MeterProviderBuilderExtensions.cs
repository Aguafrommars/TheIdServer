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
                    r.PeriodicExportingMetricReaderOptions = consoleOptions.PeriodicExportingMetricReaderOptions;
                    r.MetricReaderType = consoleOptions.MetricReaderType;
                    r.Temporality = consoleOptions.Temporality;
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
                    o.PeriodicExportingMetricReaderOptions = otlpOptions.PeriodicExportingMetricReaderOptions;
                    o.MetricReaderType = otlpOptions.MetricReaderType;
                    o.AggregationTemporality = otlpOptions.AggregationTemporality;
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
                    var honeycombOptions = metricsOptions.Honeycomb;
                    o.ApiKey = honeycombOptions.ApiKey;
                    o.TracesApiKey = honeycombOptions.TracesApiKey;
                    o.MetricsApiKey = honeycombOptions.MetricsApiKey;
                    o.Dataset = honeycombOptions.Dataset;
                    o.TracesDataset = honeycombOptions.TracesDataset;
                    o.MetricsDataset = honeycombOptions.MetricsDataset;
                    o.Endpoint = honeycombOptions.Endpoint;
                    o.TracesEndpoint = honeycombOptions.TracesEndpoint;
                    o.MetricsEndpoint = honeycombOptions.MetricsEndpoint;
                    o.SampleRate = honeycombOptions.SampleRate;
                    o.ServiceName = honeycombOptions.ServiceName;
                    o.ServiceVersion = honeycombOptions.ServiceVersion;
                    o.InstrumentHttpClient = honeycombOptions.InstrumentHttpClient;
                    o.InstrumentSqlClient = honeycombOptions.InstrumentSqlClient;
                    o.InstrumentGrpcClient = honeycombOptions.InstrumentGrpcClient;
                    o.InstrumentStackExchangeRedisClient = honeycombOptions.InstrumentStackExchangeRedisClient;
                    o.MeterNames = honeycombOptions.MeterNames;
                });
            }

            return builder;
        }
    }
}
