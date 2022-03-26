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

            if (options?.Exporter?.OpenTelemetryProtocol?.Endpoint is not null)
            {
                builder.AddOtlpExporter(o =>
                {
                    var otlpOptions = options.Exporter.OpenTelemetryProtocol;
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
            return builder;
        }
    }
}
