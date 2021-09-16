// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
#if DUENDE
using Duende.IdentityServer.Models;
#else
using IdentityServer4.Models;
#endif

namespace Aguacongas.TheIdServer.UI
{
    public class ErrorViewModel
    {
        public ErrorViewModel()
        {
        }

        public ErrorViewModel(string error)
        {
            Error = new ErrorMessage { Error = error };
        }

        public ErrorMessage Error { get; set; }
    }
}