// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Threading.Tasks;
using MsOptions = Microsoft.Extensions.Options;

namespace Aguacongas.TheIdServer.BlazorApp.Components
{
    public partial class RedirectToLogin
    {
        private string _administratorEmail;
        protected override async Task OnInitializedAsync()
        {
            var authenticationState = await _authenticationStateProvider.GetAuthenticationStateAsync().ConfigureAwait(false);
            var identity = authenticationState.User?.Identity;
            if (identity != null && !identity.IsAuthenticated)
            {
                _navigationManager.NavigateToLogin(Options.Get(MsOptions.Options.DefaultName).AuthenticationPaths.LogInPath);
                return;
            }
            _administratorEmail = _settings.AdministratorEmail;
        }

    }
}
