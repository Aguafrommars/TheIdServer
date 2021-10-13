using Aguacongas.IdentityServer.Abstractions;
#if DUENDE
using Duende.IdentityServer;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
#else
using IdentityServer4;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using IdentityModel;
using System.IdentityModel.Tokens.Jwt;

namespace Aguacongas.IdentityServer.Admin.Services
{
    /// <summary>
    /// Creates personal access tokens
    /// </summary>
    public class CreatePersonalAccessTokenService : ICreatePersonalAccessToken
    {
        private readonly ITokenService _tokenService;
        private readonly IClientStore _clientStore;
        private readonly IResourceStore _resourceStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreatePersonalAccessTokenService"/> class.
        /// </summary>
        /// <param name="tokenService">The token service.</param>
        /// <param name="clientStore">The client store.</param>
        /// <param name="resourceStore">The resource store.</param>
        /// <exception cref="System.ArgumentNullException">
        /// tokenService
        /// or
        /// clientStore
        /// </exception>
        /// <exception cref="ArgumentNullException"></exception>
        public CreatePersonalAccessTokenService(ITokenService tokenService, IClientStore clientStore, IResourceStore resourceStore)
        {
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _clientStore = clientStore ?? throw new ArgumentNullException(nameof(clientStore));
            _resourceStore = resourceStore ?? throw new ArgumentNullException(nameof(resourceStore));
        }

        /// <summary>
        /// Create a personal access token for the current user and client
        /// </summary>
        /// <param name="context">The Http context</param>
        /// <param name="isRefenceToken">Creeate a reference token if true otherwise a JWT token</param>
        /// <param name="lifetimeDays">The token life time</param>
        /// <param name="apis">The list of audience</param>
        /// <param name="scopes">The list of scopes</param>
        /// <param name="claimTypes">The list of claims types</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">apis</exception>
        /// <exception cref="System.InvalidOperationException">Client id not found in user claims.
        /// or
        /// Client not found for client id '{clientId}'.
        /// or
        /// Api '{api}' not found in '{client.ClientName}' allowed scopes.
        /// or
        /// Scope '{scope}' not found in '{client.ClientName}' allowed scopes.</exception>
        public async Task<string> CreatePersonalAccessTokenAsync(HttpContext context,
            bool isRefenceToken,
            int lifetimeDays,
            IEnumerable<string> apis, 
            IEnumerable<string> scopes, 
            IEnumerable<string> claimTypes)
        {
            CheckParams(apis);

            scopes ??= Array.Empty<string>();
            claimTypes ??= Array.Empty<string>();

            claimTypes = claimTypes.Where(c => c != JwtClaimTypes.Name &&
                c != JwtClaimTypes.ClientId &&
                c != JwtClaimTypes.Subject);

            var user = new ClaimsPrincipal(new ClaimsIdentity(context.User.Claims.Select(c =>
            {
                if (JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.TryGetValue(c.Type, out string newType))
                {
                    return new Claim(newType, c.Value);
                }
                return c;
            }), "Bearer", JwtClaimTypes.Name, JwtClaimTypes.Role));

            var clientId = user.Claims.First(c => c.Type == JwtClaimTypes.ClientId).Value;
            await ValidateRequestAsync(apis, scopes, user, clientId).ConfigureAwait(false);

            var issuer = context.GetIdentityServerIssuerUri();
            var sub = user.FindFirstValue(JwtClaimTypes.Subject) ?? user.FindFirstValue("nameid");
            var userName = user.Identity.Name;

            return await _tokenService.CreateSecurityTokenAsync(new Token(IdentityServerConstants.TokenTypes.AccessToken)
            {
                AccessTokenType = isRefenceToken ? AccessTokenType.Reference : AccessTokenType.Jwt,
                Audiences = apis.ToArray(),
                ClientId = clientId,
                Claims = user.Claims.Where(c => claimTypes.Any(t => c.Type == t))
                    .Concat(new[]
                    {
                        new Claim(JwtClaimTypes.Name, userName),
                        new Claim(JwtClaimTypes.ClientId, clientId),
                        new Claim(JwtClaimTypes.Subject, sub)
                    })
                    .Concat(scopes.Select(s => new Claim("scope", s)))
                    .ToArray(),
                CreationTime = DateTime.UtcNow,
                Lifetime = Convert.ToInt32(TimeSpan.FromDays(lifetimeDays).TotalSeconds),
                Issuer = issuer
            });
        }

        private static void CheckParams(IEnumerable<string> apis)
        {
            if (apis == null)
            {
                throw new ArgumentNullException(nameof(apis));
            }
        }

        private async Task ValidateRequestAsync(IEnumerable<string> apis, IEnumerable<string> scopes, ClaimsPrincipal user, string clientId)
        {
            var client = await _clientStore.FindEnabledClientByIdAsync(user.Claims.First(c => c.Type == "client_id").Value).ConfigureAwait(false);
            if (client == null)
            {
                throw new InvalidOperationException($"Client not found for client id '{clientId}'.");
            }

            var apiList = await _resourceStore.FindApiScopesByNameAsync(apis).ConfigureAwait(false);
            foreach (var api in apis)
            {
                if (!apiList.Any(a => a.Name == api))
                {
                    throw new InvalidOperationException($"Api '{api}' not found.");
                }
            }

            var allowedScopes = client.AllowedScopes;
            foreach (var scope in scopes)
            {
                if (!allowedScopes.Any(s => s == scope))
                {
                    throw new InvalidOperationException($"Scope '{scope}' not found in '{clientId}' allowed scopes.");
                }
            }
        }       
    }
}
