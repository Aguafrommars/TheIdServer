// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Microsoft.AspNetCore.Components;

namespace Aguacongas.TheIdServer.BlazorApp.Components.Form
{
    public partial class AuthorizeTextArea
    {
        [Parameter]
        public string Id { get; set; }
        [Parameter]
        public string Placeholder { get; set; }

        [Parameter]
        public int? MaxLength { get; set; }
    }
}
