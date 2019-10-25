using System.Text.Json.Serialization;

namespace Aguacongas.Blazor.Oidc
{
    public class Tokens
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }
    }
}
