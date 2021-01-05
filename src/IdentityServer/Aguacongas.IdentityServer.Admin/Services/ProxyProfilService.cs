// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Abstractions;
using Aguacongas.IdentityServer.Store;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.Admin.Services
{
    /// <summary>
    /// Proy profile service
    /// </summary>
    /// <typeparam name="TUser">The type of the user.</typeparam>
    /// <seealso cref="ProfileService{TUser}" />
    public class ProxyProfilService<TUser> : ProfileService<TUser> where TUser : class
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyProfilService{TUser}"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="userManager">The user manager.</param>
        /// <param name="claimsFactory">The claims factory.</param>
        /// <param name="claimsProviders">The claims providers.</param>
        /// <param name="logger">The logger.</param>
        public ProxyProfilService(HttpClient httpClient,
            UserManager<TUser> userManager,
            IUserClaimsPrincipalFactory<TUser> claimsFactory,
            IEnumerable<IProvideClaims> claimsProviders,
            ILogger<ProxyProfilService<TUser>> logger)
            : base(userManager, claimsFactory, claimsProviders, logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient)); 
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
        protected override async Task<IEnumerable<Claim>> GetClaimsFromResource(Resource resource, ClaimsPrincipal subject, Client client, string caller, string providerTypeName)
        {
            var response = await _httpClient
                .GetAsync($"/claimsprovider?resource={resource.Name}&subject={subject.GetSubjectId()}&client={client.ClientId}&caller={caller}&type={providerTypeName}")
                .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            var page = JsonSerializer.Deserialize<PageResponse<Entity.UserClaim>>(content, _jsonSerializerOptions);

            return page.Items.Select(ToClaim);
        }

        private static Claim ToClaim(Entity.UserClaim userClaim)
        {
            return new Claim(userClaim.ClaimType, userClaim.ClaimValue, null, userClaim.Issuer);
        }
    }
}
