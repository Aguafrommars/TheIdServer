using System.Collections.Generic;

namespace Aguacongas.TheIdServer.Duende.Quickstart.Ciba
{
    public class InputModel
    {
        public string? Button { get; set; }
        public IEnumerable<string>? ScopesConsented { get; set; }
        public string? Id { get; set; }
        public string? Description { get; set; }

    }
}
