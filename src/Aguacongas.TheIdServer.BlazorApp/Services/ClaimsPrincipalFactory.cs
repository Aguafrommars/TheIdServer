using Aguacongas.TheIdServer.BlazorApp.Models;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Services
{
    public class ClaimsPrincipalFactory : AccountClaimsPrincipalFactory<OidcAccount>
    {
        private readonly ILogger<ClaimsPrincipalFactory> _logger;

        public ClaimsPrincipalFactory(IAccessTokenProviderAccessor accessor, ILogger<ClaimsPrincipalFactory> logger) : base(accessor)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override async ValueTask<ClaimsPrincipal> CreateUserAsync(OidcAccount account, RemoteAuthenticationUserOptions options)
        {
            var user = await base.CreateUserAsync(account, options);
            if (user.Identity.IsAuthenticated)
            {
                var identity = user.Identity as ClaimsIdentity;
                _logger.LogInformation($"User connected {identity.Name}");
                foreach (var value in account.Roles)
                {
                    _logger.LogInformation($"Add role claim {value}");
                    identity.AddClaim(new Claim("role", value));
                }
            }
            
            return user;
        }
    }
}
