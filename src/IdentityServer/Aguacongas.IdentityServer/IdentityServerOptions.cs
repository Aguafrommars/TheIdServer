// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
namespace Aguacongas.IdentityServer
{
    public class IdentityServerOptions
    {
        public string Authority { get; set; }

        public string ApiUrl { get; set; }

        public string ClientId { get; set; } = "public-server";

        public string Scope { get; set; } = "theidserveradminapi";
        public string ClientSecret { get; set; } = "84137599-13d6-469c-9376-9e372dd2c1bd";
        public int RefreshBefore { get; set; } = 1;

        public string HttpClientName { get; set; } = "is4";
    }
}
