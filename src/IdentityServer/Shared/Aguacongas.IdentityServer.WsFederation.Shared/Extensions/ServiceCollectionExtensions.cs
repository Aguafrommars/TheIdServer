// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.IdentityServer.WsFederation;
using Aguacongas.IdentityServer.WsFederation.Stores;
using Aguacongas.IdentityServer.WsFederation.Validation;
#if DUENDE
using Duende.IdentityServer.Validation;
#else
using IdentityServer4.Validation;
#endif
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// IServiceCollection extensions to add WS-Federation servives
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds WS-Federation services.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="configuration">Configuration to bind to <see cref="WsFederationOptions"/></param>
        /// <returns></returns>
        public static IServiceCollection AddIdentityServerWsFederation(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<WsFederationOptions>(configuration);
            return AddWsFederationSevices(services);
        }

        /// <summary>
        /// Adds WS-Federation services.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="options">Options to configure metadata</param>
        /// <returns></returns>
        public static IServiceCollection AddIdentityServerWsFederation(this IServiceCollection services, WsFederationOptions options = null)
        {
            services.Configure<WsFederationOptions>(o =>
            {
                if (options == null)
                {
                    return;
                }
                o.ClaimTypesOffered = options.ClaimTypesOffered;
                o.ClaimTypesRequested = o.ClaimTypesRequested;
            });
            return AddWsFederationSevices(services);
        }

        private static IServiceCollection AddWsFederationSevices(IServiceCollection services)
        {
            return services.AddTransient<IMetadataResponseGenerator, MetadataResponseGenerator>()
                .AddTransient<IWsFederationService, WsFederationService>()
                .AddTransient<ISignInValidator, SignInValidator>()
                .AddTransient<ISignInResponseGenerator, SignInResponseGenerator>()
                .AddTransient<IRelyingPartyStore, RelyingPartyStore>()
                .AddTransient<EndSessionRequestValidator>()
                .AddTransient<IEndSessionRequestValidator, WsFederationEndSessionRequestValidator>();
        }
    }
}
