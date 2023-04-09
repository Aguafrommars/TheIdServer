using Aguacongas.IdentityServer.Saml2p.Duende.Services;
using Aguacongas.IdentityServer.Saml2p.Duende.Services.Artifact;
using Aguacongas.IdentityServer.Saml2p.Duende.Services.Configuration;
using Aguacongas.IdentityServer.Saml2p.Duende.Services.Metadata;
using Aguacongas.IdentityServer.Saml2p.Duende.Services.Signin;
using Aguacongas.IdentityServer.Saml2p.Duende.Services.Store;
using Aguacongas.IdentityServer.Saml2p.Duende.Services.Validation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// <see cref="IServiceCollection"/> extensions
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds identity server Saml2P services to the DI
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddIdentityServerSaml2P(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<Saml2POptions>(configuration);
        return services.AddIdentityServerSaml2P();
    }

    /// <summary>
    /// Adds identity server Saml2P services to the DI
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddIdentityServerSaml2P(this IServiceCollection services, Saml2POptions? options = null)
    {
        if (options is not null)
        {
            services.Configure<Saml2POptions>(o =>
            {
                o.ContactPersons = options.ContactPersons;
                o.CertificateValidationMode = options.CertificateValidationMode;
                o.RevocationMode = options.RevocationMode;
                o.SignatureAlgorithm = options.SignatureAlgorithm;
                o.ValidUntil = options.ValidUntil;
            });
        }

        services.TryAddTransient<ISaml2ConfigurationService, Saml2ConfigurationService>();
        services.TryAddTransient<IArtifactStore, ArtifactStore>();
        services.TryAddTransient<IRelyingPartyStore, RelyingPartyStore>();
        services.TryAddTransient<ISignInValidator, SignInValidator>();
        services.TryAddTransient<IMetadataResponseGenerator, MetadataResponseGenerator>();
        services.TryAddTransient<ISignInResponseGenerator, SignInResponseGenerator>();
        services.TryAddTransient<ISaml2PService, Saml2PService>();
        return services;
    }
}
