using Aguacongas.IdentityServer.Store;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace Aguacongas.TheIdServer.BlazorApp.Pages.Settings
{
    [Authorize(Policy = SharedConstants.DYNAMIC_CONFIGURATION_READER_POLICY)]
    public partial class Settings
    {
        [Parameter]
        public string? Path { get; set; }
    }
}
