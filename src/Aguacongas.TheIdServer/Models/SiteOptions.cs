// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre

namespace Aguacongas.TheIdServer.Models
{
    public class SiteOptions
    {
        public static readonly string BOOTSTRAPCSSURL = "https://cdn.jsdelivr.net/npm/bootstrap@5.3.0-alpha1/dist/css/bootstrap.min.css";
        public static readonly string BOOTSTRAPJSURL = "https://cdn.jsdelivr.net/npm/bootstrap@5.3.0-alpha1/dist/js/bootstrap.bundle.min.js";
        public static readonly string JQUERYURL = "https://cdnjs.cloudflare.com/ajax/libs/jquery/3.6.0/jquery.slim.min.js";

        public string Name { get; set; }
    }
}
