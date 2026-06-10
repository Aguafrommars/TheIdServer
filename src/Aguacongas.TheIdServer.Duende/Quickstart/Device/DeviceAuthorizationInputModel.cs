// Project: Aguafrommars/TheIdServer
// Copyright (c) 2026 @Olivier Lefebvre
using Aguacongas.TheIdServer.UI;

namespace Aguacongas.IdentityServer.UI.Device
{
    public class DeviceAuthorizationInputModel : ConsentInputModel
    {
        public string? UserCode { get; set; }
    }
}