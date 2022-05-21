namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    public class OtlpExporterOptions
    {
        //
        // Summary:
        //     Gets or sets the target to which the exporter is going to send telemetry. Must
        //     be a valid Uri with scheme (http or https) and host, and may contain a port and
        //     path. The default value is * http://localhost:4317 for OpenTelemetry.Exporter.OtlpExportProtocol.Grpc
        //     * http://localhost:4318 for OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf.
        public string Endpoint { get; set; }

        //
        // Summary:
        //     Gets or sets optional headers for the connection. Refer to the specification
        //     for information on the expected format for Headers.
        public string Headers { get; set; }

        //
        // Summary:
        //     Gets or sets the max waiting time (in milliseconds) for the backend to process
        //     each batch. The default value is 10000.
        public int TimeoutMilliseconds { get; set; }


        //
        // Summary:
        //     Gets or sets the the OTLP transport protocol. Supported values: Grpc and HttpProtobuf.
        public OtlpExportProtocol Protocol { get; set; }

        //
        // Summary:
        //     Gets or sets the export processor type to be used with the OpenTelemetry Protocol
        //     Exporter. The default value is OpenTelemetry.ExportProcessorType.Batch.
        public ExportProcessorType ExportProcessorType { get; set; }


    }
}