// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using System.Collections.Generic;

namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    public class OAuthOptions : RemoteAuthenticationOptions
    {
        public string AuthorizationEndpoint { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string TokenEndpoint { get; set; }

        public bool UsePkce { get; set; }

        public string UserInformationEndpoint { get; set; }

        public ICollection<string> Scope { get; set; }
    }
}
