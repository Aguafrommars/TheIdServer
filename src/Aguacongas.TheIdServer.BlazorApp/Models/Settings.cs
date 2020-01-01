using Aguacongas.TheIdServer.Blazor.Oidc;
using System.Diagnostics.CodeAnalysis;

namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    [SuppressMessage("Design", "CA1056:Uri properties should not be strings", Justification = "Uri should be string for deserialization")]
    public class Settings : AuthorizationOptions
    {
        public string ApiBaseUrl { get; set; }
    }
}
