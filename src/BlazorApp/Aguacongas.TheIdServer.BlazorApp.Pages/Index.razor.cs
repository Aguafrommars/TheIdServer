using Microsoft.AspNetCore.Components;
using System.Globalization;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Pages
{
    public partial class Index
    {
        private RenderFragment _renderFragment;

        protected override async Task OnInitializedAsync()
        {
            using var response = await _httpClient.GetAsync($"{_settings.WelcomeContenUrl}?culture={CultureInfo.CurrentCulture.Name}").ConfigureAwait(false);
            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            _renderFragment = builder =>
            {
                builder.OpenElement(1, "p");
                builder.AddContent(2, new MarkupString(content));
                builder.CloseElement();
            };
            base.OnInitialized();
        }
    }
}
