// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.TheIdServer.Options.OpenTelemetry;

namespace OpenTelemetry.Metrics
{
    public static class MeterProviderBuilderExtensions
    {
        public static MeterProviderBuilder AddTheIdServerTelemetry(this MeterProviderBuilder builder, OpenTelemetryOptions options)
        {
            builder = builder.AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation();

            var exporterOptions = options?.Exporter?.Telemetry;
            if (exporterOptions is null)
            {
                return builder;
            }

            if (exporterOptions.OpenTelemetryProtocol?.Endpoint is not null)
            {
                builder.AddOtlpExporter(o =>
                {
                    var otlpOptions = exporterOptions.OpenTelemetryProtocol;
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

            if (exporterOptions.Prometheus?.HttpListenerPrefixes is not null)
            {
                builder.AddPrometheusExporter(o =>
                {
                    var prometeuspOptions = exporterOptions.Prometheus;
                    o.StartHttpListener = prometeuspOptions.StartHttpListener;
                    o.HttpListenerPrefixes = prometeuspOptions.HttpListenerPrefixes;
                    o.ScrapeEndpointPath = prometeuspOptions.ScrapeEndpointPath;
                    o.ScrapeResponseCacheDurationMilliseconds = prometeuspOptions.ScrapeResponseCacheDurationMilliseconds;
                });
            }

            return builder;
        }
    }
}
