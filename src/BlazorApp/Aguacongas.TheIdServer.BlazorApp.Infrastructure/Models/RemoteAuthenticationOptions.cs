// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    public abstract class RemoteAuthenticationOptions
    {
        public bool SaveTokens { get; set; }

        public string CallbackPath { get; set; }

        public string AccessDeniedPath { get; set; }

        public string ReturnUrlParameter { get; set; }
    }
}
