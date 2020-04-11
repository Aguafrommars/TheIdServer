using Aguacongas.TheIdServer.BlazorApp.Models;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace Aguacongas.TheIdServer.BlazorApp.Components.ExternalProviderComponents
{
    public partial class OAuthOptionsProperties
    {
        [Parameter]
        public OAuthOptions Options { get; set; }

        protected override void OnInitialized()
        {
            Options.Scope = Options.Scope ?? new List<string>();
            base.OnInitialized();
        }
    }
}
