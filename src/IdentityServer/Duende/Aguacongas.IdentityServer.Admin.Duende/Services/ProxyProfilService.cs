// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.IdentityServer.Abstractions;
using Aguacongas.IdentityServer.Store;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
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

namespace Aguacongas.IdentityServer.Admin.Services;

/// <summary>
/// Proy profile service
/// </summary>
/// <typeparam name="TUser">The type of the user.</typeparam>
/// <seealso cref="ProfileService{TUser}" />
/// <remarks>
/// Initializes a new instance of the <see cref="ProxyProfilService{TUser}"/> class.
/// </remarks>
/// <param name="httpClient">The HTTP client.</param>
/// <param name="userManager">The user manager.</param>
/// <param name="claimsFactory">The claims factory.</param>
/// <param name="claimsProviders">The claims providers.</param>
/// <param name="logger">The logger.</param>
public class ProxyProfilService<TUser>(HttpClient httpClient,
    UserManager<TUser> userManager,
    IUserClaimsPrincipalFactory<TUser> claimsFactory,
    IEnumerable<IProvideClaims> claimsProviders,
    ILogger<ProxyProfilService<TUser>> logger) : ProfileService<TUser>(userManager, claimsFactory, claimsProviders, logger) where TUser : class
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    /// <summary>
    /// Gets the claims from resource.
    /// </summary>
    /// <param name="resource">The resource.</param>
    /// <param name="subject">The subject.</param>
    /// <param name="application">The client.</param>
    /// <param name="caller">The caller.</param>
    /// <param name="providerTypeName">Name of the provider type.</param>
    /// <returns></returns>
    protected override async Task<IEnumerable<Claim>> GetClaimsFromResource(Resource resource, ClaimsPrincipal subject, IConnectedApplication application, string caller, string providerTypeName)
    {
        var response = await _httpClient
            .GetAsync($"claimsprovider?resourceName={resource.Name}&userId={subject.GetSubjectId()}&clientId={application.Identifier}&caller={caller}&providerTypeName={providerTypeName}")
            .ConfigureAwait(false);

        var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var page = JsonSerializer.Deserialize<PageResponse<Entity.UserClaim>>(content, _jsonSerializerOptions);

        return page.Items.Select(ToClaim);
    }

    private static Claim ToClaim(Entity.UserClaim userClaim)
    {
        return new Claim(userClaim.ClaimType, userClaim.ClaimValue, null, userClaim.Issuer);
    }
}
