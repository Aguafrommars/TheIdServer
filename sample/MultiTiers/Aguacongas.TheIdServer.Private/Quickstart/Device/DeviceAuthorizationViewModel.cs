// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
namespace IdentityServerHost.Quickstart.UI
{
    public class DeviceAuthorizationViewModel : ConsentViewModel
    {
        public string UserCode { get; set; }
        public bool ConfirmUserCode { get; set; }
    }
}