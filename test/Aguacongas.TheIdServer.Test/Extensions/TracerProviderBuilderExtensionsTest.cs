// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.TheIdServer.Options.OpenTelemetry;
using Honeycomb.OpenTelemetry;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Instrumentation.Http;
using OpenTelemetry.Instrumentation.SqlClient;
using OpenTelemetry.Trace;
using System;
using Xunit;

namespace Aguacongas.TheIdServer.Test.Extensions
{
    public class TracerProviderBuilderExtensionsTest
    {
        [Fact]
        public void AddTheIdServerTelemetry_should_add_console_exporter()
        {
            using var provider = Sdk.CreateTracerProviderBuilder()
                .AddTheIdServerTraces(new OpenTelemetryOptions
                {
                    Trace = new TraceOptions
                    {
                        Service = new ServiceOptions
                        {
                            Name = "test"
                        },
                        ConsoleEnabled = true
                    }
                })
                .Build();
            
            Assert.NotNull(provider);
        }

        [Fact]
        public void AddTheIdServerTelemetry_should_add_exporters()
        {
            using var provider = Sdk.CreateTracerProviderBuilder()
                .AddTheIdServerTraces(new OpenTelemetryOptions
                {
                    Trace = new TraceOptions
                    {
                        OpenTelemetryProtocol = new OtlpExporterOptions
                        {
                            Endpoint = new Uri("https://exemple.com")
                        },
                        Honeycomb = new HoneycombOptions
                        {
                            ApiKey = "test"
                        },
                        Jaeger = new JaegerExporterOptions
                        {
                            AgentHost = "exemple.com",
                            AgentPort = 443
                        },
                        Zipkin = new ZipkinExporterOptions
                        {
                            Endpoint = new Uri("https://exemple.com")
                        }
                    }
                })
                .Build();

            Assert.NotNull(provider);
        }

        [Fact]
        public void AddTheIdServerTelemetry_should_set_up_instrumentation()
        {
            using var provider = Sdk.CreateTracerProviderBuilder()
                .AddTheIdServerTraces(new OpenTelemetryOptions
                {
                    Trace = new TraceOptions
                    {
                        Service = new ServiceOptions
                        {
                            Name = "test"
                        },
                        Instrumentation = new InstrumentationOptions
                        {
                            AspNetCore = new AspNetCoreInstrumentationOptions
                            {
                                EnableGrpcAspNetCoreSupport = true,
                                RecordException = true
                            },
                            HttpClient = new HttpClientInstrumentationOptions
                            {
                                RecordException = true,
                                SetHttpFlavor = true
                            },
                            Redis = new RedisOptions
                            {
                                ConnectionString = "localhost",
                                FlushInterval = TimeSpan.FromSeconds(1),
                                SetVerboseDatabaseStatements = true
                            },
                            SqlClient = new SqlClientInstrumentationOptions
                            {
                                EnableConnectionLevelAttributes = true,
                                RecordException = true,
                                SetDbStatementForStoredProcedure = true,
                                SetDbStatementForText = true
                            }
                        }
                    }
                })
                .Build();

            Assert.NotNull(provider);
        }
    }
}
