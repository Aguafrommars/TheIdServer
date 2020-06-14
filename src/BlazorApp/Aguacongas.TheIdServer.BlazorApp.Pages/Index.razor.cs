using Microsoft.AspNetCore.Components;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Pages
{
    public partial class Index
    {
        private RenderFragment _renderFragment;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync().ConfigureAwait(false);
            await GetRenderFragmentAsync().ConfigureAwait(false);
            Localizer.OnResourceReady = () => InvokeAsync(async () => 
            {
                await GetRenderFragmentAsync().ConfigureAwait(false);
                StateHasChanged();
            });
        }

        private async Task GetRenderFragmentAsync()
        {
            var response = await _httpClient.GetAsync($"{_settings.WelcomeContenUrl}?culture={CultureInfo.CurrentCulture.Name}").ConfigureAwait(false);
            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            _renderFragment = builder =>
            {
                builder.OpenElement(1, "p");
                builder.AddContent(2, new MarkupString(content));
                builder.CloseElement();
            };
        }
    }
}
