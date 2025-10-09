// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.TheIdServer.Duende.Quickstart.Account;

namespace Aguacongas.TheIdServer.UI;

/// <summary>
/// View model for the login page, extending <see cref="LoginInputModel"/>.
/// Provides properties for local and external login options and UI controls.
/// </summary>
public class LoginViewModel : LoginInputModel
{
    /// <summary>
    /// Gets or sets a value indicating whether the "Remember Login" option is allowed.
    /// </summary>
    public bool AllowRememberLogin { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether local login is enabled.
    /// </summary>
    public bool EnableLocalLogin { get; set; } = true;

    /// <summary>
    /// Gets or sets the collection of configured external authentication providers.
    /// </summary>
    public IEnumerable<ExternalProvider>? ExternalProviders { get; set; } = [];

    /// <summary>
    /// Gets the collection of external providers that have a display name and are visible to the user.
    /// </summary>
    public IEnumerable<ExternalProvider>? VisibleExternalProviders => ExternalProviders?.Where(x => !string.IsNullOrWhiteSpace(x.DisplayName));

    /// <summary>
    /// Gets a value indicating whether only external login is allowed and there is exactly one external provider.
    /// </summary>
    public bool IsExternalLoginOnly => !EnableLocalLogin && ExternalProviders?.Count() == 1;

    /// <summary>
    /// Gets the authentication scheme for the single external provider if only external login is allowed; otherwise, null.
    /// </summary>
    public string? ExternalLoginScheme => IsExternalLoginOnly ? ExternalProviders?.SingleOrDefault()?.AuthenticationScheme : null;

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