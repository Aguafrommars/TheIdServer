using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    public class OidcAccount : RemoteUserAccount
    {
        [JsonPropertyName("role")]
        [JsonConverter(typeof(JsonRoleConverter))]
        public IEnumerable<string> Roles { get; set; }
    }
}
