// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Microsoft.AspNetCore.Server.IISIntegration;
using System;

namespace IdentityServer4.Quickstart.UI
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
