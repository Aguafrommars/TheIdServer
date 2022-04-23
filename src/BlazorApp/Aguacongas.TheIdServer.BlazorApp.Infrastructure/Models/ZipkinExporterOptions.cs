namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    public class ZipkinExporterOptions
    {
        //
        // Summary:
        //     Gets or sets Zipkin endpoint address. See https://zipkin.io/zipkin-api/#/default/post_spans.
        //     Typically https://zipkin-server-name:9411/api/v2/spans.
        public string Endpoint { get; set; }


        //
        // Summary:
        //     Gets or sets a value indicating whether short trace id should be used.
        public bool UseShortTraceIds { get; set; }

        //
        // Summary:
        //     Gets or sets the maximum payload size in bytes. Default value: 4096.
        public int? MaxPayloadSizeInBytes { get; set; }


        //
        // Summary:
        //     Gets or sets the export processor type to be used with Zipkin Exporter. The default
        //     value is OpenTelemetry.ExportProcessorType.Batch.
        public ExportProcessorType ExportProcessorType { get; set; }


    }
}