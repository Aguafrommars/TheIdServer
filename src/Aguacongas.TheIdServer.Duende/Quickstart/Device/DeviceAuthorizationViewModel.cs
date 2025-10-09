// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.TheIdServer.Duende.Quickstart.Consent;

namespace Aguacongas.IdentityServer.UI.Device;

/// <summary>
/// View model for device authorization, extending <see cref="ConsentViewModel"/> to include device-specific properties.
/// </summary>
public class DeviceAuthorizationViewModel : ConsentViewModel
{
    /// <summary>
    /// Gets or sets the user code entered for device authorization.
    /// </summary>
    public string? UserCode { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user code should be confirmed.
    /// </summary>
    public bool ConfirmUserCode { get; set; }
}