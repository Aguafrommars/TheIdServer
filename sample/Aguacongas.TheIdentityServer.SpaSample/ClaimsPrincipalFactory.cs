// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace Aguacongas.TheIdentityServer.SpaSample
{
    public class ClaimsPrincipalFactory : AccountClaimsPrincipalFactory<RemoteUserAccount>
    {
        private readonly ILogger<ClaimsPrincipalFactory> _logger;

        public ClaimsPrincipalFactory(IAccessTokenProviderAccessor accessor, ILogger<ClaimsPrincipalFactory> logger) : base(accessor)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override async ValueTask<ClaimsPrincipal> CreateUserAsync(RemoteUserAccount account, RemoteAuthenticationUserOptions options)
        {
            var user = await base.CreateUserAsync(account, options);
            if (user.Identity.IsAuthenticated)
            {
                var identity = user.Identity as ClaimsIdentity;
                _logger.LogInformation($"User connected {identity.Name}");
                foreach (var claim in user.Claims.ToArray())
                {
                    var value = claim.Value;
                    if (value.StartsWith("["))
                    {
                        var values = JsonSerializer.Deserialize<IEnumerable<string>>(value);
                        var type = claim.Type;
                        foreach (var item in values)
                        {
                            _logger.LogDebug($"Add {type} claim {item}");
                            identity.AddClaim(new Claim(type, item));
                        }
                        identity.RemoveClaim(claim);
                    }
                }
            }

            return user;
        }
    }
}
