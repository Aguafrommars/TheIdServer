namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    public class JaegerExporterOptions
    {
        public JaegerExportProtocol Protocol { get; set; }

        //
        // Summary:
        //     Gets or sets the Jaeger agent host. Default value: localhost.
        public string AgentHost { get; set; }


        //
        // Summary:
        //     Gets or sets the Jaeger agent port. Default value: 6831.
        public int AgentPort { get; set; }


        //
        // Summary:
        //     Gets or sets the Jaeger HTTP endpoint. Default value: "http://localhost:14268/api/traces".
        //     Typically https://jaeger-server-name:14268/api/traces.
        public string Endpoint { get; set; }


        //
        // Summary:
        //     Gets or sets the maximum payload size in bytes. Default value: 4096.
        public int? MaxPayloadSizeInBytes { get; set; }


        //
        // Summary:
        //     Gets or sets the export processor type to be used with Jaeger Exporter. The default
        //     value is OpenTelemetry.ExportProcessorType.Batch.
        public ExportProcessorType ExportProcessorType { get; set; }


    }
}