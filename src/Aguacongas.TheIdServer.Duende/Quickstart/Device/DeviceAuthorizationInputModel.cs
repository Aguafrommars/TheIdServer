// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.TheIdServer.Duende.Quickstart.Consent;

namespace Aguacongas.IdentityServer.UI.Device;

/// <summary>
/// Represents the input model for device authorization, extending <see cref="ConsentInputModel"/>.
/// </summary>
public class DeviceAuthorizationInputModel : ConsentInputModel
{
    /// <summary>
    /// Gets or sets the user code entered during device authorization.
    /// </summary>
    public string? UserCode { get; set; }
}