// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
namespace Aguacongas.TheIdServer.MvcClient.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
