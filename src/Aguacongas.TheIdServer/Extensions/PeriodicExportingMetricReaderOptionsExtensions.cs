namespace OpenTelemetry.Metrics
{
    public static class PeriodicExportingMetricReaderOptionsExtensions
    {
        public static void FromOther(this PeriodicExportingMetricReaderOptions options, PeriodicExportingMetricReaderOptions other)
        {
            options.ExportIntervalMilliseconds = other.ExportIntervalMilliseconds;
            options.ExportTimeoutMilliseconds = other.ExportTimeoutMilliseconds;
        }
    }
}
