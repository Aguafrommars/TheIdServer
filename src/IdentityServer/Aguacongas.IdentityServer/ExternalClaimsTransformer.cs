using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer
{
    public class ExternalClaimsTransformer<TUser> where TUser : IdentityUser, new()
    {
        private readonly IAdminStore<ExternalClaimTransformation> _claimTransformationStore;

        public ExternalClaimsTransformer(IAdminStore<ExternalClaimTransformation> claimTransformationStore)
        {
            _claimTransformationStore = claimTransformationStore ?? throw new ArgumentNullException(nameof(claimTransformationStore));
        }

        public async Task<ClaimsPrincipal> TransformPrincipal(ClaimsPrincipal externalUser, string provider)
        {            
            var claims = new List<Claim>(externalUser.Claims.Count());
            var transformationsResponse = await _claimTransformationStore.GetAsync(new PageRequest
            {
                Filter = $"{nameof(ExternalClaimTransformation.Scheme)} eq '{provider}'"
            }).ConfigureAwait(false);

            var transformationList = transformationsResponse.Items;

            foreach (var claim in externalUser.Claims)
            {
                var transformation = transformationList.FirstOrDefault(t => t.FromClaimType == claim.Type);
                if (transformation != null)
                {
                    claims.Add(new Claim(transformation.ToClaimType, claim.Value));
                }
                // copy the claim as-is
                else
                {
                    claims.Add(claim);
                }
            }

            return new ClaimsPrincipal(new ClaimsIdentity(claims, provider));
        }
    }
}
