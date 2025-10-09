// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
namespace Aguacongas.TheIdServer.Duende.Quickstart.Account;

/// <summary>
/// View model for logout, extending <see cref="LogoutInputModel"/>.
/// </summary>
public class LogoutViewModel : LogoutInputModel
{
    /// <summary>
    /// Gets or sets a value indicating whether to show the logout prompt.
    /// </summary>
    public bool ShowLogoutPrompt { get; set; } = true;
}
