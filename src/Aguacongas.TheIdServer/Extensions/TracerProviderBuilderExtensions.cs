// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.TheIdServer.Options.OpenTelemetry;
using Honeycomb.OpenTelemetry;
using OpenTelemetry.Resources;
using StackExchange.Redis;

namespace OpenTelemetry.Trace
{
    public static class TracerProviderBuilderExtensions
    {
        public static TracerProviderBuilder AddTheIdServerTelemetry(this TracerProviderBuilder builder, OpenTelemetryOptions options)
        {
            if (options.Exporter?.Trace?.ConsoleEnabled == true)
            {
                builder = builder.AddConsoleExporter();
            }

            if (!string.IsNullOrEmpty(options.Service?.Name))
            {
                builder = builder.AddSource(options.Service.Name)
                                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(options.Service.Name,
                                    options.Service.Namespace,
                                    options.Service.Version,
                                    options.Service.AutoGenerateServiceInstanceId,
                                    options.Service.InstanceId));
            }

            return builder.AddExporters(options.Exporter?.Trace)                
                .AddInstrumentation(options.Instrumentation);
        }

        public static TracerProviderBuilder AddInstrumentation(this TracerProviderBuilder builder, InstrumentationOptions options)
        {
            builder = builder.AddHttpClientInstrumentation(o =>
                 {
                     var httpClientOptions = options?.HttpClient;
                     if (httpClientOptions is null)
                     {
                         return;
                     }

                     o.SetHttpFlavor = httpClientOptions.SetHttpFlavor;
                     o.RecordException = httpClientOptions.RecordException;
                 })
                .AddAspNetCoreInstrumentation(o =>
                {
                    var aspOptions = options?.AspNetCore;
                    if (aspOptions is null)
                    {
                        return;
                    }

                    o.RecordException = aspOptions.RecordException;
                    o.EnableGrpcAspNetCoreSupport = aspOptions.EnableGrpcAspNetCoreSupport;
                })
                .AddSqlClientInstrumentation(o =>
                {
                    var sqlClientOptions = options?.SqlClient;
                    if (sqlClientOptions is null)
                    {
                        return;
                    }

                    o.RecordException = sqlClientOptions.RecordException;
                    o.EnableConnectionLevelAttributes = sqlClientOptions.EnableConnectionLevelAttributes;
                    o.SetDbStatementForText = sqlClientOptions.SetDbStatementForText;
                    o.SetDbStatementForStoredProcedure = sqlClientOptions.SetDbStatementForStoredProcedure;
                });

            if (!string.IsNullOrEmpty(options?.Redis?.ConnectionString))
            {
                var connection = ConnectionMultiplexer.Connect(options.Redis.ConnectionString);
                builder = builder.AddRedisInstrumentation(connection, o =>
                {
                    var redisOptions = options?.Redis;
                    if (redisOptions is null)
                    {
                        return;
                    }

                    o.FlushInterval = redisOptions.FlushInterval;
                    o.SetVerboseDatabaseStatements = redisOptions.SetVerboseDatabaseStatements;
                });
            }

            return builder;
        }
        private static TracerProviderBuilder AddExporters(this TracerProviderBuilder builder, TraceOptions options)
        {
            if (options is null)
            {
                return builder;
            }

            if (!string.IsNullOrEmpty(options.Jaeger?.AgentHost))
            {
                builder = builder.AddJaegerExporter(o =>
                {
                    var jaegerOptions = options.Jaeger;
                    o.AgentPort = jaegerOptions.AgentPort;
                    o.AgentHost = jaegerOptions.AgentHost;
                    o.BatchExportProcessorOptions = jaegerOptions.BatchExportProcessorOptions;
                    o.ExportProcessorType = jaegerOptions.ExportProcessorType;
                });
            }

            if (options.OpenTelemetryProtocol?.Endpoint is not null)
            {
                builder = builder.AddOtlpExporter(o =>
                {
                    var otlpOptions = options.OpenTelemetryProtocol;
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

            if (options.Zipkin?.Endpoint is not null)
            {
                builder = builder.AddZipkinExporter(o =>
                {
                    var zipkinOptions = options.Zipkin;
                    o.ExportProcessorType = zipkinOptions.ExportProcessorType;
                    o.BatchExportProcessorOptions = zipkinOptions.BatchExportProcessorOptions;
                    o.Endpoint = zipkinOptions.Endpoint;
                    o.MaxPayloadSizeInBytes = zipkinOptions.MaxPayloadSizeInBytes;
                    o.UseShortTraceIds = zipkinOptions.UseShortTraceIds;
                });
            }

            if (options.Honeycomb?.ApiKey is not null)
            {
                builder = builder.AddHoneycomb(o =>
                {
                    var honeycombOptions = options.Honeycomb;
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
