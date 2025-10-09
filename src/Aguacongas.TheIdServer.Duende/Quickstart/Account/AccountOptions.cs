// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Microsoft.AspNetCore.Authentication.Negotiate;

namespace Aguacongas.TheIdServer.UI;

/// <summary>
/// Provides configuration options for account-related features in the authentication UI.
/// </summary>
public class AccountOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether local login is allowed.
    /// </summary>
    public bool AllowLocalLogin { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the "Remember Me" option is available during login.
    /// </summary>
    public bool AllowRememberLogin { get; set; } = true;

    /// <summary>
    /// Gets or sets the duration for which the "Remember Me" login persists.
    /// </summary>
    public TimeSpan RememberMeLoginDuration { get; set; } = TimeSpan.FromDays(30);

    /// <summary>
    /// Gets or sets a value indicating whether a logout prompt should be shown to the user.
    /// </summary>
    public bool ShowLogoutPrompt { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the user should be automatically redirected after signing out.
    /// </summary>
    public bool AutomaticRedirectAfterSignOut { get; set; } = false;

    /// <summary>
    /// Gets the Windows authentication scheme name being used.
    /// </summary>
    public string WindowsAuthenticationSchemeName { get; } = NegotiateDefaults.AuthenticationScheme;

    /// <summary>
    /// Gets or sets a value indicating whether to include Windows groups when using Windows authentication.
    /// </summary>
    public bool IncludeWindowsGroups { get; set; } = false;

    /// <summary>
    /// Gets or sets the error message displayed for invalid credentials.
    /// </summary>
    public string InvalidCredentialsErrorMessage { get; set; } = "Invalid username or password";

    /// <summary>
    /// Gets or sets a value indicating whether the "Forgot Password" link should be shown.
    /// </summary>
    public bool ShowForgotPassworLink { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the "Register" link should be shown.
    /// </summary>
    public bool ShowRegisterLink { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the "Resend Email Confirmation" link should be shown.
    /// </summary>
    public bool ShowResendEmailConfirmationLink { get; set; } = true;
}
