// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Aguacongas.TheIdServer.BlazorApp.Pages.ExternalProvider.Components
{
    public partial class WsFederationOptionsComponent
    {
        [CascadingParameter]
        public EditContext EditContext { get; set; }

        public bool RequireHttpsMetadata {
            get => Model.Options.RequireHttpsMetadata;
            set
            {
                Model.Options.RequireHttpsMetadata = value;
                EditContext.NotifyFieldChanged(new FieldIdentifier(Model.Options, nameof(Model.Options.MetadataAddress)));
            }
        }
    }
}
