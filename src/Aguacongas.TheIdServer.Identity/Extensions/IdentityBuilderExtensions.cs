// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Microsoft.AspNetCore.Identity;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{

    /// <summary>
    /// Contains extension methods to <see cref="IdentityBuilder"/> for adding TheIdServer stores.
    /// </summary>
    public static class IdentityBuilderExtensions
    {                
        /// <summary>
        /// Adds the identifier server stores.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IdentityBuilder AddTheIdServerStores(this IdentityBuilder builder)
        {
            builder.Services.AddTheIdServerStores(builder.UserType, builder.RoleType);
            return builder;
        }
    }
}