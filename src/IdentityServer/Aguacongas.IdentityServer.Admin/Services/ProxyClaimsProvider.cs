using Aguacongas.IdentityServer.Abstractions;
using Aguacongas.IdentityServer.Store;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.Admin.Services
{
    /// <summary>
    /// PRoxy user claims provider.
    /// </summary>
    /// <typeparam name="TUser">The type of the user.</typeparam>
    /// <seealso cref="IProxyClaimsProvider" />
    public class ProxyClaimsProvider<TUser> : IProxyClaimsProvider where TUser : class
    {
        private readonly IEnumerable<IProvideClaims> _claimsProviders;
        private readonly IResourceStore _resourceStore;
        private readonly IClientStore _clientStore;
        private readonly UserManager<TUser> _userManager;
        private readonly IUserClaimsPrincipalFactory<TUser> _principalFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyClaimsProvider{TUser}"/> class.
        /// </summary>
        /// <param name="claimsProviders">The claims providers.</param>
        /// <param name="resourceStore">The resource store.</param>
        /// <param name="clientStore">The client store.</param>
        /// <param name="userManager">The user manager.</param>
        /// <param name="principalFactory">The principal factory.</param>
        /// <exception cref="ArgumentNullException">
        /// claimsProviders
        /// or
        /// resourceStore
        /// or
        /// clientStore
        /// or
        /// userManager
        /// or
        /// principalFactory
        /// </exception>
        public ProxyClaimsProvider(IEnumerable<IProvideClaims> claimsProviders,
            IResourceStore resourceStore,
            IClientStore clientStore,
            UserManager<TUser> userManager,
            IUserClaimsPrincipalFactory<TUser> principalFactory)
        {
            _claimsProviders = claimsProviders ?? throw new ArgumentNullException(nameof(claimsProviders));
            _resourceStore = resourceStore ?? throw new ArgumentNullException(nameof(resourceStore));
            _clientStore = clientStore ?? throw new ArgumentNullException(nameof(clientStore));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _principalFactory = principalFactory ?? throw new ArgumentNullException(nameof(principalFactory));
        }

        /// <summary>
        /// Gets user claims.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="caller">The caller.</param>
        /// <returns></returns>
        public async Task<PageResponse<Entity.UserClaim>> GetAsync(string resourceName, string userId, string clientId, string caller)
        {
            var identityResourceList = await _resourceStore.FindEnabledIdentityResourcesByScopeAsync(new string[] { resourceName })
                .ConfigureAwait(false);
            var apiResource = await _resourceStore.FindApiResourceAsync(resourceName).ConfigureAwait(false);
            var client = await _clientStore.FindClientByIdAsync(clientId).ConfigureAwait(false);
            var user = await _userManager.FindByIdAsync(userId).ConfigureAwait(false);

            var claimList = new List<Claim>();
            foreach (var resource in identityResourceList)
            {
                claimList.AddRange(await GetClaimsFromResource(resource,
                    await _principalFactory.CreateAsync(user).ConfigureAwait(false),
                    client,
                    caller).ConfigureAwait(false));
            }

            if (apiResource != null)
            {
                claimList.AddRange(await GetClaimsFromResource(apiResource,
                    await _principalFactory.CreateAsync(user).ConfigureAwait(false),
                    client,
                    caller).ConfigureAwait(false));
            }

            return new PageResponse<Entity.UserClaim>
            {
                Count = claimList.Count,
                Items = claimList.Select(ToUserClaim)
            };
        }

        private Task<IEnumerable<Claim>> GetClaimsFromResource(Resource resource, ClaimsPrincipal subject, Client client, string caller)
        {
            if (!resource.Properties.TryGetValue(ProfileServiceProperties.ClaimProviderTypeKey, out string providerTypeName))
            {
                return Task.FromResult(Array.Empty<Claim>() as IEnumerable<Claim>);
            }

            var provider = _claimsProviders.FirstOrDefault(p => p.GetType().FullName == providerTypeName);

            if (provider == null)
            {
                var path = resource.Properties[ProfileServiceProperties.ClaimProviderAssemblyPathKey];
#pragma warning disable S3885 // "Assembly.Load" should be used
                var assembly = Assembly.LoadFrom(path);
#pragma warning restore S3885 // "Assembly.Load" should be used
                var type = assembly.GetType(providerTypeName);
                provider = Activator.CreateInstance(type) as IProvideClaims;
            }

            return provider.ProvideClaims(subject, client, caller, resource);
        }

        private static Entity.UserClaim ToUserClaim(Claim claim)
        {
            return new Entity.UserClaim
            {
                ClaimType = claim.Type,
                ClaimValue = claim.Value,
                Issuer = claim.Issuer
            };
        }
    }
}
