// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Abstractions;
using Aguacongas.IdentityServer.Admin.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Setup claims providers in DI.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns></returns>
        public static IServiceCollection AddClaimsProviders(this IServiceCollection services, IConfiguration configuration)
        {
            var providerSetupOptions = configuration.GetSection("ClaimsProviderOptions").Get<IEnumerable<ClaimsProviderSetupOptions>>();
            if (providerSetupOptions == null)
            {
                return services;
            }

            foreach(var options in providerSetupOptions)
            {
#pragma warning disable S3885 // "Assembly.Load" should be used
                var assembly = Assembly.LoadFrom(options.AssemblyPath);
#pragma warning restore S3885 // "Assembly.Load" should be used
                var type = assembly.GetType(options.TypeName);
                var setup = Activator.CreateInstance(type) as ISetupClaimsProvider;
                setup.SetupClaimsProvider(services, configuration);
            }
            return services;
        }
    }
}
