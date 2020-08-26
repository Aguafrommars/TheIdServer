// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
namespace IdentityServer4.Quickstart.UI.Device
{
    public class DeviceAuthorizationViewModel : ConsentViewModel
    {
        public string UserCode { get; set; }
        public bool ConfirmUserCode { get; set; }
    }
}