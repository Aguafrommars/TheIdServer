using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Shared
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
                _navigationManager.NavigateTo($"authentication/login?returnUrl={_navigationManager.Uri}");
                return;
            }
            _administratorEmail = (await _getSettingsTask.ConfigureAwait(false)).AdministratorEmail;
        }

    }
}
