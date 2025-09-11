// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.TheIdServer.UI;

namespace Aguacongas.IdentityServer.UI.Device
{
    public class DeviceAuthorizationViewModel : ConsentViewModel
    {
        public string? UserCode { get; set; }
        public bool ConfirmUserCode { get; set; }
    }
}