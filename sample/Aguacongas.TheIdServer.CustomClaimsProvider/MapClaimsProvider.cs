// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Aguacongas.IdentityServer.Abstractions;
using Duende.IdentityServer.Models;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.CustomClaimsProviders
{
    public class MapClaimsProvider: IProvideClaims
    {
        public Task<IEnumerable<Claim>> ProvideClaims(ClaimsPrincipal subject, Client client, string caller, Resource resource)
        {
            var defaultOutboundClaimMap = JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap;
            var claims = new List<Claim>(subject.Claims.Count());
            foreach (var claim in subject.Claims)
            {
                if (defaultOutboundClaimMap.TryGetValue(claim.Type, out string toClaimType))
                {
                    claims.Add(new Claim(toClaimType, claim.Value, claim.ValueType, claim.Issuer));
                }
            }

            return Task.FromResult(claims as IEnumerable<Claim>);
        }
    }
}
