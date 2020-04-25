namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    public class TwitterOptions : RemoteAuthenticationOptions
    {

        public string ConsumerKey { get; set; }

        public string ConsumerSecret { get; set; }

        public bool RetrieveUserDetails { get; set; }
    }
}
