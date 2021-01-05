// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using System;
using System.Collections.Generic;

namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    public class OpenIdConnectOptions : RemoteAuthenticationOptions
    {
        public bool DisableTelemetry { get; set; }

        public bool SkipUnrecognizedRequests { get; set; }

        public bool UseTokenLifetime { get; set; }

        public string SignOutScheme { get; set; }

        public string RemoteSignOutPath { get; set; }

        public ICollection<string> Scope { get; set; }

        public string Prompt { get; set; }

        public string ResponseType { get; set; }

        public string ResponseMode { get; set; }

        public string Resource { get; set; }

        public bool RefreshOnIssuerKeyNotFound { get; set; }

        public string Authority { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public bool RequireHttpsMetadata { get; set; }

        public TimeSpan? MaxAge { get; set; }

        public bool UsePkce { get; set; }
    }
}
