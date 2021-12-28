using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    public class ServerConfig
    {
        public SiteOptions SiteOptions { get; set; }

        public IdentityOptions IdentityOptions { get; set; }

        public IdentityServerOptions IdentityServerOptions { get; set; }

        public AccountOptions AccountOptions { get; set; }

        public IdentityServerDataProtectionOptions IdentityServer { get; set; }

        public DataProtectionOptions DataProtectionOptions { get; set; }

        public bool DisableTokenCleanup { get; set; }

        public bool TokenCleanupInterval { get; set; }

        public string AuthenticatorIssuer { get; set; }

        public bool EnableOpenApiDoc { get; set; }

        public IEnumerable<string> CorsAllowedOrigin { get; set; }

        public SwaggerUiSettings SwaggerUiSettings { get; set; }

        public ApiAuthentication ApiAuthentication { get; set; }

        public PrivateServerAuthentication PrivateServerAuthentication { get; set; }

        public PrivateServerAuthentication EmailApiAuthentication { get; set; }
    }
}
