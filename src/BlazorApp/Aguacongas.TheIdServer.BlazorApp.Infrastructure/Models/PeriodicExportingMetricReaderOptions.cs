namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    public class PeriodicExportingMetricReaderOptions
    {
        //
        // Summary:
        //     Gets or sets the metric export interval in milliseconds. If not set, the default
        //     value depends on the type of metric exporter associated with the metric reader.
        public int? ExportIntervalMilliseconds { get; set; }

        //
        // Summary:
        //     Gets or sets the metric export timeout in milliseconds. If not set, the default
        //     value depends on the type of metric exporter associated with the metric reader.
        public int? ExportTimeoutMilliseconds { get; set; }
    }
}