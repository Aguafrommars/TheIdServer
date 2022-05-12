namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    //
    // Summary:
    //     Supported by OTLP exporter protocol types according to the specification https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/protocol/exporter.md.
    public enum OtlpExportProtocol : byte
    {
        //
        // Summary:
        //     OTLP over gRPC (corresponds to 'grpc' Protocol configuration option). Used as
        //     default.
        Grpc,
        //
        // Summary:
        //     OTLP over HTTP with protobuf payloads (corresponds to 'http/protobuf' Protocol
        //     configuration option).
        HttpProtobuf
    }
}