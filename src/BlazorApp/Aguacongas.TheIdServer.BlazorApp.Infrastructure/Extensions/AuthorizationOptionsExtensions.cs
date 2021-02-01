﻿// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;

namespace Microsoft.AspNetCore.Authorization
{
    public static class AuthorizationOptionsExtensions
    {
        public static void AddIdentityServerPolicies(this AuthorizationOptions options)
        {
            options.AddPolicy(SharedConstants.WRITER, policy =>
                   policy.RequireAssertion(context => context.User.Identity.IsAuthenticated &&
                    context.User.IsInRole(SharedConstants.WRITER)));
            options.AddPolicy(SharedConstants.READER, policy =>
                   policy.RequireAssertion(context => context.User.Identity.IsAuthenticated &&
                    context.User.IsInRole(SharedConstants.READER)));
            options.AddPolicy(SharedConstants.REGISTRATION, policy =>
                   policy.RequireAssertion(context => context.User.Identity.IsAuthenticated &&
                    context.User.IsInRole(SharedConstants.REGISTRATION)));
        }
    }
}
