// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using System.Collections.Generic;

namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    public class FacebookOptions : OAuthOptions
    {
        public string AppId { get; set; }

        public string AppSecret { get; set; }

        public bool SendAppSecretProof { get; set; }

        public ICollection<string> Fields { get; set; }
    }
}
