namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    public class OAuth2Client
    {
        public string ClientId { get; set; }

        public string AppName { get; set; }

        public bool UsePkceWithAuthorizationCodeGrant { get; set; }
    }
}