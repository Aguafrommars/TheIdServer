// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Microsoft.AspNetCore.Components;

namespace Aguacongas.TheIdServer.BlazorApp.Components.Form
{
    public partial class AuthorizeNumber<T>
    {
        [Parameter]
        public string Id { get; set; }
        [Parameter]
        public string Placeholder { get; set; }

        [Parameter]
        public int? Max { get; set; }

        [Parameter]
        public int? Min { get; set; }
    }
}
