using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.Components.Web;
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
            var response = await _store.GetAsync(new PageRequest
            {
                Select = nameof(Culture.Id),
                OrderBy = nameof(Culture.Id)
            }).ConfigureAwait(false);
            _supportedCultures = response.Items.Select(c => c.Id);
            _selectedCulture = _supportedCultures.FirstOrDefault(c => c == CultureInfo.CurrentCulture.Name) ?? "en-US";
            await base.OnInitializedAsync().ConfigureAwait(false);
        }
        private async Task BeginSignOut(MouseEventArgs args)
        {
            await _signOutManager.SetSignOutState();
            _navigationManager.NavigateTo("authentication/logout");
        }

        private string GetActiveClass(string culture)
            => _selectedCulture == culture ? "active" : null;

        private Task SetSelectCulture(string culture)
        {
            _selectedCulture = culture;
            CultureInfo.CurrentCulture = CultureInfo.GetCultures(CultureTypes.AllCultures)
                .FirstOrDefault(c => c.Name == culture) ?? CultureInfo.CurrentCulture;
            Console.WriteLine($"CurrentCulture {CultureInfo.CurrentCulture}");
            return _shareLocalizer.Reset();
        }
    }
}
