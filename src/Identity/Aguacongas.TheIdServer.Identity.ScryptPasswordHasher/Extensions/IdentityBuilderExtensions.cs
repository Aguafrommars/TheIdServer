using Aguacongas.TheIdServer.Identity.ScryptPasswordHasher;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// <see cref="IdentityBuilder"/> extensions
/// </summary>
public static class IdentityBuilderExtensions
{
    /// <summary>
    /// Add Scrypt password hasher services in DI
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <param name="builder"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static IdentityBuilder AddScryptPasswordHasher<TUser>(this IdentityBuilder builder, Action<ScryptPasswordHasherOptions>? configure = null) where TUser : class
    {
        builder.Services.AddScryptPasswordHasher<TUser>(configure);
        return builder;
    }

    /// <summary>
    /// Add Scrypt password hasher services in DI
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <param name="builder"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IdentityBuilder AddScryptPasswordHasher<TUser>(this IdentityBuilder builder, IConfiguration configuration) where TUser : class
    {
        builder.Services.AddScryptPasswordHasher<TUser>(configuration);
        return builder;
    }
}
