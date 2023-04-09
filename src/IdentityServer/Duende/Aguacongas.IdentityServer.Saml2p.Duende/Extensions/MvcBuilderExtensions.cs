using Aguacongas.IdentityServer.Saml2p.Duende.Services.Configuration;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// <see cref="IMvcBuilder"/> extensions
/// </summary>
public static class MvcBuilderExtensions
{
    /// <summary>
    /// Adds identity server Saml2P controller.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="configuration">Configuration to bind to <see cref="Saml2POptions"/></param>
    /// <returns></returns>
    public static IMvcBuilder AddIdentityServerSaml2P(this IMvcBuilder builder, IConfiguration configuration)
    {
        builder.Services.AddIdentityServerSaml2P(configuration);
        builder.AddApplicationPart(typeof(MvcBuilderExtensions).Assembly);
        return builder;
    }

    /// <summary>
    /// Adds identity server Saml2P controller.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="options">Options to configure metadata</param>
    /// <returns></returns>
    public static IMvcBuilder AddIdentityServerSaml2P(this IMvcBuilder builder, Saml2POptions? options = null)
    {
        builder.Services.AddIdentityServerSaml2P(options);
        builder.AddApplicationPart(typeof(MvcBuilderExtensions).Assembly);
        return builder;
    }
}
