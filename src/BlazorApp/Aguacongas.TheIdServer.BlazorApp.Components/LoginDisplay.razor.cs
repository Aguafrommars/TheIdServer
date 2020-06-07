using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Components
{
    public partial class LoginDisplay
    {
        private IEnumerable<string> _supportedCultures = Array.Empty<string>();
        private string _selectedCulture = CultureInfo.CurrentCulture.Name;

        protected override async Task OnInitializedAsync()
        {
            Localizer.OnResourceReady = () => InvokeAsync(StateHasChanged);
            _supportedCultures = await _shareLocalizer.GetSupportedCulturesAsync().ConfigureAwait(false);
            _selectedCulture = _supportedCultures.FirstOrDefault(c => c == CultureInfo.CurrentCulture.Name) ?? "en";
            await base.OnInitializedAsync().ConfigureAwait(false);
        }
        private async Task BeginSignOut(MouseEventArgs args)
        {
            await _signOutManager.SetSignOutState();
            _navigationManager.NavigateTo(_options.Value.LogOutPath);
        }

        private string GetActiveClass(string culture)
            => _selectedCulture == culture ? "active" : null;

        private async Task SetSelectCulture(string culture)
        {
            _selectedCulture = culture;
            CultureInfo.CurrentCulture = CultureInfo.GetCultures(CultureTypes.AllCultures)
                .FirstOrDefault(c => c.Name == culture) ?? CultureInfo.CurrentCulture;
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "culture", _selectedCulture).ConfigureAwait(false);
            await _shareLocalizer.Reset().ConfigureAwait(false);
        }
    }
}
