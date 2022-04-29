namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    public class HttpClientInstrumentationOptions
    {
        //
        // Summary:
        //     Gets or sets a value indicating whether or not the HTTP version should be added
        //     as the OpenTelemetry.Trace.SemanticConventions.AttributeHttpFlavor tag. Default
        //     value: False.
        public bool SetHttpFlavor { get; set; }

        //
        // Summary:
        //     Gets or sets a value indicating whether exception will be recorded as ActivityEvent
        //     or not.
        //
        // Remarks:
        //     https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/trace/semantic_conventions/exceptions.md.
        public bool RecordException { get; set; }
    }
}