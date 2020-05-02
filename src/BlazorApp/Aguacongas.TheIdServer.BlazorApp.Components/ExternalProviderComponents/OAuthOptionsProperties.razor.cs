using Aguacongas.TheIdServer.BlazorApp.Models;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace Aguacongas.TheIdServer.BlazorApp.Components.ExternalProviderComponents
{
    public partial class OAuthOptionsProperties
    {
        [CascadingParameter]
        public ExternalProvider<OAuthOptions> Model { get; set; }

        protected override void OnInitialized()
        {
            Model.Options.Scope = Model.Options.Scope ?? new List<string>();
            base.OnInitialized();
        }
    }
}
