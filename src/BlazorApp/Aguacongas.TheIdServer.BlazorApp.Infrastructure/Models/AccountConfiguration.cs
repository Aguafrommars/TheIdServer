using System;

namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    public class AccountConfiguration
    {
        public bool AllowLocalLogin { get; set; } = true;
        public bool AllowRememberLogin { get; set; } = true;
        public TimeSpan RememberMeLoginDuration { get; set; }

        public bool ShowLogoutPrompt { get; set; } = true;
        public bool AutomaticRedirectAfterSignOut { get; set; }

        // if user uses windows auth, should we load the groups from windows
        public bool IncludeWindowsGroups { get; set; } = false;

        public string InvalidCredentialsErrorMessage { get; set; } = "Invalid username or password";
    }
}