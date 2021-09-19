// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
#if DUENDE
using Duende.IdentityServer.Models;
#else
using IdentityServer4.Models;
#endif

namespace Aguacongas.TheIdServer.UI
{
    public class ProcessConsentResult
    {
        public bool IsRedirect => RedirectUri != null;
        public string RedirectUri { get; set; }
        public Client Client { get; set; }

        public bool ShowView => ViewModel != null;
        public ConsentViewModel ViewModel { get; set; }

        public bool HasValidationError => ValidationError != null;
        public string ValidationError { get; set; }
    }
}
