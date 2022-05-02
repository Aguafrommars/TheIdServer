using System.Collections.Generic;

namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    public class PrometheusOptions
    {
        public bool Protected { get; set; }

        //
        // Summary:
        //     Gets or sets a value indicating whether or not an http listener should be started.
        //     Default value: False.
        public bool StartHttpListener { get; set; }

        //
        // Summary:
        //     Gets or sets the prefixes to use for the http listener. Default value: http://localhost:9464/.
        public IReadOnlyCollection<string> HttpListenerPrefixes { get; set; }

        //
        // Summary:
        //     Gets or sets the path to use for the scraping endpoint. Default value: /metrics.
        public string ScrapeEndpointPath { get; set; }


        //
        // Summary:
        //     Gets or sets the cache duration in milliseconds for scrape responses. Default
        //     value: 10,000 (10 seconds).
        //
        // Remarks:
        //     Note: Specify 0 to disable response caching.
        public int ScrapeResponseCacheDurationMilliseconds { get; set; }
    }
}