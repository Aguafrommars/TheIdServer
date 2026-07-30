// Project: Aguafrommars/TheIdServer
// Copyright (c) 2026 @Olivier Lefebvre
using System.Collections.Generic;

namespace Aguacongas.IdentityServer.Abstractions
{
    public interface ISupportCultures
    {
        public IEnumerable<string> CulturesNames { get; }
    }
}
