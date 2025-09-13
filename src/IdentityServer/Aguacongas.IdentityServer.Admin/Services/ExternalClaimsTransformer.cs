// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Admin.Services
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TUser">The type of the user.</typeparam>
    public class ExternalClaimsTransformer<TUser> where TUser : IdentityUser, new()
    {
        private static readonly List<string> _userIdClaimTypes = new List<string>
        {
            JwtClaimTypes.Subject,
            JwtClaimTypes.Id,
            JwtClaimTypes.Name,
            ClaimTypes.NameIdentifier,
            ClaimTypes.Name,
            JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap[ClaimTypes.NameIdentifier],
            JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap[ClaimTypes.Name]
        };

        private readonly UserManager<TUser> _userManager;
        private readonly IAdminStore<ExternalClaimTransformation> _claimTransformationStore;
        private readonly IAdminStore<ExternalProvider> _externalProviderStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalClaimsTransformer{TUser}"/> class.
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="claimTransformationStore">The claim transformation store.</param>
        /// <param name="externalProviderStore">The external provider store.</param>
        /// <exception cref="ArgumentNullException">
        /// userManager
        /// or
        /// claimTransformationStore
        /// or
        /// externalProviderStore
        /// </exception>
        public ExternalClaimsTransformer(UserManager<TUser> userManager,
            IAdminStore<ExternalClaimTransformation> claimTransformationStore,
            IAdminStore<ExternalProvider> externalProviderStore)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _claimTransformationStore = claimTransformationStore ?? throw new ArgumentNullException(nameof(claimTransformationStore));
            _externalProviderStore = externalProviderStore ?? throw new ArgumentNullException(nameof(externalProviderStore));
        }

        /// <summary>
        /// Transforms the principal asynchronous.
        /// </summary>
        /// <param name="externalUser">The external user.</param>
        /// <param name="provider">The provider.</param>
        /// <returns></returns>
        public async Task<ClaimsPrincipal> TransformPrincipalAsync(ClaimsPrincipal externalUser, string provider)
        {            
            var claims = new List<Claim>(externalUser.Claims.Count());
            var transformationsResponse = await _claimTransformationStore.GetAsync(new PageRequest
            {
                Filter = $"{nameof(ExternalClaimTransformation.Scheme)} eq '{provider}'"
            }).ConfigureAwait(false);
            
            var externalProvider = await _externalProviderStore.GetAsync(provider, new GetRequest()).ConfigureAwait(false);
            var transformationList = transformationsResponse.Items;
            var defaultOutboundClaimMap = JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap;
            var mapDefaultOutboundClaimType = externalProvider.MapDefaultOutboundClaimType;

            foreach (var claim in externalUser.Claims)
            {
                var transformation = transformationList.FirstOrDefault(t => t.FromClaimType == claim.Type);
                if (transformation != null)
                {
                    TransformClaimType(claims, claim, transformation.ToClaimType);
                    continue;
                }
                if (mapDefaultOutboundClaimType && defaultOutboundClaimMap.TryGetValue(claim.Type, out string toClaimType))
                {
                    TransformClaimType(claims, claim, toClaimType);
                    continue;
                }
                // copy the claim as-is
                claims.Add(claim);
            }

            if (externalProvider.StoreClaims)
            {
                await StoreClaims(externalUser, provider, claims).ConfigureAwait(false);
                // We store only user id claims to reduce the session cookie size to the minimum.
                // That's avoid request header too large exception.
                var newUserClaims = claims.Where(c => _userIdClaimTypes.Contains(c.Type)).Distinct().ToList();

                return new ClaimsPrincipal(new ClaimsIdentity(newUserClaims, provider));
            }

            return new ClaimsPrincipal(new ClaimsIdentity(claims, provider));
        }

        private static void TransformClaimType(List<Claim> claims, Claim claim, string toClaimType)
        {
            var newClaim = new Claim(toClaimType, claim.Value, claim.ValueType, claim.Issuer);
            newClaim.Properties.Add(nameof(UserClaim.OriginalType), claim.Type);
            claims.Add(newClaim);
        }

        private async Task StoreClaims(ClaimsPrincipal externalUser, string provider, List<Claim> claims)
        {
            var (user, providerUserId) = await FindUserFromExternalProviderAsync(externalUser, provider)
                .ConfigureAwait(false);

            if (user == null)
            {
                await AutoProvisionUserAsync(provider, providerUserId, claims)
                    .ConfigureAwait(false);
                return;
            }

            var userClaims = await _userManager.GetClaimsAsync(user).ConfigureAwait(false);
            // remove delete claims
            var deleteClaims = userClaims
                    .Where(c => !claims.Any(uc => uc.Type == c.Type &&
                        uc.Value == c.Value &&
                        uc.Issuer == c.Issuer));
            if (deleteClaims.Any())
            {
                await _userManager.RemoveClaimsAsync(user, deleteClaims)
                    .ConfigureAwait(false);
            }
            // add new claims
            var newClaims = claims
                    .Where(c => !userClaims.Any(uc => uc.Type == c.Type &&
                        uc.Value == c.Value &&
                        uc.Issuer == c.Issuer));
            if (newClaims.Any())
            {
                await _userManager.AddClaimsAsync(user, newClaims)
                    .ConfigureAwait(false);
            }
        }

        private async Task<(TUser user, string providerUserId)>
            FindUserFromExternalProviderAsync(ClaimsPrincipal externalUser, string provider)
        {
            // try to determine the unique id of the external user (issued by the provider)
            // the most common claim type for that are the sub claim and the NameIdentifier
            // depending on the external provider, some other claim type might be used
            var userIdClaim = externalUser.FindFirst(JwtClaimTypes.Subject) ??
                              externalUser.FindFirst(JwtClaimTypes.Id) ??
                              externalUser.FindFirst(ClaimTypes.NameIdentifier) ??
                              externalUser.FindFirst(ClaimTypes.Name) ??
                              throw new InvalidOperationException("Unknown userid");

            // remove the user id claim so we don't include it as an extra claim if/when we provision the user
            var claims = externalUser.Claims.ToList();
            claims.Remove(userIdClaim);

            var providerUserId = userIdClaim.Value;

            // find external user
            var user = await _userManager.FindByLoginAsync(provider, providerUserId).ConfigureAwait(false);

            return (user, providerUserId);
        }

        private async Task AutoProvisionUserAsync(string provider, string providerUserId, IEnumerable<Claim> claims)
        {            
            // email
            var email = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Email)?.Value ??
               claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            var name = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Name)?.Value ??
                claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
            var allowedUserNameCharacters = _userManager.Options.User?.AllowedUserNameCharacters;
            email = email?.All(c => allowedUserNameCharacters.Contains(c)) == true ? email : null;
            name = name?.All(c => allowedUserNameCharacters.Contains(c)) == true ? name : null;
            var sanetized = providerUserId.All(c => allowedUserNameCharacters.Contains(c)) ? providerUserId : null;

            var user = new TUser
            {
                UserName = name ?? email ?? sanetized ?? Guid.NewGuid().ToString()
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

            await _userManager.AddClaimsAsync(user, claims).ConfigureAwait(false);
        }

    }
}
