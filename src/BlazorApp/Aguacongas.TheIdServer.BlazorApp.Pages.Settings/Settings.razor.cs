using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace Aguacongas.TheIdServer.BlazorApp.Pages.Settings
{
    [Authorize(Policy = "Read-Settings")]
    public partial class Settings
    {
        [Parameter]
        public string? Path { get; set; }
    }
}
