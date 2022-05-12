using System;

namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    public class RedisOptions
    {
        public string ConnectionString { get; set; }

        //
        // Summary:
        //     Gets or sets the maximum time that should elapse between flushing the internal
        //     buffer of Redis profiling sessions and creating System.Diagnostics.Activity objects.
        //     Default value: 00:00:10.
        public TimeSpan FlushInterval { get; set; }


        //
        // Summary:
        //     Gets or sets a value indicating whether or not the OpenTelemetry.Instrumentation.StackExchangeRedis.StackExchangeRedisCallsInstrumentation
        //     should use reflection to get more detailed OpenTelemetry.Trace.SemanticConventions.AttributeDbStatement
        //     tag values. Default value: False.
        public bool SetVerboseDatabaseStatements { get; set; }

    }
}