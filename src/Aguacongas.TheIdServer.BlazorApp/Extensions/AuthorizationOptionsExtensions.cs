using Aguacongas.IdentityServer.Store;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Linq;

namespace Microsoft.AspNetCore.Authorization
{
    public static class AuthorizationOptionsExtensions
    {
        public static void AddIdentityServerPolicies(this AuthorizationOptions options)
        {
            options.AddPolicy(SharedConstants.WRITER, policy =>
                   policy.RequireAssertion(context => context.User.Identity.IsAuthenticated &&
                    context.User.Claims
                        .Any(c => c.Type == "role" && c.Value.Contains(SharedConstants.WRITER))
                   ));
            options.AddPolicy(SharedConstants.READER, policy =>
                   policy.RequireAssertion(context => context.User.Identity.IsAuthenticated &&
                    context.User.Claims
                        .Any(c => c.Type == "role" && c.Value.Contains(SharedConstants.READER))
                   ));
        }
    }
}
