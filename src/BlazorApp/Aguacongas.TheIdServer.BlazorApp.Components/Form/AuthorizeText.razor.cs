// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Microsoft.AspNetCore.Components;

namespace Aguacongas.TheIdServer.BlazorApp.Components.Form
{
    public partial class AuthorizeText
    {
        [Parameter]
        public string Id { get; set; }
        [Parameter]
        public string Placeholder { get; set; }

        [Parameter]
        public int? MaxLength { get; set; }
    }
}
