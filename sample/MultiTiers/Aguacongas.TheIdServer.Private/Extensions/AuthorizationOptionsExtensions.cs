// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;

namespace Microsoft.AspNetCore.Authorization
{
    public static class AuthorizationOptionsExtensions
    {
        public static void AddIdentityServerPolicies(this AuthorizationOptions options)
        {
            options.AddPolicy(SharedConstants.WRITERPOLICY, policy =>
                   policy.RequireAssertion(context =>
                       context.User.IsInRole(SharedConstants.WRITERPOLICY)));
            options.AddPolicy(SharedConstants.READERPOLICY, policy =>
               policy.RequireAssertion(context =>
                   context.User.IsInRole(SharedConstants.READERPOLICY)));
        }
    }
}
