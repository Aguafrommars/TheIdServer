using System.Collections.Generic;

namespace Aguacongas.TheIdServer.Duende.Quickstart.Ciba
{
    public class ViewModel
    {
        public string? Id { get; set; }

        public string? ClientName { get; set; }
        public string? ClientUrl { get; set; }
        public string? ClientLogoUrl { get; set; }

        public string? BindingMessage { get; set; }

        public string? Description { get; set; }

        public InputModel? Input { get; set; }

        public IEnumerable<ScopeViewModel>? IdentityScopes { get; set; }
        public IEnumerable<ScopeViewModel>? ApiScopes { get; set; }
    }
}

