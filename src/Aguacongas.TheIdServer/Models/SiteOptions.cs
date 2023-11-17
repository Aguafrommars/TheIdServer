// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre

namespace Aguacongas.TheIdServer.Models
{
    public class SiteOptions
    {
        public static readonly string BOOTSTRAPCSSURL = "https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css";
        public static readonly string BOOTSTRAPJSURL = "https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js";
        public static readonly string JQUERYURL = "https://cdnjs.cloudflare.com/ajax/libs/jquery/3.6.0/jquery.slim.min.js";

        public static readonly string BOOTSTRAPCSSURLINTEGRITY = "sha384-T3c6CoIi6uLrA9TneNEoa7RxnatzjcDSCmG1MXxSR1GAsXEV/Dwwykc2MPK8M2HN";
        public static readonly string BOOTSTRAPJSURLINTEGRITY = "sha384-C6RzsynM9kWDrMNeT87bh95OGNyZPhcTNXj1NW7RuBCsyN/o0jlpcV8Qyq46cDfL";
        public static readonly string JQUERYURLINTEGRITY = "sha512-6ORWJX/LrnSjBzwefdNUyLCMTIsGoNP6NftMy2UAm1JBm6PRZCO1d7OHBStWpVFZLO+RerTvqX/Z9mBFfCJZ4A==";
        public string Name { get; set; }
    }
}
