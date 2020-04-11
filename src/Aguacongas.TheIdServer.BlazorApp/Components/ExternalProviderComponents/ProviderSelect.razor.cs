using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace Aguacongas.TheIdServer.BlazorApp.Components.ExternalProviderComponents
{
    public partial class ProviderSelect
    {
        [Parameter]
        public string Id { get; set; }

        [Parameter]
        public IEnumerable<ExternalProviderKind> Kinds { get; set; }
    }
}
