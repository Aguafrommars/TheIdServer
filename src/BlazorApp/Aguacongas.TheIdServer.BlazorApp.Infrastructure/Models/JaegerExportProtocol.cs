namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    //
    // Summary:
    //     Defines the exporter protocols supported by the OpenTelemetry.Exporter.JaegerExporter.
    public enum JaegerExportProtocol : byte
    {
        //
        // Summary:
        //     Compact thrift protocol over UDP.
        //
        // Remarks:
        //     Note: Supported by Jaeger Agents only.
        UdpCompactThrift,
        //
        // Summary:
        //     Binary thrift protocol over HTTP.
        //
        // Remarks:
        //     Note: Supported by Jaeger Collectors only.
        HttpBinaryThrift
    }
}