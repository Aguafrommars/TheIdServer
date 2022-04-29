namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    //
    // Summary:
    //     Defines the behavior of a OpenTelemetry.Metrics.MetricReader with respect to
    //     OpenTelemetry.Metrics.AggregationTemporality.
    public enum MetricReaderTemporalityPreference
    {
        //
        // Summary:
        //     All aggregations are performed using cumulative temporatlity.
        Cumulative = 1,
        //
        // Summary:
        //     All measurements that are monotonic in nature are aggregated using delta temporality.
        //     Aggregations of non-monotonic measurements use cumulative temporality.
        Delta
    }
}