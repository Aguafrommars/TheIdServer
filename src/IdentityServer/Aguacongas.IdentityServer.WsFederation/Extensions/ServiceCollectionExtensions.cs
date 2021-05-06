// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.WsFederation;
using Aguacongas.IdentityServer.WsFederation.Stores;
using Aguacongas.IdentityServer.WsFederation.Validation;
using IdentityServer4.Validation;

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
        /// <returns></returns>
        public static IServiceCollection AddIdentityServerWsFederation(this IServiceCollection services)
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
