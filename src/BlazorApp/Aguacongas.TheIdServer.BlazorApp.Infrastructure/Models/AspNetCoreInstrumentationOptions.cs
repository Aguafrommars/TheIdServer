namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    public class AspNetCoreInstrumentationOptions
    {
        //
        // Summary:
        //     Gets or sets a value indicating whether the exception will be recorded as ActivityEvent
        //     or not.
        //
        // Remarks:
        //     https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/trace/semantic_conventions/exceptions.md.
        public bool RecordException { get; set; }

        //
        // Summary:
        //     Gets or sets a value indicating whether RPC attributes are added to an Activity
        //     when using Grpc.AspNetCore. Default is true.
        //
        // Remarks:
        //     https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/trace/semantic_conventions/rpc.md.
        public bool EnableGrpcAspNetCoreSupport { get; set; }
    }
}