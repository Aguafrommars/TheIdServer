// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    public class Settings
    {
        public LoggingOptions LoggingOptions { get; set; }
        public string ApiBaseUrl { get; set; }

        public string AdministratorEmail { get; set; }
        public string WelcomeContenUrl { get;  set; }
        public bool Prerendered { get; set; }
    }

    public class LoggingOptions
    {
        public LogLevel Minimum { get; set; }

        public IEnumerable<LoggingFilter> Filters { get; set; }
    }

    public class LoggingFilter
    {
        public string Category { get; set; }

        public LogLevel Level { get; set; }
    }
}
