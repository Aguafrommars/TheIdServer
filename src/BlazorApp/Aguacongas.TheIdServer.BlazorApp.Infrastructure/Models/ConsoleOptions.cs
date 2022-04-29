namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    public class ConsoleOptions
    {
        public ConsoleExporterOutputTargets Targets { get; set; }

        //
        // Summary:
        //     Gets or sets the OpenTelemetry.Metrics.MetricReaderTemporalityPreference.
        public MetricReaderTemporalityPreference TemporalityPreference { get; set; }


        //
        // Summary:
        //     Gets or sets the OpenTelemetry.Metrics.MetricReaderOptions.PeriodicExportingMetricReaderOptions.
        public PeriodicExportingMetricReaderOptions PeriodicExportingMetricReaderOptions { get; set; }

    }
}