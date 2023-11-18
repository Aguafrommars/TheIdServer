using Aguacongas.TheIdServer.Identity.UpgradePasswordHasher;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// <see cref="IdentityBuilder"/> extensions
/// </summary>
public static class IdentityBuilderExtensions
{
    /// <summary>
    /// Add Upgrade password hasher services in DI
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <param name="builder"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static IdentityBuilder AddUpgradePasswordHasher<TUser>(this IdentityBuilder builder, Action<UpgradePasswordHasherOptions>? configure = null) where TUser : class
    {
        builder.Services.AddUpgradePasswordHasher<TUser>(configure);
        return builder;
    }

    /// <summary>
    /// Add Upgrade password hasher services in DI
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <param name="builder"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IdentityBuilder AddUpgradePasswordHasher<TUser>(this IdentityBuilder builder, IConfiguration configuration) where TUser : class
    {
        builder.Services.AddUpgradePasswordHasher<TUser>(configuration);
        return builder;
    }
}
