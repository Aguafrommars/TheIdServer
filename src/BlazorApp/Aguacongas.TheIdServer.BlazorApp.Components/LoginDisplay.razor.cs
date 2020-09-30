// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
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
            var currentCulture = CultureInfo.CurrentCulture;
            _selectedCulture = _supportedCultures.FirstOrDefault(c => c == currentCulture.Name ||
                (currentCulture.Parent != null && c == currentCulture.Parent.Name)) ?? "en";
            SetPathsCulture();
            await base.OnInitializedAsync().ConfigureAwait(false);
        }

        private void SetPathsCulture()
        {
            var settings = _options.Value;
            settings.RemoteRegisterPath = SetCultureInPath(settings.RemoteRegisterPath);
            settings.RemoteProfilePath = SetCultureInPath(settings.RemoteProfilePath);
            var oidcSettings = _oidcOptions.Value;
            oidcSettings.Authority = SetCultureInPath(oidcSettings.Authority);

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

            var settings = _options.Value;
            settings.RemoteRegisterPath = ResetCultureInPath(settings.RemoteProfilePath);
            settings.RemoteProfilePath = ResetCultureInPath(settings.RemoteProfilePath);
            var oidcSettings = _oidcOptions.Value;
            oidcSettings.Authority = ResetCultureInPath(oidcSettings.Authority);

            SetPathsCulture();
        }

        private string ResetCultureInPath(string path)
            => path != null ? path.Split('?')[0] : null;

        private string SetCultureInPath(string path)
            => path != null ? $"{path}?culture={_selectedCulture}" : null;
    }
}
