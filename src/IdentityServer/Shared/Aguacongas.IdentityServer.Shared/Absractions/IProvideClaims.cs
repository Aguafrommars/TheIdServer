// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
#if DUENDE
using Duende.IdentityServer.Models;
#else
using IdentityServer4.Models;
#endif
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Abstractions
{
    public interface IProvideClaims
    {
        Task<IEnumerable<Claim>> ProvideClaims(ClaimsPrincipal subject, Client client, string caller, Resource resource);
    }
}
