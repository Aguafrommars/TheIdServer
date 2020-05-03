using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using IdentityModel;
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
        private readonly UserManager<TUser> _userManager;
        private readonly IAdminStore<ExternalClaimTransformation> _claimTransformationStore;
        private readonly IAdminStore<ExternalProvider> _externalProviderStore;

        public ExternalClaimsTransformer(UserManager<TUser> userManager,
            IAdminStore<ExternalClaimTransformation> claimTransformationStore,
            IAdminStore<ExternalProvider> externalProviderStore)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _claimTransformationStore = claimTransformationStore ?? throw new ArgumentNullException(nameof(claimTransformationStore));
            _externalProviderStore = externalProviderStore ?? throw new ArgumentNullException(nameof(externalProviderStore));
        }

        public async Task<ClaimsPrincipal> TransformPrincipalAsync(ClaimsPrincipal externalUser, string provider)
        {            
            var claims = new List<Claim>(externalUser.Claims.Count());
            var externalProvider = await _externalProviderStore.GetAsync(provider, new GetRequest()).ConfigureAwait(false);
            if (externalProvider.StoreClaims)
            {
                await StoreClaims(externalUser, provider, claims).ConfigureAwait(false);
            }

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

        private async Task StoreClaims(ClaimsPrincipal externalUser, string provider, List<Claim> claims)
        {
            var (user, providerUserId) = await FindUserFromExternalProviderAsync(externalUser, provider)
                .ConfigureAwait(false);

            if (user == null)
            {
                user = await AutoProvisionUserAsync(provider, providerUserId, claims)
                    .ConfigureAwait(false);
            }
            else
            {
                await _userManager.RemoveClaimsAsync(user, claims).ConfigureAwait(false);
            }

            await _userManager.AddClaimsAsync(user, claims).ConfigureAwait(false);
        }

        private async Task<(TUser user, string providerUserId)>
            FindUserFromExternalProviderAsync(ClaimsPrincipal externalUser, string provider)
        {
            // try to determine the unique id of the external user (issued by the provider)
            // the most common claim type for that are the sub claim and the NameIdentifier
            // depending on the external provider, some other claim type might be used
            var userIdClaim = externalUser.FindFirst(JwtClaimTypes.Subject) ??
                              externalUser.FindFirst(ClaimTypes.NameIdentifier) ??
                              throw new InvalidOperationException("Unknown userid");

            // remove the user id claim so we don't include it as an extra claim if/when we provision the user
            var claims = externalUser.Claims.ToList();
            claims.Remove(userIdClaim);

            var providerUserId = userIdClaim.Value;

            // find external user
            var user = await _userManager.FindByLoginAsync(provider, providerUserId).ConfigureAwait(false);

            return (user, providerUserId);
        }

        private async Task<TUser> AutoProvisionUserAsync(string provider, string providerUserId, IEnumerable<Claim> claims)
        {            
            // email
            var email = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Email)?.Value ??
               claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
         
            var user = new TUser
            {
                UserName = email ?? Guid.NewGuid().ToString(),
            };

            var identityResult = await _userManager.CreateAsync(user).ConfigureAwait(false);
            if (!identityResult.Succeeded)
            {
                throw new InvalidOperationException(identityResult.Errors.First().Description);
            }

            identityResult = await _userManager.AddLoginAsync(user, new UserLoginInfo(provider, providerUserId, provider))
                .ConfigureAwait(false);

            if (!identityResult.Succeeded)
            {
                throw new InvalidOperationException(identityResult.Errors.First().Description);
            }

            return user;
        }

    }
}
