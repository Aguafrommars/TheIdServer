// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Microsoft.AspNetCore.Server.IISIntegration;
using System;

namespace Aguacongas.TheIdServer.UI
{
    public class AccountOptions
    {
        public bool AllowLocalLogin { get; set; } = true;
        public bool AllowRememberLogin { get; set; } = true;
        public TimeSpan RememberMeLoginDuration { get; set; } = TimeSpan.FromDays(30);

        public bool ShowLogoutPrompt { get; set; } = true;
        public bool AutomaticRedirectAfterSignOut { get; set; } = false;

        // specify the Windows authentication scheme being used
        public string WindowsAuthenticationSchemeName { get; } = IISDefaults.AuthenticationScheme;
        // if user uses windows auth, should we load the groups from windows
        public bool IncludeWindowsGroups { get; set; } = false;

        public string InvalidCredentialsErrorMessage { get; set; } = "Invalid username or password";
    }
}
