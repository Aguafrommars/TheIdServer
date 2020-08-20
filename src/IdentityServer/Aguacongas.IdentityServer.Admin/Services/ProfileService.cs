﻿using Aguacongas.IdentityServer.Abstractions;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Admin.Services
{
    /// <summary>
    /// Defines profile service properties key constantes
    /// </summary>
    public static class ProfileServiceProperties
    {
        /// <summary>
        /// The claim builder assembly path key
        /// </summary>
        public static readonly string ClaimProviderAssemblyPathKey = "ClaimProviderAssemblyPath";
        /// <summary>
        /// The claim builder type key
        /// </summary>
        public static readonly string ClaimProviderTypeKey = "ClaimProviderType";
    }

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="DefaultProfileService" />
    public class ProfileService<TUser> : IdentityServer4.AspNetIdentity.ProfileService<TUser> where TUser : class
    {
        private readonly IEnumerable<IProvideClaims> _claimsProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileService{TUser}"/> class.
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="claimsFactory">The claims factory.</param>
        /// <param name="claimsProviders">The claims providers.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="ArgumentNullException">claimsProviders</exception>
        public ProfileService(UserManager<TUser> userManager, 
            IUserClaimsPrincipalFactory<TUser> claimsFactory,
            IEnumerable<IProvideClaims> claimsProviders,
            ILogger<ProfileService<TUser>> logger) : base(userManager, claimsFactory, logger)
        {
            _claimsProvider = claimsProviders ?? throw new ArgumentNullException(nameof(claimsProviders));
        }

        /// <summary>
        /// This method is called whenever claims about the user are requested (e.g. during token creation or via the userinfo endpoint)
        /// </summary>
        /// <param name="context">The context.</param>
        public override async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var user = await FindUserAsync(context.Subject.GetSubjectId()).ConfigureAwait(false);
            var principal = user != null ? await GetClaimsPrincipalAsync(user).ConfigureAwait(false) : context.Subject;

            var requestedResources = context.RequestedResources.Resources;
            
            foreach (var resource in requestedResources.IdentityResources)
            {
                var claims = await GetClaimsFromResource(resource, principal, context.Client, context.Caller).ConfigureAwait(false);
                context.AddRequestedClaims(claims);
            }
            
            foreach (var resource in requestedResources.ApiResources)
            {
                var claims = await GetClaimsFromResource(resource, principal, context.Client, context.Caller).ConfigureAwait(false);
                context.AddRequestedClaims(claims);
            }
            
            foreach (var resource in requestedResources.ApiScopes)
            {
                var claims = await GetClaimsFromResource(resource, principal, context.Client, context.Caller).ConfigureAwait(false);
                context.AddRequestedClaims(claims);
            }

            await base.GetProfileDataAsync(context).ConfigureAwait(false);

            context.IssuedClaims = SanetizeIssuedClaims(context.IssuedClaims);
        }


        /// <summary>
        /// Gets the claims for a user.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        protected virtual async Task<ClaimsPrincipal> GetClaimsPrincipalAsync(TUser user)
        {
            var principal = await ClaimsFactory.CreateAsync(user).ConfigureAwait(false);
            if (principal == null)
            {
                throw new InvalidOperationException("ClaimsFactory failed to create a principal");
            }

            return principal;
        }

        /// <summary>
        /// Gets the claims from resource.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="client">The client.</param>
        /// <param name="caller">The caller.</param>
        /// <param name="providerTypeName">Name of the provider type.</param>
        /// <returns></returns>
        protected virtual Task<IEnumerable<Claim>> GetClaimsFromResource(Resource resource, ClaimsPrincipal subject, Client client, string caller, string providerTypeName)
        {
            var provider = _claimsProvider.FirstOrDefault(p => p.GetType().FullName == providerTypeName);

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

        private Task<IEnumerable<Claim>> GetClaimsFromResource(Resource resource, ClaimsPrincipal subject, Client client, string caller)
        {
            if (!resource.Properties.TryGetValue(ProfileServiceProperties.ClaimProviderTypeKey, out string providerTypeName))
            {
                return Task.FromResult(Array.Empty<Claim>() as IEnumerable<Claim>);
            }

            return GetClaimsFromResource(resource, subject, client, caller, providerTypeName);
        }

        private List<Claim> SanetizeIssuedClaims(List<Claim> issuedClaims)
        {
            var claimList = new List<Claim>(issuedClaims.Count);

            foreach (var claim in issuedClaims)
            {
                var value = claim.Value;
                if (claim.Type == JwtClaimTypes.UpdatedAt)
                {
                    claimList.Add(new Claim(claim.Type, value, ClaimValueTypes.Integer64));
                    continue;
                }

                if (claim.Type == JwtClaimTypes.Address)
                {
                    try
                    {
                        JsonSerializer.Deserialize<JsonElement>(value);
                        claimList.Add(new Claim(claim.Type, value, IdentityServerConstants.ClaimValueTypes.Json));
                        continue;
                    }
                    catch
                    {
                        // silent
                    }
                }

                claimList.Add(claim);
            }

            return claimList;
        }
    }
}
