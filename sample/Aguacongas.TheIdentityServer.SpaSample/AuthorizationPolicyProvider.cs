using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aguacongas.TheIdentityServer.SpaSample
{
    public class AuthorizationPolicyProvider : IAuthorizationPolicyProvider
    {
        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            return Task.FromResult(null as AuthorizationPolicy);
        }

        public Task<AuthorizationPolicy> GetFallbackPolicyAsync()
        {
            return Task.FromResult(null as AuthorizationPolicy);
        }

        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            return Task.FromResult(null as AuthorizationPolicy);
        }
    }
}
