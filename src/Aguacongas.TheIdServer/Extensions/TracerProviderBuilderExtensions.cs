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
        public static TracerProviderBuilder AddTheIdServerTraces(this TracerProviderBuilder builder, OpenTelemetryOptions options)
        {
            var traceOptions = options.Trace;
            if (traceOptions is null)
            {
                return builder;
            }

            if (traceOptions.ConsoleEnabled)
            {
                builder = builder.AddConsoleExporter();
            }

            if (traceOptions.Sources is not null)
            {
                foreach (var source in traceOptions.Sources)
                {
                    builder = builder.AddSource(source);
                }
            }

            var serviceOptions = traceOptions.Service;
            if (!string.IsNullOrEmpty(serviceOptions?.Name))
            {                
                builder = builder.AddSource(serviceOptions.Name)
                                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceOptions.Name,
                                    serviceOptions.Namespace,
                                    serviceOptions.Version,
                                    serviceOptions.AutoGenerateServiceInstanceId,
                                    serviceOptions.InstanceId));
            }

            return builder.AddExporters(traceOptions)                
                .AddInstrumentation(traceOptions.Instrumentation);
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
                    o.FromOther(options.Honeycomb);
                });
            }

            return builder;
        }
    }
}
