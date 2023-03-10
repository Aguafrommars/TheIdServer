// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Pages.Keys.Components
{
    public partial class KeyCollection
    {
        [Parameter]
        public EventCallback<Tuple<string, string>> RevokeClick { get; set; }

        [Parameter]
        public bool ShowAlgorithm { get; set; }

        static string GetRevokedClass(Key key)
            => key.IsRevoked ? "text-body-tertiary" : null;
        
        private Task RevokeConfirmed(Tuple<string, string> tuple)
            => RevokeClick.InvokeAsync(tuple);

    }
}
