// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using IdentityServer4.Models;

namespace IdentityServerHost.Quickstart.UI
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
