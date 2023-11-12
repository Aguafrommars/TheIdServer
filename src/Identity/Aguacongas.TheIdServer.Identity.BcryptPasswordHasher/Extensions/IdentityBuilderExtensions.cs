using Aguacongas.TheIdServer.Identity.BcryptPasswordHasher;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// <see cref="IdentityBuilder"/> extensions
/// </summary>
public static class IdentityBuilderExtensions
{
    /// <summary>
    /// Add Bcrypt password hasher services in DI
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <param name="builder"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static IdentityBuilder AddBcryptPasswordHasher<TUser>(this IdentityBuilder builder, Action<BcryptPasswordHasherOptions>? configure = null) where TUser : class
    {
        builder.Services.AddBcryptPasswordHasher<TUser>(configure);
        return builder;
    }

    /// <summary>
    /// Add Bcrypt password hasher services in DI
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <param name="builder"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IdentityBuilder AddBcryptPasswordHasher<TUser>(this IdentityBuilder builder, IConfiguration configuration) where TUser : class
    {
        builder.Services.AddBcryptPasswordHasher<TUser>(configuration);
        return builder;
    }
}
