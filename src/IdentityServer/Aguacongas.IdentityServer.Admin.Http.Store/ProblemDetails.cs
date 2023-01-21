// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using System.Text.Json.Serialization;

namespace Aguacongas.IdentityServer.Admin.Http.Store
{
    public class ProblemDetails
    {
        [JsonPropertyName("detail")]
        public string Detail { get; set; }
        [JsonPropertyName("instance")]
        public string Instance { get; set; }
        [JsonPropertyName("status")]
        public int? Status { get; set; }
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }
    }
}
