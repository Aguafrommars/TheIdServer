using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    public class ServerConfig
    {
        public SiteConfiguration SiteOptions { get; set; }

        public IdentityOptions IdentityOptions { get; set; }

        public IdentityServerConfiguration IdentityServerOptions { get; set; }

        public AccountConfiguration AccountOptions { get; set; }

        public IdentityServerDataProtectionConfiguration IdentityServer { get; set; }

        public DataProtectionConfiguration DataProtectionOptions { get; set; }

        public bool DisableTokenCleanup { get; set; }

        public TimeSpan? TokenCleanupInterval { get; set; }

        public string AuthenticatorIssuer { get; set; }

        public bool EnableOpenApiDoc { get; set; }

        public IEnumerable<string> CorsAllowedOrigin { get; set; }

        public SwaggerUiSettings SwaggerUiSettings { get; set; }

        public ApiAuthentication ApiAuthentication { get; set; }

        public PrivateServerAuthentication PrivateServerAuthentication { get; set; }

        public PrivateServerAuthentication EmailApiAuthentication { get; set; }
    }
}
