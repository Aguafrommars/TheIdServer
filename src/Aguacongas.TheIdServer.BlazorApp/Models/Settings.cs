using Aguacongas.TheIdServer.Blazor.Oidc;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    [SuppressMessage("Design", "CA1056:Uri properties should not be strings", Justification = "Uri should be string for deserialization")]
    public class Settings : AuthorizationOptions
    {
        public string ApiBaseUrl { get; set; }
    }
}
